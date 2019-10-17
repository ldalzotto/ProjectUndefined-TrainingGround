using RTPuzzle;
using UnityEngine;

namespace InteractiveObjects
{
    [System.Serializable]
    [SceneHandleDraw]
    [CreateAssetMenu(fileName = "TestAttractiveObjectInitializerData", menuName = "Test/TestAttractiveObjectInitializerData", order = 1)]
    public class TestAttractiveObjectInitializerData : AbstractAttractiveObjectInitializerData
    {
        [Inline(createAtSameLevelIfAbsent: true)]
        [DrawNested]
        public DisarmSystemDefinition DisarmSystemDefinition;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        [DrawNested]
        public SelectableObjectSystemDefinition SelectableObjectSystemDefinition;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public GrabObjectActionInherentData SelectableGrabActionDefinition;

        public override CoreInteractiveObject BuildInteractiveObject(GameObject parent)
        {
            return new TestAttractiveObject(InteractiveGameObjectFactory.Build(parent), this);
        }
    }

}
