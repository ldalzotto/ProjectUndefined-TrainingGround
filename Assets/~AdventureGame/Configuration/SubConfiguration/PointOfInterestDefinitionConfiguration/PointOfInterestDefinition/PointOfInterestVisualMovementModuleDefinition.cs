using GameConfigurationID;
using OdinSerializer;

namespace AdventureGame
{
    public class PointOfInterestVisualMovementModuleDefinition : SerializedScriptableObject
    {
        [CustomEnum(ConfigurationType = typeof(PointOfInterestVisualMovementConfiguration))]
        public PointOfInterestVisualMovementID PointOfInterestVisualMovementID;
    }

}
