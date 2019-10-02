using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "BoxRangeObjectInitialization", menuName = "Test/BoxRangeObjectInitialization", order = 1)]
    public class BoxRangeObjectInitialization : RangeObjectInitialization
    {
        public BoxRangeTypeDefinition BoxRangeTypeDefinition;
    }
}
