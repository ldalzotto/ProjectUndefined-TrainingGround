using UnityEngine;
using System.Collections;

namespace InteractiveObjectTest
{
    [SceneHandleDraw]
    [System.Serializable]
    public class AIInteractiveObjectTestInitializer : A_InteractiveObjectInitializer
    {
        [Inline()]
        [DrawNested]
        public AIInteractiveObjectTestInitializerData AIInteractiveObjectTestInitializerData;

        protected override object GetInitializerDataObject()
        {
            return this.AIInteractiveObjectTestInitializerData;
        }

        protected override CoreInteractiveObject GetInteractiveObject()
        {
            return new AIInteractiveObjectTest(new InteractiveGameObject(this.gameObject), AIInteractiveObjectTestInitializerData);
        }
    }
}

