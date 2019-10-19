using System;
using CoreGame;

namespace InteractiveObjects
{
    [Serializable]
    [SceneHandleDraw]
    public abstract class AbstractAIInteractiveObjectInitializerData : AbstractInteractiveObjectV2Definition
    {
        [DrawNested] public AIAgentDefinition AIAgentDefinition;
        [DrawNested] public InteractiveObjectLogicCollider InteractiveObjectLogicCollider;
        public TransformMoveManagerComponentV3 TransformMoveManagerComponentV3;
    }
}