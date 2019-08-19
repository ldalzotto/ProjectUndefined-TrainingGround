using GameConfigurationID;
using OdinSerializer;

namespace RTPuzzle
{
    public class LevelCompletionTriggerModuleDefinition : SerializedScriptableObject
    {
        [CustomEnum(ConfigurationType = typeof(RangeTypeObjectDefinitionConfiguration))]
        public RangeTypeObjectDefinitionID RangeTypeObjectDefinitionID;
    }
}
