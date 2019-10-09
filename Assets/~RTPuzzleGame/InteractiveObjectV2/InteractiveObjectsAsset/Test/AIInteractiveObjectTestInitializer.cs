using UnityEngine;
using System.Collections;

namespace InteractiveObjectTest
{
    [System.Serializable]
    public class AIInteractiveObjectTestInitializer : A_InteractiveObjectInitializer
    {
        public AIInteractiveObjectTestInitializerData AIInteractiveObjectTestInitializerData;

        protected override CoreInteractiveObject GetInteractiveObject()
        {
            return new AIInteractiveObjectTest(new InteractiveGameObject(this.gameObject), AIInteractiveObjectTestInitializerData);
        }
    }
}

