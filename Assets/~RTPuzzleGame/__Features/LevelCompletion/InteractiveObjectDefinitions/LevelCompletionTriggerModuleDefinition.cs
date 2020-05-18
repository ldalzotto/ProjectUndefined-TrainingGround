﻿using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public class LevelCompletionTriggerModuleDefinition : AbstractInteractiveObjectDefinition
    {
        [ScriptableObjectSubstitution(substitutionName: nameof(LevelCompletionTriggerModuleDefinition.RangeTypeObjectDefinitionInherentData),
            sourcePickerName: nameof(LevelCompletionTriggerModuleDefinition.RangeTypeObjectDefinitionIDPicker))]
        [CustomEnum(ConfigurationType = typeof(RangeTypeObjectDefinitionConfiguration))]
        public RangeTypeObjectDefinitionID RangeTypeObjectDefinitionID;

        [Inline()]
        public RangeTypeObjectDefinitionInherentData RangeTypeObjectDefinitionInherentData;
        public bool RangeTypeObjectDefinitionIDPicker;

        public override void CreateObject(Transform parent)
        {
            var puzzlePrefabConfiguration = PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.GetPuzzlePrefabConfiguration();
            var LevelCompletionTriggerModule = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseLevelCompletionTriggerModule, parent);
            LevelCompletionTriggerModule.ResolveModuleDependencies();
            if (this.RangeTypeObjectDefinitionIDPicker)
            {
                LevelCompletionTriggerModule.RangeTypeObject.RangeTypeObjectDefinitionID = this.RangeTypeObjectDefinitionID;
            }
            else
            {
                LevelCompletionTriggerModule.RangeTypeObject.RangeTypeObjectDefinitionID = RangeTypeObjectDefinitionID.NONE;
                this.RangeTypeObjectDefinitionInherentData.DefineRangeTypeObject(LevelCompletionTriggerModule.RangeTypeObject, puzzlePrefabConfiguration);
            }

        }
    }
}
