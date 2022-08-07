using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Celeste.Mod.SSMQoLMod.Modifications
{
    public static class FastLookout
    {
        private static ILHook lookRoutineHook;
        private static MethodInfo lookRoutineInfo =
            typeof(Lookout).GetMethod("LookRoutine", BindingFlags.Instance | BindingFlags.NonPublic)
            .GetStateMachineTarget();

        public static void Load()
        {
            lookRoutineHook = new ILHook(lookRoutineInfo, IL_Lookout_LookRoutine);
        }

        public static void Unload()
        {
            lookRoutineHook?.Dispose();
            lookRoutineHook = null;
        }

        private static void IL_Lookout_LookRoutine(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            while (cursor.TryGotoNext(MoveType.After,
                instr => instr.MatchLdfld(lookRoutineInfo.DeclaringType.FullName, "<maxspd>5__4")))
            {
                cursor.EmitDelegate<Func<float, float>>(ModifySpeed);
            }
            cursor.Index = 0;
            while (cursor.TryGotoNext(MoveType.After,
                instr => instr.MatchLdfld(lookRoutineInfo.DeclaringType.FullName, "<accel>5__3")))
            {
                cursor.EmitDelegate<Func<float, float>>(ModifySpeed);
            }
        }

        private static float ModifySpeed(float speed)
        {
            if (SSMQoLModule.Settings.FastLookout && SSMQoLModule.Settings.FastLookoutButton.Check)
            {
                speed *= SSMQoLModule.Settings.FastLookoutMultiplier;
            }
            return speed;
        }
    }
}
