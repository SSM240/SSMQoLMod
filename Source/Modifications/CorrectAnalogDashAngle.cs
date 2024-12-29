using MonoMod.Cil;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.SSMQoLMod.Modifications
{
    public static class CorrectAnalogDashAngle
    {
        public static void Load()
        {
            IL.Celeste.Input.GetAimVector += IL_Input_GetAimVector;
        }

        public static void Unload()
        {
            IL.Celeste.Input.GetAimVector -= IL_Input_GetAimVector;
        }

        private static void IL_Input_GetAimVector(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            if (cursor.TryGotoNext(instr => instr.MatchLdcR4(0.08726646f)))
            {
                cursor.EmitDelegate(CorrectAngleCalculation);
            }
        }

        public static float CorrectAngleCalculation(float radDiff)
        {
            if (SSMQoLModule.Settings.CorrectAnalogDashAngle)
            {
                return 0f;
            }
            else
            {
                return radDiff;
            }
        }
    }
}
