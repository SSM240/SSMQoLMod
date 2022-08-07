using MonoMod.Utils;

namespace Celeste.Mod.SSMQoLMod.Modifications
{
    public static class DisablePlayerFlashing
    {
        public static void Load()
        {
            On.Celeste.Player.Render += On_Player_Render;
        }

        public static void Unload()
        {
            On.Celeste.Player.Render -= On_Player_Render;
        }

        private static void On_Player_Render(On.Celeste.Player.orig_Render orig, Player self)
        {
            if (SSMQoLModule.Settings.DisableLowStaminaFlashing)
            {
                DynData<Player> playerData = new DynData<Player>(self);
                playerData["flash"] = true;
            }
            orig(self);
        }
    }
}
