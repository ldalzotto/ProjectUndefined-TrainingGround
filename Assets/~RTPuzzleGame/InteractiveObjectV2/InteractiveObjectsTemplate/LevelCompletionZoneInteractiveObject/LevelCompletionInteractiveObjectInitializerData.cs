using OdinSerializer;
using UnityEngine;

namespace InteractiveObjectTest
{

    [System.Serializable]
    [SceneHandleDraw]
    [CreateAssetMenu(fileName = "LevelCompletionInteractiveObjectInitializerData", menuName = "Test/LevelCompletionInteractiveObjectInitializerData", order = 1)]
    public class LevelCompletionInteractiveObjectInitializerData : AbstractInteractiveObjectV2Definition
    {
        [DrawNested]
        [Inline(createAtSameLevelIfAbsent: true)]
        public LevelCompletionZoneSystemDefinition LevelCompletionZoneSystemDefinition;

        public override CoreInteractiveObject BuildInteractiveObject(GameObject parent)
        {
            return new LevelCompletionInteractiveObject(this, new InteractiveGameObject(parent));
        }
    }

}
