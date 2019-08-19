using System;
using System.Collections.Generic;
using UnityEngine;
using static RTPuzzle.LevelCompletionTriggerModule;
using static RTPuzzle.TargetZoneModule;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "InteractiveObjectTypeDefinitionConfigurationInherentData", menuName = "Configuration/PuzzleGame/InteractiveObjectTypeDefinitionConfiguration/InteractiveObjectTypeDefinitionConfigurationInherentData", order = 1)]
    public class InteractiveObjectTypeDefinitionConfigurationInherentData : AbstractObjectDefinitionConfigurationInherentData
    {
        public static List<Type> InteractiveObjectModuleTypes = new List<Type>() { typeof(TargetZoneModuleDefinition), typeof(LevelCompletionTriggerModuleDefinition) };

        public override List<Type> ModuleTypes => InteractiveObjectTypeDefinitionConfigurationInherentData.InteractiveObjectModuleTypes;

        public void DefineInteractiveObject(InteractiveObjectType InteractiveObjectType, PuzzlePrefabConfiguration puzzlePrefabConfiguration, RangeTypeObjectDefinitionConfigurationInherentData LevelCompletionZoneDefinition = null)
        {
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
                            LevelCompletionTriggerModuleInstancer.PopuplateFromDefinition(LevelCompletionTriggerModule, LevelCompletionTriggerModuleDefinition, puzzlePrefabConfiguration, LevelCompletionZoneDefinition);
                        }
                    }
                }
            }
        }
    }

    public static class InteractiveObjectTypeDefinitionConfigurationInherentDataBuilder
    {
        public static InteractiveObjectTypeDefinitionConfigurationInherentData TargetZone()
        {
            return new InteractiveObjectTypeDefinitionConfigurationInherentData()
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
