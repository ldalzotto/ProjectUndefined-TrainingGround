using GameConfigurationID;
using OdinSerializer;
using UnityEngine;

namespace RTPuzzle
{
    public abstract class RangeObjectInitialization : SerializedScriptableObject
    {
        [CustomEnum()]
        public RangeTypeID RangeTypeID;
        public bool IsTakingIntoAccountObstacles;
    }

    public static class RangeObjectInitializationDataBuilderV2
    {
        public static SphereRangeObjectInitialization SphereRangeWithObstacleListener(float sphereRange, RangeTypeID rangeTypeID)
        {
            return new SphereRangeObjectInitialization
            {
                RangeTypeID = rangeTypeID,
                IsTakingIntoAccountObstacles = true,
                SphereRangeTypeDefinition = new SphereRangeTypeDefinition
                {
                    Radius = sphereRange
                }
            };
        }

        public static BoxRangeObjectInitialization BoxRangeNoObstacleListener(Vector3 center, Vector3 size, RangeTypeID rangeTypeID)
        {
            return new BoxRangeObjectInitialization
            {
                RangeTypeID = rangeTypeID,
                IsTakingIntoAccountObstacles = false,
                BoxRangeTypeDefinition = new BoxRangeTypeDefinition
                {
                    Center = center,
                    Size = size
                }
            };
        }
    }
}
