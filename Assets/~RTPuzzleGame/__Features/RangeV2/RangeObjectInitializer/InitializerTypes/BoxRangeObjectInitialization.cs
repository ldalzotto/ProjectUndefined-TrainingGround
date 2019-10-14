using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    [System.Serializable]
    [SceneHandleDraw]
    [CreateAssetMenu(fileName = "BoxRangeObjectInitialization", menuName = "Test/BoxRangeObjectInitialization", order = 1)]
    public class BoxRangeObjectInitialization : RangeObjectInitialization
    {
        [DrawNested]
        public BoxRangeTypeDefinition BoxRangeTypeDefinition;
    }
}
