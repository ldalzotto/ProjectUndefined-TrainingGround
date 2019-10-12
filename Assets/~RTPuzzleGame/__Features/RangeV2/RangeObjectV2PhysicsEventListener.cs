using InteractiveObjectTest;
using System;
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
        public virtual void OnTriggerExit(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo) { }

        public virtual bool ColliderSelectionGuard(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo) { return true; }
    }

    public class RangeObjectV2PhysicsEventListener : MonoBehaviour
    {
        private List<ARangeObjectV2PhysicsEventListener> PhysicsEventListeners = new List<ARangeObjectV2PhysicsEventListener>();
        private List<Collider> CurrentlyTrackedColliders = new List<Collider>();

        private CoreInteractiveObject AssociatedInteractiveObject;
        public void Init(CoreInteractiveObject AssociatedInteractiveObject)
        {
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
        }

        public void AddPhysicsEventListener(ARangeObjectV2PhysicsEventListener ARangeObjectV2PhysicsEventListener) { this.PhysicsEventListeners.Add(ARangeObjectV2PhysicsEventListener); }

        public void OnTriggerEnter(Collider other)
        {
            InteractiveObjectV2Manager.Get().InteractiveObjectsIndexedByLogicCollider.TryGetValue(other, out CoreInteractiveObject OtherInteractiveObject);
            if (OtherInteractiveObject != null && ((this.AssociatedInteractiveObject != null && OtherInteractiveObject != this.AssociatedInteractiveObject) || this.AssociatedInteractiveObject == null))
            {
                this.CurrentlyTrackedColliders.Add(other);
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
        public void OnTriggerExit(Collider other)
        {
            InteractiveObjectV2Manager.Get().InteractiveObjectsIndexedByLogicCollider.TryGetValue(other, out CoreInteractiveObject OtherInteractiveObject);
            if (OtherInteractiveObject != null && ((this.AssociatedInteractiveObject != null && OtherInteractiveObject != this.AssociatedInteractiveObject) || this.AssociatedInteractiveObject == null))
            {
                this.CurrentlyTrackedColliders.Remove(other);
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

        public void Destroy()
        {
            for (var i = this.CurrentlyTrackedColliders.Count - 1; i >= 0; i--)
            {
                this.OnTriggerExit(this.CurrentlyTrackedColliders[i]);
            }

            this.PhysicsEventListeners.Clear();
            this.CurrentlyTrackedColliders.Clear();
        }
    }

    public class RangeObjectV2PhysicsEventListener_Delegated : ARangeObjectV2PhysicsEventListener
    {
        private Action<CoreInteractiveObject> OnTriggerEnterAction = null;
        private Action<CoreInteractiveObject> OnTriggerExitAction = null;

        protected InteractiveObjectTagStruct InteractiveObjectSelectionGuard;

        public RangeObjectV2PhysicsEventListener_Delegated(InteractiveObjectTagStruct interactiveObjectSelectionGuard,
            Action<CoreInteractiveObject> onTriggerEnterAction = null, Action<CoreInteractiveObject> onTriggerExitAction = null)
        {
            OnTriggerEnterAction = onTriggerEnterAction;
            OnTriggerExitAction = onTriggerExitAction;
            InteractiveObjectSelectionGuard = interactiveObjectSelectionGuard;
        }

        public override bool ColliderSelectionGuard(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo)
        {
            return this.InteractiveObjectSelectionGuard.Compare(RangeObjectPhysicsTriggerInfo.OtherInteractiveObject.InteractiveObjectTag);
        }

        public override void OnTriggerEnter(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            if (this.OnTriggerEnterAction != null) { this.OnTriggerEnterAction.Invoke(PhysicsTriggerInfo.OtherInteractiveObject); }
        }

        public override void OnTriggerExit(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            if (this.OnTriggerExitAction != null) { this.OnTriggerExitAction.Invoke(PhysicsTriggerInfo.OtherInteractiveObject); }
        }
    }
}
