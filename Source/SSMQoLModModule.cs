using System;

namespace Celeste.Mod.SSMQoLMod;

public class SSMQoLModModule : EverestModule {
    public static SSMQoLModModule Instance { get; private set; }

    public override Type SettingsType => typeof(SSMQoLModModuleSettings);
    public static SSMQoLModModuleSettings Settings => (SSMQoLModModuleSettings) Instance._Settings;

    public override Type SessionType => typeof(SSMQoLModModuleSession);
    public static SSMQoLModModuleSession Session => (SSMQoLModModuleSession) Instance._Session;

    public SSMQoLModModule() {
        Instance = this;
#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(nameof(SSMQoLModModule), LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(nameof(SSMQoLModModule), LogLevel.Info);
#endif
    }

    public override void Load() {
        // TODO: apply any hooks that should always be active
    }

    public override void Unload() {
        // TODO: unapply any hooks applied in Load()
    }
}