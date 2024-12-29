using System;
using Celeste.Mod.SSMQoLMod.Commands;
using Celeste.Mod.SSMQoLMod.Modifications;

namespace Celeste.Mod.SSMQoLMod
{
    public class SSMQoLModule : EverestModule
    {
        public static SSMQoLModule Instance;

        public SSMQoLModule()
        {
            Instance = this;
        }

        public override Type SettingsType => typeof(SSMQoLSettings);
        public static SSMQoLSettings Settings => (SSMQoLSettings)Instance._Settings;

        public override Type SessionType => typeof(SSMQoLSession);
        public static SSMQoLSession Session => (SSMQoLSession)Instance._Session;

        public override void Load()
        {
            DisablePlayerFlashing.Load();
            FastLookout.Load();
            SkipControllerHooks.Load();
            RemoveLookoutsCommand.Load();
            KeepToggleGrabOnDeath.Load();
            DisableHeatWaveDistortion.Load();
            CorrectAnalogDashAngle.Load();
        }

        public override void Unload()
        {
            DisablePlayerFlashing.Unload();
            FastLookout.Unload();
            SkipControllerHooks.Unload();
            RemoveLookoutsCommand.Unload();
            KeepToggleGrabOnDeath.Unload();
            DisableHeatWaveDistortion.Unload();
            CorrectAnalogDashAngle.Unload();
        }
    }
}
