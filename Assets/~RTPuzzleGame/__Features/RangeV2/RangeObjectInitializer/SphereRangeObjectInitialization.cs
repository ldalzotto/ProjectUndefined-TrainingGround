using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "SphereRangeObjectInitialization", menuName = "Test/SphereRangeObjectInitialization", order = 1)]
    public class SphereRangeObjectInitialization : RangeObjectInitialization
    {
        public SphereRangeTypeDefinition SphereRangeTypeDefinition;
    }
}
