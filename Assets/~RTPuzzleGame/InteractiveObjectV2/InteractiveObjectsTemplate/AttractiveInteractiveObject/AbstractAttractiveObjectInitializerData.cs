using System;
using InteractiveObjects_Interfaces;

namespace InteractiveObjects
{
    [Serializable]
    [SceneHandleDraw]
    public abstract class AbstractAttractiveObjectInitializerData : AbstractInteractiveObjectV2Definition
    {
        [DrawNested] public InteractiveObjectLogicColliderDefinition InteractiveObjectLogicCollider;

        [DrawNested] [Inline(createAtSameLevelIfAbsent: true)]
        public AttractiveObjectSystemDefinition AttractiveObjectSystemDefinition;
    }
}