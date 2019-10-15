using OdinSerializer;
using UnityEngine;

namespace InteractiveObjectTest
{
    public abstract class AbstractInteractiveObjectV2Definition : SerializedScriptableObject
    {
        public abstract CoreInteractiveObject BuildInteractiveObject(GameObject parent);
    }
}

