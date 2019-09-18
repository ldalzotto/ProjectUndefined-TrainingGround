using GameConfigurationID;
using OdinSerializer;

namespace RTPuzzle
{
    public class TargetZoneModuleDefinition : AbstractInteractiveObjectDefinition
    {
        [CustomEnum(configurationType: typeof(TargetZoneConfiguration))]
        public TargetZoneID TargetZoneID;
    }
}
