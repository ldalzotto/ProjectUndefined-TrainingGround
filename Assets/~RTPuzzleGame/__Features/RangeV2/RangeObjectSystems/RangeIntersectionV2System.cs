using RTPuzzle;
using System;
using System.Collections.Generic;

public class RangeIntersectionV2System : ARangeObjectSystem
{
    private List<ARangeIntersectionV2Listener> RangeIntersectionListeners = null;
    
    public RangeIntersectionV2System(RangeObjectV2 rangeObjectV2Ref) : base(rangeObjectV2Ref) { }

    public override void Tick(float d)
    {
        if (this.RangeIntersectionListeners != null)
        {
            for (var RangeIntersectionListenerIndex = 0; RangeIntersectionListenerIndex < this.RangeIntersectionListeners.Count; RangeIntersectionListenerIndex++)
            {
                this.RangeIntersectionListeners[RangeIntersectionListenerIndex].Tick();
            }
        }
    }

    public void ReceiveEvent(RangeIntersectionAddIntersectionListenerEvent RangeIntersectionAddIntersectionListenerEvent, RangeObjectV2PhysicsEventListener RangeObjectV2PhysicsEventListener)
    {
        if (this.RangeIntersectionListeners == null) { this.RangeIntersectionListeners = new List<ARangeIntersectionV2Listener>(); }
        RangeObjectV2PhysicsEventListener.AddPhysicsEventListener(RangeIntersectionAddIntersectionListenerEvent.ARangeIntersectionV2Listener);
        this.RangeIntersectionListeners.Add(RangeIntersectionAddIntersectionListenerEvent.ARangeIntersectionV2Listener);
    }

    public void OnDestroy()
    {
        if (this.RangeIntersectionListeners != null)
        {
            for (var RangeIntersectionListenerIndex = 0; RangeIntersectionListenerIndex < this.RangeIntersectionListeners.Count; RangeIntersectionListenerIndex++)
            {
                this.RangeIntersectionListeners[RangeIntersectionListenerIndex].OnDestroy();
            }
        }
    }
}

public struct RangeIntersectionAddIntersectionListenerEvent
{
    public ARangeIntersectionV2Listener ARangeIntersectionV2Listener;
}

public abstract class ARangeIntersectionV2Listener : ARangeObjectV2PhysicsEventListener
{
    private List<RangeIntersectionCalculatorV2> intersectionCalculators = new List<RangeIntersectionCalculatorV2>();
    private Dictionary<CollisionType, RangeIntersectionCalculatorV2> intersectionCalculatorsIndexedByCollisionType = new Dictionary<CollisionType, RangeIntersectionCalculatorV2>();

    protected virtual bool ColliderSelectionGuard(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo) { return true; }

    protected virtual void OnJustIntersected(RangeIntersectionCalculatorV2 intersectionCalculator) { }
    protected virtual void OnJustNotIntersected(RangeIntersectionCalculatorV2 intersectionCalculator) { }
    protected virtual void OnInterestedNothing(RangeIntersectionCalculatorV2 intersectionCalculator) { }

    protected virtual void OnTriggerEnterSuccess(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo) { }
    protected virtual void OnTriggerExitSuccess(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo) { }

    public abstract void OnDestroy();

    protected RangeObjectV2 associatedRangeObject;

    protected ARangeIntersectionV2Listener(RangeObjectV2 associatedRangeObject)
    {
        this.associatedRangeObject = associatedRangeObject;
    }

    public void Tick()
    {
        for (var intersectionCalculatorIndex = 0; intersectionCalculatorIndex < this.intersectionCalculators.Count; intersectionCalculatorIndex++)
        {
            this.SingleCalculation(this.intersectionCalculators[intersectionCalculatorIndex]);
        }
    }

    public sealed override void OnTriggerEnter(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
    {
        if (this.ColliderSelectionGuard(PhysicsTriggerInfo))
        {
            var rangeIntersectionCalculator = new RangeIntersectionCalculatorV2(this.associatedRangeObject, PhysicsTriggerInfo.OtherCollisionType, forceObstacleOcclusionIfNecessary: true);
            this.intersectionCalculators.Add(rangeIntersectionCalculator);
            this.intersectionCalculatorsIndexedByCollisionType[rangeIntersectionCalculator.TrackedCollider] = rangeIntersectionCalculator;
            this.OnTriggerEnterSuccess(PhysicsTriggerInfo);
        }
    }

    public sealed override void OnTriggerExit(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
    {
        if (this.ColliderSelectionGuard(PhysicsTriggerInfo))
        {
            var rangeIntersectionCalculator = this.intersectionCalculatorsIndexedByCollisionType[PhysicsTriggerInfo.OtherCollisionType];
            this.intersectionCalculators.Remove(rangeIntersectionCalculator);
            this.intersectionCalculatorsIndexedByCollisionType.Remove(PhysicsTriggerInfo.OtherCollisionType);
            this.OnTriggerExitSuccess(PhysicsTriggerInfo);
        }
    }

    public sealed override void OnTriggerStay(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo) { }

    private void SingleCalculation(RangeIntersectionCalculatorV2 intersectionCalculator)
    {
        var intersectionOperation = intersectionCalculator.Tick();
        if (intersectionOperation == InterserctionOperationType.JustInteresected)
        {
            this.OnJustIntersected(intersectionCalculator);
        }
        else if (intersectionOperation == InterserctionOperationType.JustNotInteresected)
        {
            this.OnJustNotIntersected(intersectionCalculator);
        }
        else if (intersectionOperation == InterserctionOperationType.IntersectedNothing)
        {
            this.OnInterestedNothing(intersectionCalculator);
        }
    }
}
