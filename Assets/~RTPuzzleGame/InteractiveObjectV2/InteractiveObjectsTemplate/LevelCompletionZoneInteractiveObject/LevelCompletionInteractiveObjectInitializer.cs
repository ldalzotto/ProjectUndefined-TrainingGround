using UnityEngine;
using System.Collections;

namespace InteractiveObjectTest
{

    [System.Serializable]
    [SceneHandleDraw]
    public class LevelCompletionInitializerData
    {
        [DrawNested]
        public LevelCompletionZoneSystemDefinition LevelCompletionZoneSystemDefinition;
    }

    [SceneHandleDraw]
    public class LevelCompletionInteractiveObjectInitializer : A_InteractiveObjectInitializer
    {
        [DrawNested]
        public LevelCompletionInitializerData LevelCompletionInitializerData;

        protected override object GetInitializerDataObject()
        {
            return this.LevelCompletionInitializerData;
        }

        protected override CoreInteractiveObject GetInteractiveObject()
        {
            return new LevelCompletionInteractiveObject(this.LevelCompletionInitializerData, new InteractiveGameObject(this.gameObject));
        }
    }

}
