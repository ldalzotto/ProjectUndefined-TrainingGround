using UnityEngine;
using System.Collections;
using CoreGame;
using OdinSerializer;

namespace InteractiveObjectTest
{
    [System.Serializable]
    [SceneHandleDraw]
    public class SightObjectSystemDefinition : SerializedScriptableObject
    {
        [WireRoundedFrustum(R = 1f, G = 1f, B = 0f)]
        public FrustumV2 Frustum;
    }
}