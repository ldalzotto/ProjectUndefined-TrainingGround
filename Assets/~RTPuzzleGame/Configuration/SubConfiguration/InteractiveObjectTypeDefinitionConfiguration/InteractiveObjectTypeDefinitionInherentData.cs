using GameConfigurationID;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static RTPuzzle.ActionInteractableObjectModule;
using static RTPuzzle.AILogicColliderModule;
using static RTPuzzle.AttractiveObjectModule;
using static RTPuzzle.DisarmObjectModule;
using static RTPuzzle.GrabObjectModule;
using static RTPuzzle.LaunchProjectileModule;
using static RTPuzzle.LevelCompletionTriggerModule;
using static RTPuzzle.ModelObjectModule;
using static RTPuzzle.NearPlayerGameOverTriggerModule;
using static RTPuzzle.ObjectRepelModule;
using static RTPuzzle.ObjectSightModule;
using static RTPuzzle.TargetZoneModule;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "InteractiveObjectTypeDefinitionInherentData", menuName = "Configuration/PuzzleGame/InteractiveObjectTypeDefinitionConfiguration/InteractiveObjectTypeDefinitionInherentData", order = 1)]
    public class InteractiveObjectTypeDefinitionInherentData : AbstractObjectDefinitionConfigurationInherentData
    {
        [CustomEnum()]
        public InteractiveObjectID InteractiveObjectID;

        public static List<Type> AbstractInteractiveObjectDefinitionTypes;

        private List<Type> GetAbstractInteractiveObjectDefinitionTypes()
        {
            if (AbstractInteractiveObjectDefinitionTypes == null)
            {
                AbstractInteractiveObjectDefinitionTypes = typeof(AbstractInteractiveObjectDefinition)
                    .Assembly.GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(AbstractInteractiveObjectDefinition)) && !t.IsAbstract)
                    .ToList();
            }
            return AbstractInteractiveObjectDefinitionTypes;
        }

        public override List<Type> ModuleTypes => GetAbstractInteractiveObjectDefinitionTypes();

        public InteractiveObjectSharedDataTypeInherentData InteractiveObjectSharedDataTypeInherentData;

        public void DefineInteractiveObject(InteractiveObjectType InteractiveObjectType, PuzzlePrefabConfiguration puzzlePrefabConfiguration,
                        PuzzleGameConfiguration puzzleGameConfiguration)
        {
            if (this.InteractiveObjectSharedDataTypeInherentData != null)
            {
                this.InteractiveObjectSharedDataTypeInherentData.DefineInteractiveObjectSharedDataType(InteractiveObjectType);
            }

            InteractiveObjectType.InteractiveObjectID = InteractiveObjectID;

            if (this.RangeDefinitionModulesActivation != null && this.RangeDefinitionModules != null)
            {
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
                            LevelCompletionTriggerModuleInstancer.PopuplateFromDefinition(LevelCompletionTriggerModule, LevelCompletionTriggerModuleDefinition, puzzlePrefabConfiguration);
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
                            AttractiveObjectModuleInstancer.PopuplateFromDefinition((AttractiveObjectModuleDefinition)moduleConfiguration, InteractiveObjectType.transform);
                        }
                        else if (moduleConfiguration.GetType() == typeof(ModelObjectModuleDefinition))
                        {
                            var ModelObjectModuleDefinition = (ModelObjectModuleDefinition)moduleConfiguration;
                            var ModelObjectModule = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseModelObjectModule, InteractiveObjectType.transform);
                            ModelObjectModuleInstancer.PopuplateFromDefinition(ModelObjectModule, ModelObjectModuleDefinition);
                        }
                        else if (moduleConfiguration.GetType() == typeof(DisarmObjectModuleDefinition))
                        {
                            DisarmObjectModuleInstancer.PopuplateFromDefinition((DisarmObjectModuleDefinition)moduleConfiguration, InteractiveObjectType.transform);
                        }
                        else if (moduleConfiguration.GetType() == typeof(GrabObjectModuleDefinition))
                        {
                            var GrabObjectModuleDefinition = (GrabObjectModuleDefinition)moduleConfiguration;
                            var GrabObjectModule = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseGrabObjectModule, InteractiveObjectType.transform);
                            GrabObjectModuleInstancer.PopuplateFromDefinition(GrabObjectModule, GrabObjectModuleDefinition);
                        }
                        else if (moduleConfiguration.GetType() == typeof(ObjectRepelModuleDefinition))
                        {
                            var ObjectRepelModuleDefinition = (ObjectRepelModuleDefinition)moduleConfiguration;
                            var ObjectRepelModule = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseObjectRepelModule, InteractiveObjectType.transform);
                            ObjectRepelModuleInstancer.PopuplateFromDefinition(ObjectRepelModule, ObjectRepelModuleDefinition);
                        }
                        else if (moduleConfiguration.GetType() == typeof(ObjectSightModuleDefinition))
                        {
                            var ObjectSightModuleDefinition = (ObjectSightModuleDefinition)moduleConfiguration;
                            var ObjectSightModule = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseObjectSightModule, InteractiveObjectType.transform);
                            ObjectSightModuleInstancer.PopuplateFromDefinition(ObjectSightModule, ObjectSightModuleDefinition, puzzlePrefabConfiguration);
                        }
                        else if (moduleConfiguration.GetType() == typeof(AILogicColliderModuleDefinition))
                        {
                            var AILogicColliderModuleDefinition = (AILogicColliderModuleDefinition)moduleConfiguration;
                            var AILogicColliderModule = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseAILogicColliderModule, InteractiveObjectType.transform);
                            AILogicColliderModuleInstancer.PopulateFromDefinition(AILogicColliderModule, AILogicColliderModuleDefinition);
                        }
                        else if (moduleConfiguration.GetType() == typeof(InRangeColliderTrackerModuleDefinition))
                        {
                            var InRangeColliderTrackerModuleDefinition = (InRangeColliderTrackerModuleDefinition)moduleConfiguration;
                            var InRangeColliderTrackerModule = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseInRangeColliderTrackerModule, InteractiveObjectType.transform);
                        }
                        else if (moduleConfiguration.GetType() == typeof(FovModuleDefinition))
                        {
                            var FovModuleDefinition = (FovModuleDefinition)moduleConfiguration;
                            var FovModule = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseFovModule, InteractiveObjectType.transform);
                        }
                        else if (moduleConfiguration.GetType() == typeof(ContextMarkVisualFeedbackModuleDefinition))
                        {
                            var ContextMarkVisualFeedbackModuleDefinition = (ContextMarkVisualFeedbackModuleDefinition)moduleConfiguration;
                            var ContextMarkVisualFeedbackModule = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseContextMarkVisualFeedbackModule, InteractiveObjectType.transform);
                        }
                        else if (moduleConfiguration.GetType() == typeof(LineVisualFeedbackModuleDefinition))
                        {
                            var LineVisualFeedbackModuleDefinition = (LineVisualFeedbackModuleDefinition)moduleConfiguration;
                            var LineVisualFeedbackModule = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseLineVisualFeedbackModule, InteractiveObjectType.transform);
                        }
//${addNewEntry}
                    }
                }
            }
        }
    }

    public static class InteractiveObjectTypeDefinitionConfigurationInherentDataBuilder
    {
        public static InteractiveObjectTypeDefinitionInherentData TargetZone(RangeTypeObjectDefinitionInherentData targetZoneRangeDefinition = null)
        {
            return new InteractiveObjectTypeDefinitionInherentData()
            {
                RangeDefinitionModules = new Dictionary<Type, ScriptableObject>()
                {
                    {typeof(TargetZoneModuleDefinition), new TargetZoneModuleDefinition() },
                    {typeof(LevelCompletionTriggerModuleDefinition), new LevelCompletionTriggerModuleDefinition() {
                            RangeTypeObjectDefinitionIDPicker = (targetZoneRangeDefinition == null),
                            RangeTypeObjectDefinitionInherentData = targetZoneRangeDefinition
                        }
                    }
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
