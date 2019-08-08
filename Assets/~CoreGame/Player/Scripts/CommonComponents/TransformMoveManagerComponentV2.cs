using UnityEngine;
using UnityEngine.Serialization;


namespace CoreGame
{
    [System.Serializable]
    public class TransformMoveManagerComponentV2 : ADataComponent
    {
        public float SpeedMultiplicationFactor = 20f;
        public float RotationSpeed = 10f;

        public bool IsPositionUpdateConstrained = false;

        [Range(0f, 360f)]
        public float MinAngleThatAllowThePositionUpdate = 45f;
    }
}
