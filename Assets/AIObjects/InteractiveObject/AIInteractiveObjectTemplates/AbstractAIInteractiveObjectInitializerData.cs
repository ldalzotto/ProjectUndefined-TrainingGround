using System;
using CoreGame;
using InteractiveObjects;
using InteractiveObjects_Interfaces;

namespace AIObjects
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