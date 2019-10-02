using GameConfigurationID;
using OdinSerializer;


public abstract class RangeObjectInitialization : SerializedScriptableObject
{
    [CustomEnum()]
    public RangeTypeID RangeTypeID;
    public bool IsTakingIntoAccountObstacles;
}

public static class RangeObjectInitializationDataBuilderV2
{
    public static RangeObjectInitialization SphereRangeWithObstacleListener(float sphereRange, RangeTypeID rangeTypeID)
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
}