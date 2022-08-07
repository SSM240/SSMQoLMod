using System;
using System.Collections.Generic;

namespace Celeste.Mod.SSMQoLMod
{
    public class SSMQoLSession : EverestModuleSession
    {
        public HashSet<EntityID> DisabledLookoutIDs { get; set; } = new HashSet<EntityID>();
    }
}
