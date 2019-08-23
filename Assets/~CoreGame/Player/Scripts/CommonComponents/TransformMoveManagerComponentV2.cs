using System;
using UnityEngine;
using UnityEngine.Serialization;


namespace CoreGame
{
    [Obsolete("Move to V3 with scriptable object definition.")]
    [System.Serializable]
    public class TransformMoveManagerComponentV2 : ADataComponent
    {
        public float SpeedMultiplicationFactor = 20f;
        public float RotationSpeed = 10f;

        public bool IsPositionUpdateConstrained = false;

        [Range(0f, 360f)]
        public float MinAngleThatAllowThePositionUpdate = 45f;

        public TransformMoveManagerComponentV3 ToV3()
        {
            var TransformMoveManagerComponentV3 = new TransformMoveManagerComponentV3();
            TransformMoveManagerComponentV3.SpeedMultiplicationFactor = this.SpeedMultiplicationFactor;
            TransformMoveManagerComponentV3.RotationSpeed = this.RotationSpeed;
            TransformMoveManagerComponentV3.IsPositionUpdateConstrained = this.IsPositionUpdateConstrained;
            TransformMoveManagerComponentV3.TransformPositionUpdateConstraints = new TransformPositionUpdateConstraints();
            TransformMoveManagerComponentV3.TransformPositionUpdateConstraints.MinAngleThatAllowThePositionUpdate = this.MinAngleThatAllowThePositionUpdate;
            return TransformMoveManagerComponentV3;
        }
    }
}
