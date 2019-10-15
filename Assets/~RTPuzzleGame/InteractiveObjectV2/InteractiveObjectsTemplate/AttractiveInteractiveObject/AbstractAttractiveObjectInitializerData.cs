using RTPuzzle;

namespace InteractiveObjectTest
{
    [System.Serializable]
    [SceneHandleDraw]
    public abstract class AbstractAttractiveObjectInitializerData : AbstractInteractiveObjectV2Definition
    {
        [DrawNested]
        [Inline(createAtSameLevelIfAbsent: true)]
        public AttractiveObjectSystemDefinition AttractiveObjectSystemDefinition;
    }

}
