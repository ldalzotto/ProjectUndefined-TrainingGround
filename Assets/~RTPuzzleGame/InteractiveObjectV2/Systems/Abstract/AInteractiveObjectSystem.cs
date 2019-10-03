using UnityEngine;
using System.Collections;
namespace InteractiveObjectTest
{
    public abstract class AInteractiveObjectSystem
    {
        public virtual void Tick(float d, float timeAttenuationFactor) { }
        public virtual void TickWhenTimeIsStopped() { }
        public virtual void OnDestroy() { }
    }

}
