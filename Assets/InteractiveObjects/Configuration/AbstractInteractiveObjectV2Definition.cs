using OdinSerializer;
using UnityEngine;

namespace InteractiveObjectTest
{
    [SceneHandleDraw]
    public abstract class AbstractInteractiveObjectV2Definition : SerializedScriptableObject
    {
        public abstract CoreInteractiveObject BuildInteractiveObject(GameObject parent);
    }

    [System.Serializable]
    [SceneHandleDraw]
    public class InteractiveObjectLogicCollider
    {
        public bool Enabled = true;
        public bool HasRigidBody = true;

        [WireBox(R = 1, G = 1, B = 0, CenterFieldName = nameof(InteractiveObjectLogicCollider.LocalCenter),
            SizeFieldName = nameof(InteractiveObjectLogicCollider.LocalSize))]
        public Vector3 LocalCenter;
        public Vector3 LocalSize;
    }
}

