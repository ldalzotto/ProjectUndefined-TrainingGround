using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    [SceneHandleDraw]
    [System.Serializable]
    [CreateAssetMenu(fileName = "SphereRangeObjectInitialization", menuName = "Test/SphereRangeObjectInitialization", order = 1)]
    public class SphereRangeObjectInitialization : RangeObjectInitialization
    {
        [DrawNested]
        public SphereRangeTypeDefinition SphereRangeTypeDefinition;
    }
}
