using InteractiveObjectTest;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public struct RangeObjectPhysicsTriggerInfo
    {
        public CoreInteractiveObject OtherInteractiveObject;
        public Collider Other;
    }

    public abstract class ARangeObjectV2PhysicsEventListener
    {
        ////// FIXED UPDATE ///////
        public virtual void OnTriggerEnter(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo) { }
        public virtual void OnTriggerStay(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo) { }
        public virtual void OnTriggerExit(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo) { }

        public virtual bool ColliderSelectionGuard(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo) { return true; }
    }

    public class RangeObjectV2PhysicsEventListener : MonoBehaviour
    {
        private List<ARangeObjectV2PhysicsEventListener> PhysicsEventListeners = new List<ARangeObjectV2PhysicsEventListener>();

        private CoreInteractiveObject AssociatedInteractiveObject;
        public void Init(CoreInteractiveObject AssociatedInteractiveObject)
        {
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
        }

        public void AddPhysicsEventListener(ARangeObjectV2PhysicsEventListener ARangeObjectV2PhysicsEventListener) { this.PhysicsEventListeners.Add(ARangeObjectV2PhysicsEventListener); }

        private void OnTriggerEnter(Collider other)
        {
            InteractiveObjectV2Manager.Get().InteractiveObjectsIndexedByCollider.TryGetValue(other, out CoreInteractiveObject OtherInteractiveObject);
            if (OtherInteractiveObject != null && ((this.AssociatedInteractiveObject != null && OtherInteractiveObject != this.AssociatedInteractiveObject) || this.AssociatedInteractiveObject == null))
            {
                var RangeObjectPhysicsTriggerInfo = new RangeObjectPhysicsTriggerInfo { Other = other, OtherInteractiveObject = OtherInteractiveObject };
                for (var i = 0; i < this.PhysicsEventListeners.Count; i++)
                {
                    if (this.PhysicsEventListeners[i].ColliderSelectionGuard(RangeObjectPhysicsTriggerInfo))
                    {
                        this.PhysicsEventListeners[i].OnTriggerEnter(RangeObjectPhysicsTriggerInfo);
                    }
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            InteractiveObjectV2Manager.Get().InteractiveObjectsIndexedByCollider.TryGetValue(other, out CoreInteractiveObject OtherInteractiveObject);
            if (OtherInteractiveObject != null && ((this.AssociatedInteractiveObject != null && OtherInteractiveObject != this.AssociatedInteractiveObject) || this.AssociatedInteractiveObject == null))
            {
                var RangeObjectPhysicsTriggerInfo = new RangeObjectPhysicsTriggerInfo { Other = other, OtherInteractiveObject = OtherInteractiveObject };
                for (var i = 0; i < this.PhysicsEventListeners.Count; i++)
                {
                    if (this.PhysicsEventListeners[i].ColliderSelectionGuard(RangeObjectPhysicsTriggerInfo))
                    {
                        this.PhysicsEventListeners[i].OnTriggerExit(RangeObjectPhysicsTriggerInfo);
                    }
                }
            }
        }
        private void OnTriggerStay(Collider other)
        {
            InteractiveObjectV2Manager.Get().InteractiveObjectsIndexedByCollider.TryGetValue(other, out CoreInteractiveObject OtherInteractiveObject);
            if (OtherInteractiveObject != null && ((this.AssociatedInteractiveObject != null && OtherInteractiveObject != this.AssociatedInteractiveObject) || this.AssociatedInteractiveObject == null))
            {
                var RangeObjectPhysicsTriggerInfo = new RangeObjectPhysicsTriggerInfo { Other = other, OtherInteractiveObject = OtherInteractiveObject };
                for (var i = 0; i < this.PhysicsEventListeners.Count; i++)
                {
                    if (this.PhysicsEventListeners[i].ColliderSelectionGuard(RangeObjectPhysicsTriggerInfo))
                    {
                        this.PhysicsEventListeners[i].OnTriggerStay(RangeObjectPhysicsTriggerInfo);
                    }
                }
            }
        }
    }

}
