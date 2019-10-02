using UnityEngine;
using System.Collections;


[System.Serializable]
[CreateAssetMenu(fileName = "FrustumRangeObjectInitialization", menuName = "Test/FrustumRangeObjectInitialization", order = 1)]
public class FrustumRangeObjectInitialization : RangeObjectInitialization
{
    public FrustumRangeTypeDefinition FrustumRangeTypeDefinition;
}