using RTPuzzle;
using System.Collections.Generic;
using UnityEngine;

public struct RangeObjectPhysicsTriggerInfo
{
    public CollisionType OtherCollisionType;
    public Collider Other;
}

public abstract class ARangeObjectV2PhysicsEventListener
{
    ////// FIXED UPDATE ///////
    public virtual void OnTriggerEnter(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo) { }
    public virtual void OnTriggerStay(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo) { }
    public virtual void OnTriggerExit(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo) { }
}

public class RangeObjectV2PhysicsEventListener : MonoBehaviour
{
    private List<ARangeObjectV2PhysicsEventListener> PhysicsEventListeners = new List<ARangeObjectV2PhysicsEventListener>();

    public void AddPhysicsEventListener(ARangeObjectV2PhysicsEventListener ARangeObjectV2PhysicsEventListener) { this.PhysicsEventListeners.Add(ARangeObjectV2PhysicsEventListener); }

    private void OnTriggerEnter(Collider other)
    {
        var collisionType = other.GetComponent<CollisionType>();
        if (collisionType != null)
        {
            for (var i = 0; i < this.PhysicsEventListeners.Count; i++)
            {
                this.PhysicsEventListeners[i].OnTriggerEnter(new RangeObjectPhysicsTriggerInfo { Other = other, OtherCollisionType = collisionType });
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var collisionType = other.GetComponent<CollisionType>();
        if (collisionType != null)
        {
            for (var i = 0; i < this.PhysicsEventListeners.Count; i++)
            {
                this.PhysicsEventListeners[i].OnTriggerExit(new RangeObjectPhysicsTriggerInfo { Other = other, OtherCollisionType = collisionType });
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        var collisionType = other.GetComponent<CollisionType>();
        if (collisionType != null)
        {
            for (var i = 0; i < this.PhysicsEventListeners.Count; i++)
            {
                this.PhysicsEventListeners[i].OnTriggerStay(new RangeObjectPhysicsTriggerInfo { Other = other, OtherCollisionType = collisionType });
            }
        }
    }
}
