using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.SSMQoLMod.Modifications
{
    public class DisableHeatWaveDistortion
    {
        public static void Load()
        {
            On.Celeste.HeatWave.Update += On_HeatWave_Update;
        }

        public static void Unload()
        {
            On.Celeste.HeatWave.Update -= On_HeatWave_Update;
        }

        private static void On_HeatWave_Update(On.Celeste.HeatWave.orig_Update orig, HeatWave self, Scene scene)
        {
            orig(self, scene);
            if (SSMQoLModule.Settings.DisableHeatWaveDistortion)
            {
                Distort.WaterAlpha = 0f;
            }
        }
    }
}
