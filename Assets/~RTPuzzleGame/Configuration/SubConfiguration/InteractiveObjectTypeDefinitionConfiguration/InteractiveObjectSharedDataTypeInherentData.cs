using UnityEngine;
using System.Collections;
using OdinSerializer;
using CoreGame;

namespace RTPuzzle
{
    [System.Serializable]
    public class InteractiveObjectSharedDataTypeInherentData : SerializedScriptableObject
    {
        [Foldable()]
        public TransformMoveManagerComponentV3 TransformMoveManagerComponent;

        public void DefineInteractiveObjectSharedDataType(InteractiveObjectType interactiveObjectType)
        {
            var InteractiveObjectSharedDataType = interactiveObjectType.gameObject.AddComponent<InteractiveObjectSharedDataType>();
            InteractiveObjectSharedDataType.InteractiveObjectSharedDataTypeInherentData = this;
        }
    }
}

