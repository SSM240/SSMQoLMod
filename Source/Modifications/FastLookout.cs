using Celeste.Mod.CommunalHelper.Entities;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using VivHelper.Entities;

namespace Celeste.Mod.SSMQoLMod.Modifications
{
    public static class FastLookout
    {
        private static BindingFlags instancePrivate = BindingFlags.Instance | BindingFlags.NonPublic;
        private static ILHook lookRoutineHook;
        private static MethodInfo lookRoutineInfo =
            typeof(Lookout).GetMethod("LookRoutine", instancePrivate).GetStateMachineTarget();

        #region Mod info

        private static EverestModuleMetadata communalHelperMetadata = new()
        {
            Name = "CommunalHelper",
            Version = new Version(1, 17, 3)
        };
        private static EverestModuleMetadata vivHelperMetadata = new()
        {
            Name = "VivHelper",
            Version = new Version(1, 12, 1)
        };

        private static MethodInfo communalHelper_NoOverlayLookRoutineInfo;
        private static ILHook communalHelper_NoOverlayLookRoutineHook;

        private static MethodInfo vivHelper_CustomPlaybackLookRoutineInfo;
        private static ILHook vivHelper_CustomPlaybackLookRoutineHook;

        private static MethodInfo vivHelper_PlatinumLookRoutineInfo;
        private static ILHook vivHelper_PlatinumLookRoutineHook;

        #endregion

        public static void Load()
        {
            lookRoutineHook = new ILHook(lookRoutineInfo, IL_Lookout_LookRoutine);
            if (Everest.Loader.DependencyLoaded(communalHelperMetadata))
            {
                LoadCommunalHelperHooks();
            }
            if (Everest.Loader.DependencyLoaded(vivHelperMetadata))
            {
                LoadVivHelperHooks();
            }
        }

        private static void LoadCommunalHelperHooks()
        {
            communalHelper_NoOverlayLookRoutineInfo =
                typeof(NoOverlayLookout).GetMethod("LookRoutine", instancePrivate).GetStateMachineTarget();
            communalHelper_NoOverlayLookRoutineHook = 
                new ILHook(communalHelper_NoOverlayLookRoutineInfo, IL_CommunalHelper_NoOverlayLookout_LookRoutine);
        }

        private static void LoadVivHelperHooks()
        {
            vivHelper_CustomPlaybackLookRoutineInfo =
                typeof(CustomPlaybackWatchtower).GetMethod("LookRoutine", instancePrivate).GetStateMachineTarget();
            vivHelper_CustomPlaybackLookRoutineHook =
                new ILHook(vivHelper_CustomPlaybackLookRoutineInfo, IL_VivHelper_CustomPlaybackWatchtower_LookRoutine);

            vivHelper_PlatinumLookRoutineInfo =
                typeof(PlatinumWatchtower).GetMethod("LookRoutine", instancePrivate).GetStateMachineTarget();
            vivHelper_PlatinumLookRoutineHook =
                new ILHook(vivHelper_PlatinumLookRoutineInfo, IL_VivHelper_PlatinumWatchtower_LookRoutine);
        }

        public static void Unload()
        {
            lookRoutineHook?.Dispose();
            lookRoutineHook = null;

            communalHelper_NoOverlayLookRoutineHook?.Dispose();
            communalHelper_NoOverlayLookRoutineHook = null;
            communalHelper_NoOverlayLookRoutineInfo = null;

            vivHelper_CustomPlaybackLookRoutineHook?.Dispose();
            vivHelper_CustomPlaybackLookRoutineHook = null;
            vivHelper_CustomPlaybackLookRoutineInfo = null;

            vivHelper_PlatinumLookRoutineHook?.Dispose();
            vivHelper_PlatinumLookRoutineHook = null;
            vivHelper_PlatinumLookRoutineInfo = null;
        }

        private static void IL_Lookout_LookRoutine(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            string routineTypeName = lookRoutineInfo.DeclaringType.FullName;
            while (cursor.TryGotoNext(MoveType.After,
                instr => instr.MatchLdfld(routineTypeName, "<maxspd>5__4")))
            {
                cursor.EmitDelegate(ModifySpeed);
            }
            cursor.Index = 0;
            while (cursor.TryGotoNext(MoveType.After,
                instr => instr.MatchLdfld(routineTypeName, "<accel>5__3")))
            {
                cursor.EmitDelegate(ModifySpeed);
            }
        }

        #region Mod hooks

        private static void IL_CommunalHelper_NoOverlayLookout_LookRoutine(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            string routineTypeName = communalHelper_NoOverlayLookRoutineInfo.DeclaringType.FullName;
            while (cursor.TryGotoNext(MoveType.After,
                instr => instr.MatchLdfld(routineTypeName, "<maxSpeed>5__4")))
            {
                cursor.EmitDelegate(ModifySpeed);
            }
            cursor.Index = 0;
            while (cursor.TryGotoNext(MoveType.After,
                instr => instr.MatchLdfld(routineTypeName, "<accel>5__3")))
            {
                cursor.EmitDelegate(ModifySpeed);
            }
        }

        private static void IL_VivHelper_CustomPlaybackWatchtower_LookRoutine(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            while (cursor.TryGotoNext(MoveType.After,
                instr => instr.MatchLdfld(typeof(CustomPlaybackWatchtower), "maxSpeed")))
            {
                cursor.EmitDelegate(ModifySpeed);
            }
            cursor.Index = 0;
            while (cursor.TryGotoNext(MoveType.After,
                instr => instr.MatchLdfld(typeof(CustomPlaybackWatchtower), "accel")))
            {
                cursor.EmitDelegate(ModifySpeed);
            }
        }

        private static void IL_VivHelper_PlatinumWatchtower_LookRoutine(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            while (cursor.TryGotoNext(MoveType.After,
                instr => instr.MatchLdfld(typeof(PlatinumWatchtower), "maxSpd")))
            {
                cursor.EmitDelegate(ModifySpeed);
            }
            cursor.Index = 0;
            while (cursor.TryGotoNext(MoveType.After,
                instr => instr.MatchLdfld(typeof(PlatinumWatchtower), "accel")))
            {
                cursor.EmitDelegate(ModifySpeed);
            }
        }

        #endregion

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
