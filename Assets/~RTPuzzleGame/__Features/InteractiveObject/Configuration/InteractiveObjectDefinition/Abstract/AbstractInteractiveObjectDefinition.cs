using UnityEngine;
using System.Collections;
using OdinSerializer;

namespace RTPuzzle
{
    public abstract class AbstractInteractiveObjectDefinition : SerializedScriptableObject
    {
        public abstract void CreateObject(Transform parent);
    }
}
