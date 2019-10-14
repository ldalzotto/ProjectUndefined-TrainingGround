using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    [SceneHandleDraw]
    [System.Serializable]
    [CreateAssetMenu(fileName = "RoundedFrustumRangeObjectInitialization", menuName = "Test/RoundedFrustumRangeObjectInitialization", order = 1)]
    public class RoundedFrustumRangeObjectInitialization : RangeObjectInitialization
    {
        [DrawNested]
        public RoundedFrustumRangeTypeDefinition RoundedFrustumRangeTypeDefinition;
    }

}
