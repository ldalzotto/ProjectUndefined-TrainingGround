using UnityEngine;
using UnityEngine.Profiling;

namespace InteractiveObjectTest
{
    public abstract class CoreInteractiveObject
    {
        public InteractiveGameObject InteractiveGameObject { get; protected set; }

        public InteractiveObjectTag InteractiveObjectTag { get; protected set; }
        
        public bool IsAskingToBeDestroyed { get; protected set; }

        public CoreInteractiveObject(InteractiveGameObject interactiveGameObject)
        {
            this.IsAskingToBeDestroyed = false;
            InteractiveGameObject = interactiveGameObject;
        }

        public virtual void Tick(float d, float timeAttenuationFactor)
        {
        }

        public virtual void TickAlways(float d)
        {
        }
        
        public virtual void Destroy()
        {
            Object.Destroy(this.InteractiveGameObject.InteractiveGameObjectParent);
        }
    }
    
    public class InteractiveObjectTag
    {
        public bool IsAttractiveObject;
    }

    [System.Serializable]
    public struct InteractiveObjectTagStruct
    {
        public bool IsAttractiveObject;
        public bool Equals(InteractiveObjectTag InteractiveObjectState)
        {
            return this.IsAttractiveObject == InteractiveObjectState.IsAttractiveObject;
        }
    }
}
