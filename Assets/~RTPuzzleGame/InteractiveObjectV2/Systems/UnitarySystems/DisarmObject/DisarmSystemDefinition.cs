using UnityEngine;
using System.Collections;
using OdinSerializer;

namespace InteractiveObjectTest
{
    [System.Serializable]
    [SceneHandleDraw]
    public class DisarmSystemDefinition : SerializedScriptableObject
    {
        [WireCircle(R = 0f, G = 0f, B = 1f)]
        public float DisarmRange;
        public float DisarmTime;
    }
}
