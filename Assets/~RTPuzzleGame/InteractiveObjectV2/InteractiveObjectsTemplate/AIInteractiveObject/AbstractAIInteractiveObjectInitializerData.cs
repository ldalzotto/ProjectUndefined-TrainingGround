using OdinSerializer;

namespace InteractiveObjectTest
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

    [System.Serializable]
    [SceneHandleDraw]
    public class AIAgentDefinition
    {
        [WireCircle(R = 0f, G = 1f, B = 0f)]
        public float AgentStoppingDistance = 0.5f;
        [WireDirectionalLineAttribute(R = 0f, G = 1f, B = 0f, dY = 1f)]
        public float AgentHeight = 2f;
    }
}
