using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Celeste.Mod.SSMQoLMod.Modifications
{
    /// <summary>
    /// Handles the functionality for skipping routines.
    /// </summary>
    public class SkipController : Entity
    {
        public Action OnSkip;

        private const float skipTime = 0.73f;
        private const float timeBeforeRender = 0.2f;
        private const float totalRenderTime = skipTime - timeBeforeRender;

        private float timer;

        private static readonly Vector2 circlePosition = new Vector2(100f, 100f);

        private static readonly Vector2[] texturePositions = new Vector2[]
        {
            circlePosition + new Vector2(60f, 0f),
            circlePosition + new Vector2(60f, 60f),
            circlePosition + new Vector2(0f, 60f),
            circlePosition
        };

        private static readonly float[] textureRotations = new float[]
        {
            0f,
            90f * Calc.DegToRad,
            180f * Calc.DegToRad,
            270f * Calc.DegToRad
        };

        public SkipController()
        {
            Tag = Tags.HUD;
            timer = 0f;
        }

        public override void Update()
        {
            base.Update();

            if (Input.MenuConfirm.Check)
            {
                timer += Engine.DeltaTime;
            }
            else
            {
                timer = 0f;
            }
            if (timer > skipTime)
            {
                OnSkip?.Invoke();
            }
        }

        public override void Render()
        {
            base.Render();

            float renderTimer = timer - timeBeforeRender;
            if (renderTimer > 0f)
            {
                float alpha = Calc.ClampedMap(renderTimer, 0f, 0.15f);
                float quarterStartTime = totalRenderTime / 4f;
                for (int i = 0; i < 4; i++)
                {
                    float currentQuarterStartTime = quarterStartTime * i;
                    if (renderTimer > currentQuarterStartTime)
                    {
                        float currentRemainder = renderTimer - currentQuarterStartTime;
                        int textureID = (int)Calc.Clamp(currentRemainder / Engine.DeltaTime, 0, 7);
                        MTexture quarterCircle = GFX.Gui[$"ssmqol/quartercircle{textureID:D2}"];
                        quarterCircle.Draw(texturePositions[i], new Vector2(30f, 30f), Color.White * alpha, 1f, textureRotations[i]);
                    }
                }
            }
        }

        public static void SkipRoutine(string routineName)
        {
            Audio.BusStopAll("bus:/ui_sfx", immediate: false);
            foreach (Entity entity in Engine.Scene.Entities)
            {
                foreach (Component component in entity)
                {
                    if (component is Coroutine coroutine)
                    {
                        // kinda wish there was a nicer way to do this but oh well
                        DynData<Coroutine> coroutineData = new DynData<Coroutine>(coroutine);
                        Stack<IEnumerator> enumerators = coroutineData.Get<Stack<IEnumerator>>("enumerators");
                        while (enumerators.Any(e => e.GetType().Name == routineName))
                        {
                            enumerators.Pop();
                        }
                    }
                }
            }
        }
    }

    public static class SkipControllerHooks
    {

        private static ILHook owLoaderRoutineHook;
        private static MethodInfo owLoaderRoutineInfo =
            typeof(OverworldLoader).GetMethod("Routine", BindingFlags.Instance | BindingFlags.NonPublic)
            .GetStateMachineTarget();

        private static ILHook levelEnterRoutineHook;
        private static MethodInfo levelEnterRoutineInfo =
            typeof(LevelEnter).GetMethod("Routine", BindingFlags.Instance | BindingFlags.NonPublic)
            .GetStateMachineTarget();

        public static void Load()
        {
            owLoaderRoutineHook = new ILHook(owLoaderRoutineInfo, IL_OverworldLoader_Routine);
            On.Celeste.Postcard.ctor_string_string_string += On_Postcard_ctor;
            levelEnterRoutineHook = new ILHook(levelEnterRoutineInfo, IL_LevelEnter_Routine);
        }

        public static void Unload()
        {
            owLoaderRoutineHook?.Dispose();
            owLoaderRoutineHook = null;
            On.Celeste.Postcard.ctor_string_string_string -= On_Postcard_ctor;
            levelEnterRoutineHook?.Dispose();
            levelEnterRoutineHook = null;
        }

        /// <summary>
        /// Hook to modify C-side postcard wait time.
        /// </summary>
        private static void IL_OverworldLoader_Routine(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            // look for constant 3f, only places it's used in the method are for wait times
            while (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(3f)))
            {
                cursor.EmitDelegate<Func<float, float>>(ModifyWaitTime);
            }
        }

        private static float ModifyWaitTime(float time)
        {
            if (SSMQoLModule.Settings.SkippablePostcards && SSMQoLModule.Settings.SkipCSidePostcardWait)
            {
                time = 1f;
            }
            return time;
        }

        /// <summary>
        /// Hook to add the ability to skip postcards.
        /// </summary>
        private static void On_Postcard_ctor(
            On.Celeste.Postcard.orig_ctor_string_string_string orig, Postcard self, string msg, string sfxEventIn, string sfxEventOut)
        {
            orig(self, msg, sfxEventIn, sfxEventOut);
            if (SSMQoLModule.Settings.SkippablePostcards)
            {
                Engine.Scene.Add(new SkipController
                {
                    OnSkip = () => SkipController.SkipRoutine("<DisplayRoutine>d__13")
                });
            }
        }

        /// <summary>
        /// Hook to add the ability to skip B-sides.
        /// </summary>
        private static void IL_LevelEnter_Routine(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            FieldInfo f_this = levelEnterRoutineInfo.DeclaringType.GetField("<>4__this");
            if (cursor.TryGotoNext(MoveType.After,
                instr => instr.MatchNewobj(out _),
                instr => instr.MatchStfld(out _)))
            {
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.Emit(OpCodes.Ldfld, f_this);
                cursor.EmitDelegate<Action<LevelEnter>>(AddBSideSkip);
            }
        }

        private static void AddBSideSkip(LevelEnter levelEnter)
        {
            if (SSMQoLModule.Settings.SkippableBSideIntro)
            {
                Engine.Scene.Add(new SkipController
                {
                    OnSkip = () =>
                    {
                        // finish up the rest of the setup
                        Session session = new DynData<LevelEnter>(levelEnter).Get<Session>("session");
                        Input.SetLightbarColor(AreaData.Get(session.Area).TitleBaseColor);
                        Engine.Scene = new LevelLoader(session);
                        // then skip the routine
                        SkipController.SkipRoutine("<Routine>d__5");
                    }
                });
            }
        }
    }
}
