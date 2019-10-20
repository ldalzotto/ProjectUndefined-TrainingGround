using System;
using CoreGame;
using InteractiveObjects_Interfaces;

namespace InteractiveObjects
{
    [Serializable]
    [SceneHandleDraw]
    public abstract class AbstractAIInteractiveObjectInitializerData : AbstractInteractiveObjectV2Definition
    {
        [DrawNested] public AIAgentDefinition AIAgentDefinition;
        [DrawNested] public InteractiveObjectLogicColliderDefinition InteractiveObjectLogicCollider;
        public TransformMoveManagerComponentV3 TransformMoveManagerComponentV3;
    }
}