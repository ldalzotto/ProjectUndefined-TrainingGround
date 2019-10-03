using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "RoundedFrustumRangeObjectInitialization", menuName = "Test/RoundedFrustumRangeObjectInitialization", order = 1)]
    public class RoundedFrustumRangeObjectInitialization : RangeObjectInitialization
    {
        public RoundedFrustumRangeTypeDefinition RoundedFrustumRangeTypeDefinition;
    }

}
