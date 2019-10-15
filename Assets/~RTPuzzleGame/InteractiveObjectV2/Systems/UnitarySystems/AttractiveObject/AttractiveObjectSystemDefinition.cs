using UnityEngine;
using System.Collections;
using OdinSerializer;

namespace InteractiveObjectTest
{
    [System.Serializable]
    [SceneHandleDraw]
    public class AttractiveObjectSystemDefinition : SerializedScriptableObject
    {
        [WireCircle(R = 1, G = 1, B = 0)]
        public float EffectRange;

        public float EffectiveTime;
    }
}
