using OdinSerializer;

namespace InteractiveObjectTest
{
    [System.Serializable]
    public abstract class AbstractAIInteractiveObjectInitializerData : SerializedScriptableObject
    {
        public float SpeedMultiplicationFactor;
        public float RotationSpeed;
        public float MinAngleThatAllowThePositionUpdate;
    }


}
