using GameConfigurationID;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
                        ((AbstractInteractiveObjectDefinition)this.RangeDefinitionModules[rangeDefinitionModuleActivation.Key])
                                .CreateObject(InteractiveObjectType.transform);
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
