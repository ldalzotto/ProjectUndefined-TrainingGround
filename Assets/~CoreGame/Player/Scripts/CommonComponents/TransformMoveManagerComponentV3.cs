using UnityEngine;
using UnityEngine.Serialization;


namespace CoreGame
{
    [System.Serializable]
    public class TransformMoveManagerComponentV3
    {
        public float SpeedMultiplicationFactor = 20f;
        public float RotationSpeed = 10f;

        [HideInInspector]
        public bool IsPositionUpdateConstrained = false;

        [Foldable(true, nameof(TransformMoveManagerComponentV3.IsPositionUpdateConstrained))]
        public TransformPositionUpdateConstraints TransformPositionUpdateConstraints;
    }

    [System.Serializable]
    public class TransformPositionUpdateConstraints
    {
        [Range(0f, 360f)]
        public float MinAngleThatAllowThePositionUpdate = 45f;
    }
}
