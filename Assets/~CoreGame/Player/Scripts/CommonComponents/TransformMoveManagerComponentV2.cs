using UnityEngine.Serialization;

namespace CoreGame
{
    [System.Serializable]
    public class TransformMoveManagerComponentV2 : ADataComponent
    {
        public float SpeedMultiplicationFactor = 20f;
        [FormerlySerializedAs("AIRotationSpeed")]
        public float RotationSpeed = 10f;
    }
}
