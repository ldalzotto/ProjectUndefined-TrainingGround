using UnityEngine;

namespace InteractiveObjectTest
{
    [System.Serializable]
    public abstract class A_InteractiveObjectInitializer : MonoBehaviour
    {
        protected abstract CoreInteractiveObject GetInteractiveObject();
        protected abstract object GetInitializerDataObject();

        public virtual void Init()
        {
            this.GetInteractiveObject();
        }

    }

}
