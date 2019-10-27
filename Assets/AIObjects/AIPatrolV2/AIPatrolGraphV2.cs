using System;
using System.Collections.Generic;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using OdinSerializer;
using SequencedAction;
using UnityEngine;
using UnityEngine.Serialization;

namespace AIObjects
{
    [Serializable]
    public abstract class AIPatrolGraphV2 : SerializedScriptableObject
    {
        public abstract List<ASequencedAction> AIPatrolGraphActions(CoreInteractiveObject InvolvedInteractiveObject);

        protected AIMoveToActionV2 CreateAIMoveToActionV2(CoreInteractiveObject InvolvedInteractiveObject, AIMoveToActionInputData AIMoveToActionInputData, Func<List<ASequencedAction>> nextActionsDeffered)
        {
            return new AIMoveToActionV2(InvolvedInteractiveObject, AIMoveToActionInputData.WorldPoint, AIMoveToActionInputData.AIMovementSpeed, nextActionsDeffered);
        }
    }

    [Serializable]
    public class AIMoveToActionInputData
    {
        public TransformStruct WorldPoint;
        public AIMovementSpeedDefinition AIMovementSpeed;

        public Vector3 GetWorldPosition()
        {
            return this.WorldPoint.WorldPosition;
        }
    }
}