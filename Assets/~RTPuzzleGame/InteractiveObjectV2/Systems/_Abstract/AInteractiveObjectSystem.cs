using UnityEngine;
using System.Collections;
namespace InteractiveObjects
{
    public abstract class AInteractiveObjectSystem
    {
        public virtual void TickAlways(float d) { }
        public virtual void Tick(float d, float timeAttenuationFactor) { }
        public virtual void TickWhenTimeIsStopped() { }
        public virtual void AfterTicks() { }
        public virtual void OnDestroy() { }
    }

}
