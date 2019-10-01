using GameConfigurationID;
using OdinSerializer;
using UnityEngine;

public abstract class RangeObjectInitialization : SerializedScriptableObject
{
    [CustomEnum()]
    public RangeTypeID RangeTypeID;
    public bool IsTakingIntoAccountObstacles;
}

[System.Serializable]
[CreateAssetMenu(fileName = "SphereRangeObjectInitialization", menuName = "Test/SphereRangeObjectInitialization", order = 1)]
public class SphereRangeObjectInitialization : RangeObjectInitialization
{
    public SphereRangeTypeDefinition SphereRangeTypeDefinition;
}

[System.Serializable]
[CreateAssetMenu(fileName = "BoxRangeObjectInitialization", menuName = "Test/BoxRangeObjectInitialization", order = 1)]
public class BoxRangeObjectInitialization : RangeObjectInitialization
{
    public BoxRangeTypeDefinition BoxRangeTypeDefinition;
}

[System.Serializable]
[CreateAssetMenu(fileName = "FrustumRangeObjectInitialization", menuName = "Test/FrustumRangeObjectInitialization", order = 1)]
public class FrustumRangeObjectInitialization : RangeObjectInitialization
{
    public FrustumRangeTypeDefinition FrustumRangeTypeDefinition;
}

[System.Serializable]
[CreateAssetMenu(fileName = "RoundedFrustumRangeObjectInitialization", menuName = "Test/RoundedFrustumRangeObjectInitialization", order = 1)]
public class RoundedFrustumRangeObjectInitialization : RangeObjectInitialization
{
    public RoundedFrustumRangeTypeDefinition RoundedFrustumRangeTypeDefinition;
}
