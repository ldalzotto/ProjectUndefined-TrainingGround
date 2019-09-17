using UnityEngine;

namespace RTPuzzle
{
    public abstract class InteractiveObjectModule : MonoBehaviour
    {
        public virtual void OnInteractiveObjectDestroyed() { }

        public virtual void OnModuleDisabled() { }

        public abstract void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval,
            IInteractiveObjectTypeEvents IInteractiveObjectTypeEvents);
    }
    
}
