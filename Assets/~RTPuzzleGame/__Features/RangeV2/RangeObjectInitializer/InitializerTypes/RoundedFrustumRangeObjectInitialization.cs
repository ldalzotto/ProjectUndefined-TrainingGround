using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    [SceneHandleDraw]
    [System.Serializable]
    public class RoundedFrustumRangeObjectInitialization : RangeObjectInitialization
    {
        [DrawNested]
        public RoundedFrustumRangeTypeDefinition RoundedFrustumRangeTypeDefinition;
    }

}
