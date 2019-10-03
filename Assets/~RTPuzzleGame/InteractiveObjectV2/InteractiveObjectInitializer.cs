using UnityEngine;

namespace InteractiveObjectTest
{
    [System.Serializable]
    public class InteractiveObjectInitializerData
    {
        public AttractiveObjectSystemDefinition AttractiveObjectSystemDefinition;
    }

    [System.Serializable]
    public class InteractiveObjectInitializer : MonoBehaviour
    {

        public InteractiveObjectInitializerData InteractiveObjectInitializerData;
        public void Init()
        {
            InteractiveObjectV2Manager.Get().OnInteractiveObjectCreated(new AttractiveInteractiveObject(new InteractiveGameObject(this.gameObject), InteractiveObjectInitializerData));
        }
    }
}
