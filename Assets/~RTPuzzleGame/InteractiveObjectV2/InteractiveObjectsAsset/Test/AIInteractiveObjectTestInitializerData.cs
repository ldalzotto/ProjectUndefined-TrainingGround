﻿using RTPuzzle;
using UnityEngine;
using static InteractiveObjectTest.AIMovementDefinitions;

namespace InteractiveObjectTest
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AIInteractiveObjectTestInitializerData", menuName = "Test/AIInteractiveObjectTestInitializerData", order = 1)]
    public class AIInteractiveObjectTestInitializerData : AbstractAIInteractiveObjectInitializerData
    {
        [Inline(CreateAtSameLevelIfAbsent = true)]
        public AIPatrolSystemDefinition AIPatrolSystemDefinition;
        [Inline(CreateAtSameLevelIfAbsent = true)]
        public SightObjectSystemDefinition SightObjectSystemDefinition;
        public AIMovementSpeedDefinition AISpeedWhenAttracted;
        public LocalPuzzleCutsceneTemplate DisarmObjectAnimation;
    }
}
