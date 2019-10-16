using UnityEngine;
using System.Collections;
using InteractiveObjectTest;

namespace InteractiveObjectTest
{
    [System.Serializable]
    [SceneHandleDraw]
    [CreateAssetMenu(fileName = "AggressiveObjectInitializerData", menuName = "Test/AggressiveObjectInitializerData", order = 1)]
    public class AggressiveObjectInitializerData : AbstractAIInteractiveObjectInitializerData
    {
        public AIPatrolSystemDefinition AIPatrolSystemDefinition;

        [Inline(createAtSameLevelIfAbsent: true)]
        public SightObjectSystemDefinition SightObjectSystemDefinition;

        public override CoreInteractiveObject BuildInteractiveObject(GameObject parent)
        {
            return new AggressiveAIObject(new InteractiveGameObject(parent), this);
        }
    }
}

