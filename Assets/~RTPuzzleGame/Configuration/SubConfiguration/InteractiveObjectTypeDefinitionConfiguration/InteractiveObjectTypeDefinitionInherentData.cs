using GameConfigurationID;
using System;
using System.Collections.Generic;
using UnityEngine;
using static RTPuzzle.ActionInteractableObjectModule;
using static RTPuzzle.AttractiveObjectModule;
using static RTPuzzle.DisarmObjectModule;
using static RTPuzzle.GrabObjectModule;
using static RTPuzzle.LaunchProjectileModule;
using static RTPuzzle.LevelCompletionTriggerModule;
using static RTPuzzle.ModelObjectModule;
using static RTPuzzle.NearPlayerGameOverTriggerModule;
using static RTPuzzle.TargetZoneModule;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "InteractiveObjectTypeDefinitionInherentData", menuName = "Configuration/PuzzleGame/InteractiveObjectTypeDefinitionConfiguration/InteractiveObjectTypeDefinitionInherentData", order = 1)]
    public class InteractiveObjectTypeDefinitionInherentData : AbstractObjectDefinitionConfigurationInherentData
    {
        public InteractiveObjectID InteractiveObjectID;

        public override List<Type> ModuleTypes => InteractiveObjectModuleTypesConstants.InteractiveObjectModuleTypes;

        public void DefineInteractiveObject(InteractiveObjectType InteractiveObjectType, PuzzlePrefabConfiguration puzzlePrefabConfiguration, 
                        PuzzleGameConfiguration puzzleGameConfiguration, RangeTypeObjectDefinitionInherentData LevelCompletionZoneDefinition = null)
        {
            if (this.RangeDefinitionModulesActivation != null && this.RangeDefinitionModules != null)
            {
                if (InteractiveObjectID != InteractiveObjectID.NONE)
                {
                    InteractiveObjectType.InteractiveObjectID = InteractiveObjectID;
                }

                this.DestroyExistingModules(InteractiveObjectType.gameObject);

                foreach (var rangeDefinitionModuleActivation in this.RangeDefinitionModulesActivation)
                {
                    if (rangeDefinitionModuleActivation.Value)
                    {
                        var moduleConfiguration = this.RangeDefinitionModules[rangeDefinitionModuleActivation.Key];

                        if (moduleConfiguration.GetType() == typeof(TargetZoneModuleDefinition))
                        {
                            var TargetZoneModuleDefinition = (TargetZoneModuleDefinition)moduleConfiguration;
                            var TargetZoneModule = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseTargetZoneModule, InteractiveObjectType.transform);
                            TargetZoneModuleInstancer.PopulateFromDefinition(TargetZoneModule, TargetZoneModuleDefinition);
                        }
                        else if (moduleConfiguration.GetType() == typeof(LevelCompletionTriggerModuleDefinition))
                        {
                            var LevelCompletionTriggerModuleDefinition = (LevelCompletionTriggerModuleDefinition)moduleConfiguration;
                            var LevelCompletionTriggerModule = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseLevelCompletionTriggerModule, InteractiveObjectType.transform);
                            LevelCompletionTriggerModuleInstancer.PopuplateFromDefinition(LevelCompletionTriggerModule, LevelCompletionTriggerModuleDefinition, puzzlePrefabConfiguration, LevelCompletionZoneDefinition);
                        }
                        else if (moduleConfiguration.GetType() == typeof(InteractiveObjectCutsceneControllerModuleDefinition))
                        {
                            var InteractiveObjectCutsceneControllerModuleDefinition = (InteractiveObjectCutsceneControllerModuleDefinition)moduleConfiguration;
                            MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseInteractiveObjectCutsceneControllerModule, InteractiveObjectType.transform);
                        }
                        else if (moduleConfiguration.GetType() == typeof(ActionInteractableObjectModuleDefinition))
                        {
                            var ActionInteractableObjectModuleDefinition = (ActionInteractableObjectModuleDefinition)moduleConfiguration;
                            var ActionInteractableObjectModule = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseActionInteractableObjectModule, InteractiveObjectType.transform);
                            ActionInteractableObjectModuleInstancer.PopuplateFromDefinition(ActionInteractableObjectModule, ActionInteractableObjectModuleDefinition);
                        }
                        else if (moduleConfiguration.GetType() == typeof(NearPlayerGameOverTriggerModuleDefinition))
                        {
                            var NearPlayerGameOverTriggerModuleDefinition = (NearPlayerGameOverTriggerModuleDefinition)moduleConfiguration;
                            var NearPlayerGameOverTriggerModule = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseNearPlayerGameOverTriggerModule, InteractiveObjectType.transform);
                            NearPlayerGameOverTriggerModuleInstancer.PopuplateFromDefinition(NearPlayerGameOverTriggerModule, NearPlayerGameOverTriggerModuleDefinition);
                        }
                        else if (moduleConfiguration.GetType() == typeof(LaunchProjectileModuleDefinition))
                        {
                            var LaunchProjectileModuleDefinition = (LaunchProjectileModuleDefinition)moduleConfiguration;
                            var LaunchProjectileModule = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseLaunchProjectileModule, InteractiveObjectType.transform);
                            LaunchProjectileModuleInstancer.PopuplateFromDefinition(LaunchProjectileModule, LaunchProjectileModuleDefinition);
                        }
                        else if (moduleConfiguration.GetType() == typeof(AttractiveObjectModuleDefinition))
                        {
                            var AttractiveObjectModuleDefinition = (AttractiveObjectModuleDefinition)moduleConfiguration;
                            var AttractiveObjectModule =  MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseAttractiveObjectModule, InteractiveObjectType.transform);
                            AttractiveObjectModuleInstancer.PopuplateFromDefinition(AttractiveObjectModule, AttractiveObjectModuleDefinition, puzzlePrefabConfiguration, puzzleGameConfiguration);
                        }
                        else if (moduleConfiguration.GetType() == typeof(ModelObjectModuleDefinition))
                        {
                            var ModelObjectModuleDefinition = (ModelObjectModuleDefinition)moduleConfiguration;
                            var ModelObjectModule = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseModelObjectModule, InteractiveObjectType.transform);
                            ModelObjectModuleInstancer.PopuplateFromDefinition(ModelObjectModule, ModelObjectModuleDefinition);
                        }
                        else if (moduleConfiguration.GetType() == typeof(DisarmObjectModuleDefinition))
                        {
                            var DisarmObjectModuleDefinition = (DisarmObjectModuleDefinition)moduleConfiguration;
                            var DisarmObjectModule = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseDisarmObjectModule, InteractiveObjectType.transform);
                            DisarmObjectModuleInstancer.PopuplateFromDefinition(DisarmObjectModule, DisarmObjectModuleDefinition);
                        }
                        else if (moduleConfiguration.GetType() == typeof(GrabObjectModuleDefinition))
                        {
                            var GrabObjectModuleDefinition = (GrabObjectModuleDefinition)moduleConfiguration;
                          var GrabObjectModule =  MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseGrabObjectModule, InteractiveObjectType.transform);
                            GrabObjectModuleInstancer.PopuplateFromDefinition(GrabObjectModule, GrabObjectModuleDefinition);
                        }
//${addNewEntry}
                    }
                }
            }
        }
    }

    public static class InteractiveObjectTypeDefinitionConfigurationInherentDataBuilder
    {
        public static InteractiveObjectTypeDefinitionInherentData TargetZone()
        {
            return new InteractiveObjectTypeDefinitionInherentData()
            {
                RangeDefinitionModules = new Dictionary<Type, ScriptableObject>()
                {
                    {typeof(TargetZoneModuleDefinition), new TargetZoneModuleDefinition() },
                    {typeof(LevelCompletionTriggerModuleDefinition), new LevelCompletionTriggerModuleDefinition() }
                },
                RangeDefinitionModulesActivation = new Dictionary<Type, bool>()
                {
                    {typeof(TargetZoneModuleDefinition), true },
                    {typeof(LevelCompletionTriggerModuleDefinition), true }
                }
            };
        }
    }
}