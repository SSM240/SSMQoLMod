using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.SSMQoLMod.Modifications
{
    public static class KeepToggleGrabOnDeath
    {
        public static void Load()
        {
            IL.Celeste.Player.ctor += IL_Player_ctor;
            Everest.Events.Level.OnEnter += Level_OnEnter;
        }

        public static void Unload()
        {
            IL.Celeste.Player.ctor -= IL_Player_ctor;
            Everest.Events.Level.OnEnter -= Level_OnEnter;
        }

        private static void IL_Player_ctor(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            if (cursor.TryGotoNext(instr => instr.MatchCall(typeof(Input).FullName, "ResetGrab")))
            {
                cursor.Remove();
                cursor.EmitDelegate(ResetGrabIfEnabled);
            }
        }

        private static void ResetGrabIfEnabled()
        {
            if (!SSMQoLModule.Settings.KeepToggleGrabOnDeath)
            {
                Input.ResetGrab();
            }
        }

        private static void Level_OnEnter(Session session, bool fromSaveData)
        {
            if (SSMQoLModule.Settings.KeepToggleGrabOnDeath)
            {
                // still reset when first entering a level
                Input.ResetGrab();
            }
        }
    }
}
