using RTPuzzle;
using UnityEngine;
using static InteractiveObjectTest.AIMovementDefinitions;

namespace InteractiveObjectTest
{
    [System.Serializable]
    [SceneHandleDraw]
    [CreateAssetMenu(fileName = "AIInteractiveObjectTestInitializerData", menuName = "Test/AIInteractiveObjectTestInitializerData", order = 1)]
    public class AIInteractiveObjectTestInitializerData : AbstractAIInteractiveObjectInitializerData
    {
        [Inline(CreateAtSameLevelIfAbsent = true)]
        public AIPatrolSystemDefinition AIPatrolSystemDefinition;

        [DrawNested]
        [Inline(CreateAtSameLevelIfAbsent = true)]
        public SightObjectSystemDefinition SightObjectSystemDefinition;

        public AIMovementSpeedDefinition AISpeedWhenAttracted;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public LocalPuzzleCutsceneTemplate DisarmObjectAnimation;

        public override CoreInteractiveObject BuildInteractiveObject(GameObject parent)
        {
            return new AIInteractiveObjectTest(new InteractiveGameObject(parent), this);
        }
    }
}
