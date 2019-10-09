using UnityEngine;
using System.Collections;
using CoreGame;
using OdinSerializer;

namespace InteractiveObjectTest
{
    [System.Serializable]
    public class SightObjectSystemDefinition : SerializedScriptableObject
    {
        public FrustumV2 Frustum;
    }
}