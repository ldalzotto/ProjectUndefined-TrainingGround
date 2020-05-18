﻿using System;
using System.Collections.Generic;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using OdinSerializer;
using SequencedAction;
using UnityEngine;

namespace AIObjects
{
    [Serializable]
    public abstract class AIPatrolGraphV2 : SerializedScriptableObject
    {
        public Vector3 RootWorldPosition;

        protected TransformStruct TransformToWorldPosition(TransformStruct AIPatrolGraphPosition)
        {
            return new TransformStruct
            {
                WorldPosition = this.RootWorldPosition + AIPatrolGraphPosition.WorldPosition,
                LossyScale = AIPatrolGraphPosition.LossyScale,
                WorldRotation = AIPatrolGraphPosition.WorldRotation
            };
        }

        public abstract List<ASequencedAction> AIPatrolGraphActions(CoreInteractiveObject InvolvedInteractiveObject);

        protected AIMoveToActionV2 CreateAIMoveToActionV2(CoreInteractiveObject InvolvedInteractiveObject, AIMoveToActionInputData AIMoveToActionInputData, Func<List<ASequencedAction>> nextActionsDeffered)
        {
            return new AIMoveToActionV2(InvolvedInteractiveObject, this.TransformToWorldPosition(AIMoveToActionInputData.WorldPoint), AIMoveToActionInputData.AIMovementSpeed, nextActionsDeffered);
        }
    }


    [Serializable]
    public struct AIMoveToActionInputData
    {
        public TransformStruct WorldPoint;
        public AIMovementSpeedDefinition AIMovementSpeed;

        public Vector3 GetWorldPosition()
        {
            return this.WorldPoint.WorldPosition;
        }
    }
}