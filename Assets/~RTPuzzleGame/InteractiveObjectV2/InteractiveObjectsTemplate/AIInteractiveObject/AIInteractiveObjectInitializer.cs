using OdinSerializer;

namespace InteractiveObjectTest
{
    [System.Serializable]
    public abstract class A_AIInteractiveObjectInitializerData : SerializedScriptableObject
    {
        public float SpeedMultiplicationFactor;
        public float RotationSpeed;
        public float MinAngleThatAllowThePositionUpdate;
    }


}
