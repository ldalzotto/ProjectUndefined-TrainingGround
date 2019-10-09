using UnityEngine;

namespace InteractiveObjectTest
{
    [System.Serializable]
    public abstract class A_InteractiveObjectInitializer : MonoBehaviour
    {
        protected abstract CoreInteractiveObject GetInteractiveObject();

        public void Init()
        {
            InteractiveObjectV2Manager.Get().OnInteractiveObjectCreated(this.GetInteractiveObject());
        }
    }

}
