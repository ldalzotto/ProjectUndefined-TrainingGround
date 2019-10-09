﻿using InteractiveObjectTest;
using System.Collections.Generic;

namespace RTPuzzle
{
    public class RangeIntersectionV2System : ARangeObjectSystem
    {
        public List<ARangeIntersectionV2Listener> RangeIntersectionListeners { get; private set; } = null;

        public RangeIntersectionV2System(RangeObjectV2 rangeObjectV2Ref) : base(rangeObjectV2Ref)
        { }

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
        protected List<RangeIntersectionCalculatorV2> intersectionCalculators = new List<RangeIntersectionCalculatorV2>();
        private Dictionary<CoreInteractiveObject, RangeIntersectionCalculatorV2> intersectionCalculatorsIndexedByTrackedInteractiveObject = new Dictionary<CoreInteractiveObject, RangeIntersectionCalculatorV2>();

        private List<RangeIntersectionCalculatorV2> justTriggerExitedCalculators = new List<RangeIntersectionCalculatorV2>();
        private Dictionary<CoreInteractiveObject, RangeIntersectionCalculatorV2> justTriggerExitedCalculatorsIndexedByTrackedInteractiveObject = new Dictionary<CoreInteractiveObject, RangeIntersectionCalculatorV2>();

        protected virtual void OnJustIntersected(RangeIntersectionCalculatorV2 intersectionCalculator) { }
        protected virtual void OnJustNotIntersected(RangeIntersectionCalculatorV2 intersectionCalculator) { }
        protected virtual void OnInterestedNothing(RangeIntersectionCalculatorV2 intersectionCalculator) { }

        protected virtual void OnTriggerEnterSuccess(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo) { }
        protected virtual void OnTriggerExitSuccess(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo) { }

        public virtual void OnDestroy() { }

        protected RangeObjectV2 associatedRangeObject;

        protected ARangeIntersectionV2Listener(RangeObjectV2 associatedRangeObject)
        {
            this.associatedRangeObject = associatedRangeObject;
        }

        public void Tick()
        {
            foreach (var intersectionCalculator in this.intersectionCalculators)
            {
                this.SingleCalculation(intersectionCalculator);
            }

            for (var i = this.justTriggerExitedCalculators.Count - 1; i >= 0; i--)
            {
                this.SingleJustExited(this.justTriggerExitedCalculators[i]);
            }
        }

        public sealed override void OnTriggerEnter(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            var rangeIntersectionCalculator = new RangeIntersectionCalculatorV2(this.associatedRangeObject, PhysicsTriggerInfo.OtherInteractiveObject);
            this.intersectionCalculators.Add(rangeIntersectionCalculator);
            this.intersectionCalculatorsIndexedByTrackedInteractiveObject[PhysicsTriggerInfo.OtherInteractiveObject] = rangeIntersectionCalculator;
            this.OnTriggerEnterSuccess(PhysicsTriggerInfo);
        }

        public sealed override void OnTriggerExit(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            var rangeIntersectionCalculator = this.intersectionCalculatorsIndexedByTrackedInteractiveObject[PhysicsTriggerInfo.OtherInteractiveObject];

            if (rangeIntersectionCalculator.IsInside)
            {
                this.justTriggerExitedCalculators.Add(rangeIntersectionCalculator);
                this.justTriggerExitedCalculatorsIndexedByTrackedInteractiveObject.Add(PhysicsTriggerInfo.OtherInteractiveObject, rangeIntersectionCalculator);
            }

            this.intersectionCalculators.Remove(rangeIntersectionCalculator);
            this.intersectionCalculatorsIndexedByTrackedInteractiveObject.Remove(PhysicsTriggerInfo.OtherInteractiveObject);
            this.OnTriggerExitSuccess(PhysicsTriggerInfo);
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

        private void SingleJustExited(RangeIntersectionCalculatorV2 JustTriggerExitedRangeIntersectionCalculatorV2)
        {
            //Debug.Log("JustTriggeredExit : " + justTriggerExitedCalculatorIndex + " " + this.justTriggerExitedCalculators.Count);
            this.OnJustNotIntersected(JustTriggerExitedRangeIntersectionCalculatorV2);
            JustTriggerExitedRangeIntersectionCalculatorV2.OnDestroy();
            this.justTriggerExitedCalculatorsIndexedByTrackedInteractiveObject.Remove(JustTriggerExitedRangeIntersectionCalculatorV2.TrackedInteractiveObject);
            this.justTriggerExitedCalculators.Remove(JustTriggerExitedRangeIntersectionCalculatorV2);
        }

        public void RemoveReferencesToInteractiveObject(CoreInteractiveObject InteractiveObjectRefToRemove)
        {
            this.intersectionCalculatorsIndexedByTrackedInteractiveObject.TryGetValue(InteractiveObjectRefToRemove, out RangeIntersectionCalculatorV2 RangeIntersectionCalculatorV2ToRemove);
            if (RangeIntersectionCalculatorV2ToRemove != null)
            {
                RangeIntersectionCalculatorV2ToRemove.OnDestroy();
                this.intersectionCalculators.Remove(RangeIntersectionCalculatorV2ToRemove);
                this.intersectionCalculatorsIndexedByTrackedInteractiveObject.Remove(InteractiveObjectRefToRemove);
            }

            this.justTriggerExitedCalculatorsIndexedByTrackedInteractiveObject.TryGetValue(InteractiveObjectRefToRemove, out RangeIntersectionCalculatorV2 JustTriggeredExitRangeIntersectionCalculatorV2);

            if (JustTriggeredExitRangeIntersectionCalculatorV2 != null)
            {
                JustTriggeredExitRangeIntersectionCalculatorV2.OnDestroy();
                this.justTriggerExitedCalculators.Remove(JustTriggeredExitRangeIntersectionCalculatorV2);
                this.justTriggerExitedCalculatorsIndexedByTrackedInteractiveObject.Remove(InteractiveObjectRefToRemove);
            }
        }
    }

}

/*
 

            foreach (var intersectionCalculator in this.intersectionCalculators)
            {
                intersectionCalculator.OnDestroy();
            }

            foreach (var justTriggerExitedCalculator in this.justTriggerExitedCalculators)
            {
                justTriggerExitedCalculator.OnDestroy();
            }

            this.intersectionCalculators.Clear();
            this.intersectionCalculatorsIndexedByTrackedInteractiveObject.Clear();
            this.justTriggerExitedCalculators.Clear();
            this.justTriggerExitedCalculatorsIndexedByTrackedInteractiveObject.Clear();
     */
