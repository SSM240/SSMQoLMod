// the below is too janky for me to want to actually include it lol

//using Microsoft.Xna.Framework;
//using Monocle;
//using MonoMod.Utils;
//using System;
//using System.Collections.Generic;
//using TAS.EverestInterop.InfoHUD;

//namespace Celeste.Mod.SSMQoLMod
//{
//    public static class DisableEntityCommand
//    {
//        private static DynamicData entityExtensionsData;

//        private static readonly EverestModuleMetadata CelesteTASMetadata = new EverestModuleMetadata
//        {
//            Name = "CelesteTAS",
//            Version = new Version(3, 6, 5)
//        };
//        private static bool CelesteTASLoaded
//        {
//            get
//            {
//                bool loaded = Everest.Loader.DependencyLoaded(CelesteTASMetadata);
//                if (loaded && entityExtensionsData == null)
//                {
//                    entityExtensionsData = new DynamicData(Type.GetType("TAS.Utils.EntityExtensions, CelesteTAS-EverestInterop"));
//                }
//                return loaded;
//            }
//        }

//        private static readonly HashSet<EntityID> disabledEntities = new HashSet<EntityID>();

//        private static EntityData GetEntityData(this Entity entity)
//        {
//            return entityExtensionsData.Invoke<EntityData>("GetEntityData", entity);
//        }
//        private static EntityID ToEntityId(this EntityData entityData)
//        {
//            return entityExtensionsData.Invoke<EntityID>("ToEntityId", entityData);
//        }

//        [Command("disable_entity", "Removes entity for the remainder of the current session (requires CelesteTAS)")]
//        private static void TryCmdDisableEntity()
//        {
//            if (CelesteTASLoaded)
//            {
//                CmdDisableEntity();
//            }
//            else
//            {
//                Engine.Commands.Log("CelesteTAS must be loaded to use this command", Color.Yellow);
//            }
//        }

//        [Command("clear_disabled_entities", "Un-disables entities that were disabled by disable_entity")]
//        private static void TryCmdClearDisabledEntities()
//        {
//            if (CelesteTASLoaded)
//            {
//                CmdClearDisabledEntities();
//            }
//            else
//            {
//                Engine.Commands.Log("CelesteTAS must be loaded to use this command", Color.Yellow);
//            }
//        }

//        private static void CmdDisableEntity()
//        {
//            Entity clickedEntity = InfoWatchEntity.FindClickedEntity();
//            if (clickedEntity == null)
//            {
//                Engine.Commands.Log("no entity selected");
//                return;
//            }
//            if (clickedEntity.GetEntityData() is { } entityData && Engine.Scene is Level level)
//            {
//                EntityID clickedEntityID = entityData.ToEntityId();
//                level.Session.DoNotLoad.Add(clickedEntityID);
//                disabledEntities.Add(clickedEntityID);
//                clickedEntity.RemoveSelf();
//                string clickedEntityName = clickedEntity.GetType().Name;
//                Engine.Commands.Log($"{clickedEntityName} {clickedEntityID} removed and added to DoNotLoad");
//            }
//            else
//            {
//                Engine.Commands.Log("could not find entity data");
//            }
//        }

//        private static void CmdClearDisabledEntities()
//        {
//            foreach (EntityID id in disabledEntities)
//            {
//                if (Engine.Scene is Level level)
//                {
//                    level.Session.DoNotLoad.Remove(id);
//                }
//            }
//            disabledEntities.Clear();
//            Engine.Commands.Log("entities removed from DoNotLoad");
//        }
//    }
//}
