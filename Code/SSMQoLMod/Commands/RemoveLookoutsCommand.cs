using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.SSMQoLMod.Commands
{
    public static class RemoveLookoutsCommand
    {
        [Command("remove_lookouts", "Removes all lookouts (watchtowers) in the current room for the remainder of the session")]
        public static void CmdRemoveLookouts()
        {
            if (!(Engine.Scene is Level level))
            {
                Engine.Commands.Log("Not currently in a level", Color.Yellow);
                return;
            }
            int lookoutsFound = 0;
            foreach (Lookout lookout in level.Tracker.GetEntities<Lookout>())
            {
                lookoutsFound++;
                EntityID id = new DynamicData(lookout).Get<EntityID>("EntityID");
                level.Session.DoNotLoad.Add(id);
                SSMQoLModule.Session.DisabledLookoutIDs.Add(id);
                lookout.RemoveSelf();
            }
            if (lookoutsFound > 0)
            {
                string s = lookoutsFound > 1 ? "s" : "";
                Engine.Commands.Log($"Removed {lookoutsFound} lookout{s} from the current room");
            }
            else
            {
                Engine.Commands.Log("No lookouts found in the current room");
            }
        }

        [Command("restore_lookouts", "Restores all lookouts removed by remove_lookouts")]
        public static void CmdRestoreLookouts()
        {
            if (!(Engine.Scene is Level level))
            {
                Engine.Commands.Log("Not currently in a level", Color.Yellow);
                return;
            }
            int currentRoomLookoutCount = 0;
            int otherRoomlookoutCount = 0;
            foreach (EntityID id in SSMQoLModule.Session.DisabledLookoutIDs)
            {
                if (id.Level == level.Session.Level)
                {
                    currentRoomLookoutCount++;
                }
                else
                {
                    otherRoomlookoutCount++;
                }
                level.Session.DoNotLoad.Remove(id);
            }
            SSMQoLModule.Session.DisabledLookoutIDs.Clear();
            if (otherRoomlookoutCount > 0)
            {
                string s = otherRoomlookoutCount > 1 ? "s" : "";
                Engine.Commands.Log($"Restored {otherRoomlookoutCount} lookout{s} in other rooms");
            }
            if (currentRoomLookoutCount > 0)
            {
                string s = currentRoomLookoutCount > 1 ? "s" : "";
                Engine.Commands.Log($"Restored {currentRoomLookoutCount} lookout{s} in the current room (requires room reload)");
            }
            if (otherRoomlookoutCount == 0 && currentRoomLookoutCount == 0)
            {
                Engine.Commands.Log("No removed lookouts to restore");
            }
        }

        public static void Load()
        {
            On.Celeste.Lookout.ctor += On_Lookout_ctor;
        }

        public static void Unload()
        {
            On.Celeste.Lookout.ctor -= On_Lookout_ctor;
        }

        private static void On_Lookout_ctor(On.Celeste.Lookout.orig_ctor orig, Lookout self, EntityData data, Vector2 offset)
        {
            orig(self, data, offset);
            new DynamicData(self).Set("EntityID", new EntityID(data.Level.Name, data.ID));
        }
    }
}
