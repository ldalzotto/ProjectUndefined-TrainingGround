using OdinSerializer;

namespace InteractiveObjects
{
    [System.Serializable]
    [SceneHandleDraw]
    public abstract class AbstractAIInteractiveObjectInitializerData : AbstractInteractiveObjectV2Definition
    {
        public float SpeedMultiplicationFactor = 20f;
        public float RotationSpeed = 5f;
        public float MinAngleThatAllowThePositionUpdate = 45f;
        [DrawNested]
        public AIAgentDefinition AIAgentDefinition;
        [DrawNested]
        public InteractiveObjectLogicCollider InteractiveObjectLogicCollider;
    }
}
