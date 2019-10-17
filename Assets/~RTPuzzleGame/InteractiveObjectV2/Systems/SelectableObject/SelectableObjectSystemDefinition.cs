using UnityEngine;
using System.Collections;
using OdinSerializer;

namespace InteractiveObjects
{
    [System.Serializable]
    [SceneHandleDraw]
    public class SelectableObjectSystemDefinition : SerializedScriptableObject
    {
        [WireCircle(R = 0f, G = 0f, B = 1f)]
        public float SelectionRange;
    }
}

