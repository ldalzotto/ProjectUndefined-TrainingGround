using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    [SceneHandleDraw]
    [System.Serializable]
    public class SphereRangeObjectInitialization : RangeObjectInitialization
    {
        [DrawNested]
        public SphereRangeTypeDefinition SphereRangeTypeDefinition;
    }
}
