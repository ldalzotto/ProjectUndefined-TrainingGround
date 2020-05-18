using CoreGame;

namespace RTPuzzle
{
    public static class InteractiveObjectModulesInitializationOperations
    {
        #region Initialization

        public static void InitializeModelObjectModule(InteractiveObjectInitializationObject interactiveObjectInitializationObject, InteractiveObjectType interactiveObjectType)
        {
            interactiveObjectType.GetModule<ModelObjectModule>().IfNotNull((ModelObjectModule modelObjectModule) => modelObjectModule.Init());
        }

        public static void InitializeObjectRepelModule(InteractiveObjectInitializationObject interactiveObjectInitializationObject, InteractiveObjectType interactiveObjectType)
        {
            interactiveObjectType.GetModule<ObjectRepelModule>().IfNotNull((ObjectRepelModule objectRepelTypeModule) => objectRepelTypeModule.Init(interactiveObjectType.GetModule<ModelObjectModule>()));
        }

        public static void InitializeLevelCompletionTriggerModule(InteractiveObjectInitializationObject interactiveObjectInitializationObject, InteractiveObjectType interactiveObjectType)
        {
            interactiveObjectType.GetModule<LevelCompletionTriggerModule>().IfNotNull((LevelCompletionTriggerModule levelCompletionTriggerModule) => levelCompletionTriggerModule.Init());
        }

        public static void InitializeTargetZoneModule(InteractiveObjectInitializationObject interactiveObjectInitializationObject, InteractiveObjectType interactiveObjectType)
        {
            interactiveObjectType.GetModule<TargetZoneModule>().IfNotNull((TargetZoneModule targetZoneObjectModule) =>
            {
                if (interactiveObjectInitializationObject.TargetZoneInherentData == null) { targetZoneObjectModule.Init(interactiveObjectType.GetModule<LevelCompletionTriggerModule>()); }
                else { targetZoneObjectModule.Init(interactiveObjectType.GetModule<LevelCompletionTriggerModule>(), interactiveObjectInitializationObject.TargetZoneInherentData); }
            });
        }

        public static void InitializeAttractiveObjectModule(InteractiveObjectInitializationObject InteractiveObjectInitializationObject, InteractiveObjectType interactiveObjectType)
        {
            interactiveObjectType.GetModule<AttractiveObjectModule>().IfNotNull((AttractiveObjectModule attractiveObjectTypeModule) =>
            {
                if (InteractiveObjectInitializationObject.InputAttractiveObjectInherentConfigurationData == null)
                {
                    attractiveObjectTypeModule.Init(interactiveObjectType.PuzzleGameConfigurationManager.AttractiveObjectsConfiguration()[attractiveObjectTypeModule.AttractiveObjectId], interactiveObjectType.GetModule<ModelObjectModule>());
                }
                else
                {
                    attractiveObjectTypeModule.Init(InteractiveObjectInitializationObject.InputAttractiveObjectInherentConfigurationData, interactiveObjectType.GetModule<ModelObjectModule>());
                }
            }
            );
        }

        public static void InitializeLaunchProjectileModule(InteractiveObjectInitializationObject interactiveObjectInitializationObject, InteractiveObjectType interactiveObjectType)
        {
            interactiveObjectType.GetModule<LaunchProjectileModule>().IfNotNull((LaunchProjectileModule launchProjectileModule) =>
            {
                if (interactiveObjectInitializationObject.ProjectilePath != null)
                {
                    if (interactiveObjectInitializationObject.LaunchProjectileInherentData == null) { launchProjectileModule.Init(interactiveObjectType.PuzzleGameConfigurationManager.ProjectileConf()[interactiveObjectType.GetModule<LaunchProjectileModule>().LaunchProjectileID], interactiveObjectInitializationObject.ProjectilePath, interactiveObjectType.transform); }
                    else { launchProjectileModule.Init(interactiveObjectInitializationObject.LaunchProjectileInherentData, interactiveObjectInitializationObject.ProjectilePath, interactiveObjectType.transform); }
                }
            });
        }

        public static void InitializeDisarmObjectModule(InteractiveObjectInitializationObject interactiveObjectInitializationObject, InteractiveObjectType interactiveObjectType)
        {
            interactiveObjectType.GetModule<DisarmObjectModule>().IfNotNull((DisarmObjectModule disarmObjectModule) =>
            {
                if (interactiveObjectInitializationObject.DisarmObjectInherentData == null) { disarmObjectModule.Init(interactiveObjectType.GetModule<ModelObjectModule>(), interactiveObjectType.PuzzleGameConfigurationManager.DisarmObjectsConfiguration()[disarmObjectModule.DisarmObjectID]); }
                else { disarmObjectModule.Init(interactiveObjectType.GetModule<ModelObjectModule>(), interactiveObjectInitializationObject.DisarmObjectInherentData); }
            });

        }

        public static void InitializeActionInteractableObjectModule(InteractiveObjectInitializationObject interactiveObjectInitializationObject, InteractiveObjectType interactiveObjectType)
        {
            interactiveObjectType.GetModule<ActionInteractableObjectModule>().IfNotNull((ActionInteractableObjectModule ActionInteractableObjectModule) =>
            {
                if (interactiveObjectInitializationObject.ActionInteractableObjectInherentData == null) { ActionInteractableObjectModule.Init(interactiveObjectType.PuzzleGameConfigurationManager.ActionInteractableObjectConfiguration()[ActionInteractableObjectModule.ActionInteractableObjectID], interactiveObjectType.PuzzleGameConfigurationManager, interactiveObjectType.PuzzleEventsManager); }
                else { ActionInteractableObjectModule.Init(interactiveObjectInitializationObject.ActionInteractableObjectInherentData, interactiveObjectType.PuzzleGameConfigurationManager, interactiveObjectType.PuzzleEventsManager); }
            });
        }

        //------------------------------------------------------------------------------
        // <auto-generated>
        //     This code was generated by a tool.
        // </auto-generated>
        //------------------------------------------------------------------------------
        public static void InitializeGrabObjectModule(InteractiveObjectInitializationObject interactiveObjectInitializationObject, InteractiveObjectType interactiveObjectType)
        {
            interactiveObjectType.GetModule<GrabObjectModule>().IfNotNull((GrabObjectModule GrabObjectModule) =>
            {
                if (interactiveObjectInitializationObject.GrabObjectInherentData == null) { GrabObjectModule.Init(interactiveObjectType.PuzzleGameConfigurationManager.GrabObjectConfiguration()[GrabObjectModule.GrabObjectID]); }
                else { GrabObjectModule.Init(interactiveObjectInitializationObject.GrabObjectInherentData); }
            });
        }
//${addNewEntry}
        #endregion
    }
}
