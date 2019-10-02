using GameConfigurationID;
using OdinSerializer;

public abstract class RangeObjectInitialization : SerializedScriptableObject
{
    [CustomEnum()]
    public RangeTypeID RangeTypeID;
    public bool IsTakingIntoAccountObstacles;
}