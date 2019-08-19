using GameConfigurationID;
using OdinSerializer;

namespace RTPuzzle
{
    public class TargetZoneModuleDefinition : SerializedScriptableObject
    {
        [CustomEnum(configurationType: typeof(TargetZoneConfiguration))]
        public TargetZoneID TargetZoneID;
    }
}
