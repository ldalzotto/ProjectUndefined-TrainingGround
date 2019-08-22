using GameConfigurationID;
using OdinSerializer;
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
    }
}
