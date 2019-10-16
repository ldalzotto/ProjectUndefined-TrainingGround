using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    [System.Serializable]
    [SceneHandleDraw]
    public class BoxRangeObjectInitialization : RangeObjectInitialization
    {
        [DrawNested]
        public BoxRangeTypeDefinition BoxRangeTypeDefinition;
    }
}
