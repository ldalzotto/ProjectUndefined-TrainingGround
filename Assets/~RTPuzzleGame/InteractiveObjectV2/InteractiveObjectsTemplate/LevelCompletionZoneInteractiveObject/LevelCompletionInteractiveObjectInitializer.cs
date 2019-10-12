using UnityEngine;
using System.Collections;

namespace InteractiveObjectTest
{

    [System.Serializable]
    public class LevelCompletionInitializerData
    {
        public LevelCompletionZoneSystemDefinition LevelCompletionZoneSystemDefinition;
    }

    public class LevelCompletionInteractiveObjectInitializer : A_InteractiveObjectInitializer
    {
        public LevelCompletionInitializerData LevelCompletionInitializerData;
        protected override CoreInteractiveObject GetInteractiveObject()
        {
            return new LevelCompletionInteractiveObject(this.LevelCompletionInitializerData, new InteractiveGameObject(this.gameObject));
        }
    }

}
