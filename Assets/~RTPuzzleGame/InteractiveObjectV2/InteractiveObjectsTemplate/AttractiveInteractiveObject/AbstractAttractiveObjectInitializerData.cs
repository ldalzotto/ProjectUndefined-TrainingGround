﻿using RTPuzzle;

namespace InteractiveObjects
{
    [System.Serializable]
    [SceneHandleDraw]
    public abstract class AbstractAttractiveObjectInitializerData : AbstractInteractiveObjectV2Definition
    {
        [DrawNested]
        public InteractiveObjectLogicCollider InteractiveObjectLogicCollider;

        [DrawNested]
        [Inline(createAtSameLevelIfAbsent: true)]
        public AttractiveObjectSystemDefinition AttractiveObjectSystemDefinition;
    }

}