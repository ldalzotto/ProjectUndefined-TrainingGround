using CoreGame;
using InteractiveObjectTest;
using OdinSerializer;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    public abstract class AIPatrolGraphV2 : SerializedScriptableObject
    {
        public Vector3 RootWorldPosition;
        protected CoreInteractiveObject CoreInteractiveObject;

        public void Init(CoreInteractiveObject CoreInteractiveObject)
        {
            this.CoreInteractiveObject = CoreInteractiveObject;
        }

        public TransformStruct TrasnformToWorldPosition(TransformStruct AIPatrolGraphPosition)
        {
            return new TransformStruct
            {
                WorldPosition = this.RootWorldPosition + AIPatrolGraphPosition.WorldPosition,
                LossyScale = AIPatrolGraphPosition.LossyScale,
                WorldRotation = AIPatrolGraphPosition.WorldRotation
            };
        }
        public abstract List<SequencedAction> AIPatrolGraphActions();
    }
}
