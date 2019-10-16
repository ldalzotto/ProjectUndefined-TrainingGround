using OdinSerializer;
using RTPuzzle;

namespace InteractiveObjectTest
{
    [System.Serializable]
    [SceneHandleDraw]
    public class LevelCompletionZoneSystemDefinition : SerializedScriptableObject
    {
        [DrawNested]
        [Inline(createAtSameLevelIfAbsent: true)]
        public RangeObjectInitialization TriggerRangeObjectInitialization;
    }
}

