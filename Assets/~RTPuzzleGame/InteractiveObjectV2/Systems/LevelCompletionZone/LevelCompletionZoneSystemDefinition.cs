using OdinSerializer;
using RTPuzzle;

namespace InteractiveObjectTest
{
    [System.Serializable]
    [SceneHandleDraw]
    public class LevelCompletionZoneSystemDefinition : SerializedScriptableObject
    {
        [DrawNested]
        [Inline()]
        public RangeObjectInitialization TriggerRangeObjectInitialization;
    }
}

