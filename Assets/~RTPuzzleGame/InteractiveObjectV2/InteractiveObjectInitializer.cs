using UnityEngine;

namespace InteractiveObjectTest
{
    [System.Serializable]
    public abstract class A_InteractiveObjectInitializer : MonoBehaviour
    {
        protected abstract CoreInteractiveObject GetInteractiveObject();

        public virtual void Init()
        {
            this.GetInteractiveObject();
        }
    }

}
