using OdinSerializer;

namespace InteractiveObjectTest
{
    [System.Serializable]
    public abstract class AbstractAIInteractiveObjectInitializerData : AbstractInteractiveObjectV2Definition
    {
        public float SpeedMultiplicationFactor;
        public float RotationSpeed;
        public float MinAngleThatAllowThePositionUpdate;
    }
}
