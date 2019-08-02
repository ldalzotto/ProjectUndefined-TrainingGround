using Editor_ActionInteractableObjectCreationWizard;
using Editor_AttractiveObjectCreationWizard;
using Editor_DisarmObjectCreationWizard;
using Editor_MainGameCreationWizard;
using Editor_LaunchProjectileCreationWizard;
using Editor_ObjectRepelCreationWizard;
using Editor_TargetZoneCreationWizard;
using GameConfigurationID;
using RTPuzzle;
using System;
using System.Collections.Generic;

namespace Editor_GameDesigner
{
    public static class InteractiveObjectModuleWizardConfiguration
    {
        public static Dictionary<Type, InteractiveObjectModuleConfigurationProfile> InteractiveObjectModuleConfiguration = new Dictionary<Type, InteractiveObjectModuleConfigurationProfile>()
        {
            {typeof(ModelObjectModule), new InteractiveObjectModuleConfigurationProfile(
                        new InteractiveObjectNonIdentifiedEditOperation((CommonGameConfigurations CommonGameConfigurations) => CommonGameConfigurations.PuzzleInteractiveObjectModulePrefabs.BaseModelObjectModule)
                      , new NoPrefabEditConditionOperation()
                      , "Model object definition." )
            },
            {typeof(ObjectRepelModule), new InteractiveObjectModuleConfigurationProfile(
                    new InteractiveObjectIdentifiedEditOperation(
                        (CommonGameConfigurations) => { return CommonGameConfigurations.PuzzleInteractiveObjectModulePrefabs.BaseObjectRepelModule; },
                        (CommonGameConfigurations) => CommonGameConfigurations.PuzzleGameConfigurations.ObjectRepelConfiguration,
                        (module) => ((ObjectRepelModule)module).ObjectRepelID, (module, id) => ((ObjectRepelModule)module).ObjectRepelID = (ObjectRepelID)id,
                        (InteractiveObjectModuleWizardID) => InteractiveObjectModuleWizardID.ObjectRepelID
                      ),
                    new PrefabIdentifiableConditionOperation((CommonGameConfigurations) => CommonGameConfigurations.PuzzleGameConfigurations.ObjectRepelConfiguration,
                            (InteractiveObjectModuleWizardID) => InteractiveObjectModuleWizardID.ObjectRepelID,
                            typeof(ObjectRepelCreationWizard)),
                    "Allow the object to be repelled by projectile."
                    )
            },
            {typeof(AttractiveObjectModule), new InteractiveObjectModuleConfigurationProfile(
                new InteractiveObjectIdentifiedEditOperation(
                    (CommonGameConfigurations) => CommonGameConfigurations.PuzzleInteractiveObjectModulePrefabs.BaseAttractiveObjectModule,
                    (CommonGameConfigurations) => CommonGameConfigurations.PuzzleGameConfigurations.AttractiveObjectConfiguration,
                    (module) => ((AttractiveObjectModule)module).AttractiveObjectId, (module, id) => ((AttractiveObjectModule)module).AttractiveObjectId = (AttractiveObjectId)id,
                    (InteractiveObjectModuleWizardID) => InteractiveObjectModuleWizardID.AttractiveObjectId
                ),
                new PrefabIdentifiableConditionOperation((CommonGameConfigurations) => CommonGameConfigurations.PuzzleGameConfigurations.AttractiveObjectConfiguration,
                            (InteractiveObjectModuleWizardID) => InteractiveObjectModuleWizardID.AttractiveObjectId,
                            typeof(AttractiveObjectCreationWizard)),
                "Attract AI when on range."
                )
            },
            {typeof(TargetZoneModule), new InteractiveObjectModuleConfigurationProfile(
                new InteractiveObjectIdentifiedEditOperation(
                    (CommonGameConfigurations) => CommonGameConfigurations.PuzzleInteractiveObjectModulePrefabs.BaseTargetZoneModule,
                    (CommonGameConfigurations) => CommonGameConfigurations.PuzzleGameConfigurations.TargetZoneConfiguration,
                    (module) => ((TargetZoneModule)module).TargetZoneID, (module, id) => ((TargetZoneModule)module).TargetZoneID = (TargetZoneID)id,
                    (InteractiveObjectModuleWizardID) => InteractiveObjectModuleWizardID.TargetZoneID
                ),
                new PrefabIdentifiableConditionOperation((CommonGameConfigurations) => CommonGameConfigurations.PuzzleGameConfigurations.TargetZoneConfiguration,
                            (InteractiveObjectModuleWizardID) => InteractiveObjectModuleWizardID.TargetZoneID,
                            typeof(TargetZoneCreationWizard)),
                "Definie a zone for AI to reach to complete level."
                )
            },
            {typeof(LevelCompletionTriggerModule), new InteractiveObjectModuleConfigurationProfile(
                new InteractiveObjectNonIdentifiedEditOperation((CommonGameConfigurations CommonGameConfigurations) => CommonGameConfigurations.PuzzleInteractiveObjectModulePrefabs.BaseLevelCompletionTriggerModule),
                new NoPrefabEditConditionOperation(),
                "Trigger Zone used to trigger end of puzzle level event.")
            },
            {typeof(LaunchProjectileModule), new InteractiveObjectModuleConfigurationProfile(
                 new InteractiveObjectIdentifiedEditOperation(
                    (CommonGameConfigurations) => CommonGameConfigurations.PuzzleInteractiveObjectModulePrefabs.BaseLaunchProjectileModule,
                    (CommonGameConfigurations) => CommonGameConfigurations.PuzzleGameConfigurations.LaunchProjectileConfiguration,
                    (module) => ((LaunchProjectileModule)module).LaunchProjectileID, (module, id) => ((LaunchProjectileModule)module).LaunchProjectileID = (LaunchProjectileID)id,
                    (InteractiveObjectModuleWizardID) => InteractiveObjectModuleWizardID.LaunchProjectileId
                ),
                   new PrefabIdentifiableConditionOperation((CommonGameConfigurations) => CommonGameConfigurations.PuzzleGameConfigurations.LaunchProjectileConfiguration,
                            (InteractiveObjectModuleWizardID) => InteractiveObjectModuleWizardID.LaunchProjectileId,
                            typeof(LaunchProjectileCreationWizard)),
                   "Projectile following a path and do action on contact."
                 )
            },
            { typeof(DisarmObjectModule), new InteractiveObjectModuleConfigurationProfile(
                new InteractiveObjectIdentifiedEditOperation(
                    (CommonGameConfigurations) => CommonGameConfigurations.PuzzleInteractiveObjectModulePrefabs.BaseDisarmObjectModule,
                    (CommonGameConfigurations) => CommonGameConfigurations.PuzzleGameConfigurations.DisarmObjectConfiguration,
                    (module) => ((DisarmObjectModule)module).DisarmObjectID, (module, id) => ((DisarmObjectModule)module).DisarmObjectID = (DisarmObjectID)id,
                    (InteractiveObjectModuleWizardID) => InteractiveObjectModuleWizardID.DisarmObjectID
                ),
                   new PrefabIdentifiableConditionOperation((CommonGameConfigurations) => CommonGameConfigurations.PuzzleGameConfigurations.DisarmObjectConfiguration,
                            (InteractiveObjectModuleWizardID) => InteractiveObjectModuleWizardID.DisarmObjectID,
                            typeof(DisarmObjectCreationWizard)),
                   "The object can be deleted by AI."
                )
            },
            { typeof(ActionInteractableObjectModule), new InteractiveObjectModuleConfigurationProfile(
                new InteractiveObjectIdentifiedEditOperation(
                    (CommonGameConfigurations) => CommonGameConfigurations.PuzzleInteractiveObjectModulePrefabs.BaseActionInteractableObjectModule,
                    (CommonGameConfigurations) => CommonGameConfigurations.PuzzleGameConfigurations.ActionInteractableObjectConfiguration,
                    (module) => ((ActionInteractableObjectModule)module).ActionInteractableObjectID, (module, id) => ((ActionInteractableObjectModule)module).ActionInteractableObjectID = (ActionInteractableObjectID)id,
                    (InteractiveObjectModuleWizardID) => InteractiveObjectModuleWizardID.ActionInteractableObjectID
                ),
                 new PrefabIdentifiableConditionOperation((CommonGameConfigurations) => CommonGameConfigurations.PuzzleGameConfigurations.ActionInteractableObjectConfiguration,
                            (InteractiveObjectModuleWizardID) => InteractiveObjectModuleWizardID.ActionInteractableObjectID,
                            typeof(ActionInteractableObjectCreationWizard)),
                 "The object can be interacted by player."
                )
            },
            //${addNewEntry}
        };

    }
}
