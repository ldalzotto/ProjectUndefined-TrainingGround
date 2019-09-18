using GameConfigurationID;
using OdinSerializer;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    public class RangeTypeDefinition : SerializedScriptableObject
    {
        [CustomEnum()]
        public RangeTypeID RangeTypeID;

        [Inline]
        [SerializeField]
        public RangeShapeConfiguration RangeShapeConfiguration;
    }


}
