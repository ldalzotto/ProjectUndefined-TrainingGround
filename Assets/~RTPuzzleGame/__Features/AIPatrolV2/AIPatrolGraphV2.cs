using CoreGame;
using InteractiveObjectTest;
using OdinSerializer;
using System.Collections.Generic;
using UnityEngine;
using static InteractiveObjectTest.AIMovementDefinitions;

namespace RTPuzzle
{
    [System.Serializable]
    public abstract class AIPatrolGraphV2 : SerializedScriptableObject
    {
        public Vector3 RootWorldPosition;
        protected CoreInteractiveObject InvolvedInteractiveObject;

        public void Init(CoreInteractiveObject CoreInteractiveObject)
        {
            this.InvolvedInteractiveObject = CoreInteractiveObject;
        }

        protected TransformStruct TransformToWorldPosition(TransformStruct AIPatrolGraphPosition)
        {
            return new TransformStruct
            {
                WorldPosition = this.RootWorldPosition + AIPatrolGraphPosition.WorldPosition,
                LossyScale = AIPatrolGraphPosition.LossyScale,
                WorldRotation = AIPatrolGraphPosition.WorldRotation
            };
        }
        public abstract List<SequencedAction> AIPatrolGraphActions();

        protected AIMoveToActionV2 CreateAIMoveToActionV2(CoreInteractiveObject InvolvedInteractiveObject, AIMoveToActionInputData AIMoveToActionInputData, List<SequencedAction> nextActions)
        {
            return new AIMoveToActionV2(InvolvedInteractiveObject, this.TransformToWorldPosition(AIMoveToActionInputData.WorldPoint), AIMoveToActionInputData.AIMovementSpeed, nextActions);
        }
    }


    [System.Serializable]
    public struct AIMoveToActionInputData
    {
        public TransformStruct WorldPoint;
        public AIMovementSpeedDefinition AIMovementSpeed;
    }
}
