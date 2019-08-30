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
                if (InteractiveObjectInitializationObject.AttractiveObjectInherentConfigurationData == null)
                {
                    attractiveObjectTypeModule.Init(interactiveObjectType.PuzzleGameConfigurationManager.AttractiveObjectsConfiguration()[attractiveObjectTypeModule.AttractiveObjectId], interactiveObjectType.GetModule<ModelObjectModule>(), interactiveObjectType.PuzzleEventsManager);
                }
                else
                {
                    attractiveObjectTypeModule.Init(InteractiveObjectInitializationObject.AttractiveObjectInherentConfigurationData, interactiveObjectType.GetModule<ModelObjectModule>(), interactiveObjectType.PuzzleEventsManager);
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
                if (interactiveObjectInitializationObject.DisarmObjectInherentData == null) { disarmObjectModule.Init(interactiveObjectType.GetModule<ModelObjectModule>(), interactiveObjectType, interactiveObjectType.PuzzleGameConfigurationManager.DisarmObjectsConfiguration()[disarmObjectModule.DisarmObjectID]); }
                else { disarmObjectModule.Init(interactiveObjectType.GetModule<ModelObjectModule>(), interactiveObjectType, interactiveObjectInitializationObject.DisarmObjectInherentData); }
            });

        }

        public static void InitializeActionInteractableObjectModule(InteractiveObjectInitializationObject interactiveObjectInitializationObject, InteractiveObjectType interactiveObjectType)
        {
            interactiveObjectType.GetModule<ActionInteractableObjectModule>().IfNotNull((ActionInteractableObjectModule ActionInteractableObjectModule) =>
            {
                if (interactiveObjectInitializationObject.ActionInteractableObjectInherentData == null) { ActionInteractableObjectModule.Init(interactiveObjectType.PuzzleGameConfigurationManager.ActionInteractableObjectConfiguration()[ActionInteractableObjectModule.ActionInteractableObjectID], interactiveObjectType, interactiveObjectType.PuzzleGameConfigurationManager, interactiveObjectType.PuzzleEventsManager, interactiveObjectType.GetModule<ModelObjectModule>()); }
                else { ActionInteractableObjectModule.Init(interactiveObjectInitializationObject.ActionInteractableObjectInherentData, interactiveObjectType, interactiveObjectType.PuzzleGameConfigurationManager, interactiveObjectType.PuzzleEventsManager, interactiveObjectType.GetModule<ModelObjectModule>()); }
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
                if (interactiveObjectInitializationObject.GrabObjectInherentData == null) { GrabObjectModule.Init(interactiveObjectType.PuzzleGameConfigurationManager.GrabObjectConfiguration()[GrabObjectModule.GrabObjectID], interactiveObjectType.GetModule<ModelObjectModule>()); }
                else { GrabObjectModule.Init(interactiveObjectInitializationObject.GrabObjectInherentData, interactiveObjectType.GetModule<ModelObjectModule>()); }
            });
        }
        //------------------------------------------------------------------------------
        // <auto-generated>
        //     This code was generated by a tool.
        // </auto-generated>
        //------------------------------------------------------------------------------
        public static void InitializeInteractiveObjectCutsceneControllerModule(InteractiveObjectInitializationObject interactiveObjectInitializationObject, InteractiveObjectType interactiveObjectType)
        {
            interactiveObjectType.GetModule<InteractiveObjectCutsceneControllerModule>().IfNotNull((InteractiveObjectCutsceneControllerModule InteractiveObjectCutsceneControllerModule) =>
                       InteractiveObjectCutsceneControllerModule.Init(interactiveObjectType.GetModule<ModelObjectModule>(), interactiveObjectInitializationObject));
        }
        //------------------------------------------------------------------------------
        // <auto-generated>
        //     This code was generated by a tool.
        // </auto-generated>
        //------------------------------------------------------------------------------
        public static void InitializeNearPlayerGameOverTriggerModule(InteractiveObjectInitializationObject interactiveObjectInitializationObject, InteractiveObjectType interactiveObjectType)
        {
            interactiveObjectType.GetModule<NearPlayerGameOverTriggerModule>().IfNotNull((NearPlayerGameOverTriggerModule NearPlayerGameOverTriggerModule) => NearPlayerGameOverTriggerModule.Init(interactiveObjectType.GetModule<ObjectSightModule>(), interactiveObjectType));
        }

        public static void InitializeObjectSightModule(InteractiveObjectInitializationObject interactiveObjectInitializationObject, InteractiveObjectType interactiveObjectType)
        {
            interactiveObjectType.GetModule<ObjectSightModule>().IfNotNull((ObjectSightModule ObjectSightModule) => ObjectSightModule.Init());
        }
        //${addNewEntry}
        #endregion
    }
}
