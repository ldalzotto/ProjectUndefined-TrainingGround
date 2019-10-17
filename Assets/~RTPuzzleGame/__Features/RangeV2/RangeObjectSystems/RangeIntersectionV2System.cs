using System;
using System.Collections.Generic;
using InteractiveObjects;

namespace RTPuzzle
{
    public class RangeIntersectionV2System : ARangeObjectSystem
    {
        public RangeIntersectionV2System(RangeObjectV2 rangeObjectV2Ref) : base(rangeObjectV2Ref)
        {
        }

        public List<ARangeIntersectionV2Listener> RangeIntersectionListeners { get; private set; } = null;

        public override void Tick(float d)
        {
            if (RangeIntersectionListeners != null)
                for (var RangeIntersectionListenerIndex = 0; RangeIntersectionListenerIndex < RangeIntersectionListeners.Count; RangeIntersectionListenerIndex++)
                    RangeIntersectionListeners[RangeIntersectionListenerIndex].Tick();
        }

        public void ReceiveEvent(RangeIntersectionAddIntersectionListenerEvent RangeIntersectionAddIntersectionListenerEvent, RangeObjectV2PhysicsEventListener RangeObjectV2PhysicsEventListener)
        {
            if (RangeIntersectionListeners == null) RangeIntersectionListeners = new List<ARangeIntersectionV2Listener>();

            RangeObjectV2PhysicsEventListener.AddPhysicsEventListener(RangeIntersectionAddIntersectionListenerEvent.ARangeIntersectionV2Listener);
            RangeIntersectionListeners.Add(RangeIntersectionAddIntersectionListenerEvent.ARangeIntersectionV2Listener);
        }

        public void OnDestroy()
        {
            if (RangeIntersectionListeners != null)
                for (var RangeIntersectionListenerIndex = 0; RangeIntersectionListenerIndex < RangeIntersectionListeners.Count; RangeIntersectionListenerIndex++)
                    RangeIntersectionListeners[RangeIntersectionListenerIndex].OnDestroy();
        }
    }

    public struct RangeIntersectionAddIntersectionListenerEvent
    {
        public ARangeIntersectionV2Listener ARangeIntersectionV2Listener;
    }

    public abstract class ARangeIntersectionV2Listener : ARangeObjectV2PhysicsEventListener
    {
        protected RangeObjectV2 associatedRangeObject;
        protected List<RangeIntersectionCalculatorV2> intersectionCalculators = new List<RangeIntersectionCalculatorV2>();
        private Dictionary<CoreInteractiveObject, RangeIntersectionCalculatorV2> intersectionCalculatorsIndexedByTrackedInteractiveObject = new Dictionary<CoreInteractiveObject, RangeIntersectionCalculatorV2>();

        private List<RangeIntersectionCalculatorV2> justTriggerExitedCalculators = new List<RangeIntersectionCalculatorV2>();
        private Dictionary<CoreInteractiveObject, RangeIntersectionCalculatorV2> justTriggerExitedCalculatorsIndexedByTrackedInteractiveObject = new Dictionary<CoreInteractiveObject, RangeIntersectionCalculatorV2>();

        protected ARangeIntersectionV2Listener(RangeObjectV2 associatedRangeObject)
        {
            this.associatedRangeObject = associatedRangeObject;
        }

        protected virtual void OnJustIntersected(RangeIntersectionCalculatorV2 intersectionCalculator)
        {
        }

        protected virtual void OnJustNotIntersected(RangeIntersectionCalculatorV2 intersectionCalculator)
        {
        }

        protected virtual void OnInterestedNothing(RangeIntersectionCalculatorV2 intersectionCalculator)
        {
        }

        protected virtual void OnTriggerEnterSuccess(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo)
        {
        }

        protected virtual void OnTriggerExitSuccess(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo)
        {
        }

        public virtual void OnDestroy()
        {
        }

        public void Tick()
        {
            foreach (var intersectionCalculator in intersectionCalculators) SingleCalculation(intersectionCalculator);

            for (var i = justTriggerExitedCalculators.Count - 1; i >= 0; i--) SingleJustExited(justTriggerExitedCalculators[i]);
        }

        public sealed override void OnTriggerEnter(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            var rangeIntersectionCalculator = new RangeIntersectionCalculatorV2(associatedRangeObject, PhysicsTriggerInfo.OtherInteractiveObject);
            intersectionCalculators.Add(rangeIntersectionCalculator);
            intersectionCalculatorsIndexedByTrackedInteractiveObject[PhysicsTriggerInfo.OtherInteractiveObject] = rangeIntersectionCalculator;
            OnTriggerEnterSuccess(PhysicsTriggerInfo);
        }

        public sealed override void OnTriggerExit(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            var rangeIntersectionCalculator = intersectionCalculatorsIndexedByTrackedInteractiveObject[PhysicsTriggerInfo.OtherInteractiveObject];

            if (rangeIntersectionCalculator.IsInside)
            {
                justTriggerExitedCalculators.Add(rangeIntersectionCalculator);
                justTriggerExitedCalculatorsIndexedByTrackedInteractiveObject.Add(PhysicsTriggerInfo.OtherInteractiveObject, rangeIntersectionCalculator);
            }

            intersectionCalculators.Remove(rangeIntersectionCalculator);
            intersectionCalculatorsIndexedByTrackedInteractiveObject.Remove(PhysicsTriggerInfo.OtherInteractiveObject);
            OnTriggerExitSuccess(PhysicsTriggerInfo);
        }

        private void SingleCalculation(RangeIntersectionCalculatorV2 intersectionCalculator)
        {
            var intersectionOperation = intersectionCalculator.Tick();
            if (intersectionOperation == InterserctionOperationType.JustInteresected)
                OnJustIntersected(intersectionCalculator);
            else if (intersectionOperation == InterserctionOperationType.JustNotInteresected)
                OnJustNotIntersected(intersectionCalculator);
            else if (intersectionOperation == InterserctionOperationType.IntersectedNothing) OnInterestedNothing(intersectionCalculator);
        }

        private void SingleJustExited(RangeIntersectionCalculatorV2 JustTriggerExitedRangeIntersectionCalculatorV2)
        {
            //Debug.Log("JustTriggeredExit : " + justTriggerExitedCalculatorIndex + " " + this.justTriggerExitedCalculators.Count);
            OnJustNotIntersected(JustTriggerExitedRangeIntersectionCalculatorV2);
            JustTriggerExitedRangeIntersectionCalculatorV2.OnDestroy();
            justTriggerExitedCalculatorsIndexedByTrackedInteractiveObject.Remove(JustTriggerExitedRangeIntersectionCalculatorV2.TrackedInteractiveObject);
            justTriggerExitedCalculators.Remove(JustTriggerExitedRangeIntersectionCalculatorV2);
        }

        public void RemoveReferencesToInteractiveObject(CoreInteractiveObject InteractiveObjectRefToRemove)
        {
            intersectionCalculatorsIndexedByTrackedInteractiveObject.TryGetValue(InteractiveObjectRefToRemove, out var RangeIntersectionCalculatorV2ToRemove);
            if (RangeIntersectionCalculatorV2ToRemove != null)
            {
                RangeIntersectionCalculatorV2ToRemove.OnDestroy();
                intersectionCalculators.Remove(RangeIntersectionCalculatorV2ToRemove);
                intersectionCalculatorsIndexedByTrackedInteractiveObject.Remove(InteractiveObjectRefToRemove);
            }

            justTriggerExitedCalculatorsIndexedByTrackedInteractiveObject.TryGetValue(InteractiveObjectRefToRemove, out var JustTriggeredExitRangeIntersectionCalculatorV2);

            if (JustTriggeredExitRangeIntersectionCalculatorV2 != null)
            {
                JustTriggeredExitRangeIntersectionCalculatorV2.OnDestroy();
                justTriggerExitedCalculators.Remove(JustTriggeredExitRangeIntersectionCalculatorV2);
                justTriggerExitedCalculatorsIndexedByTrackedInteractiveObject.Remove(InteractiveObjectRefToRemove);
            }
        }
    }

    public class RangeIntersectionV2Listener_Delegated : ARangeIntersectionV2Listener
    {
        protected InteractiveObjectTagStruct InteractiveObjectSelectionGuard;
        private Action<CoreInteractiveObject> OnInterestedNothingAction = null;
        private Action<CoreInteractiveObject> OnJustIntersectedAction = null;
        private Action<CoreInteractiveObject> OnJustNotIntersectedAction = null;
        private Action<CoreInteractiveObject> OnTriggerEnterSuccessAction = null;
        private Action<CoreInteractiveObject> OnTriggerExitSuccessAction = null;

        public RangeIntersectionV2Listener_Delegated(RangeObjectV2 associatedRangeObject, InteractiveObjectTagStruct InteractiveObjectSelectionGuard,
            Action<CoreInteractiveObject> OnInterestedNothingAction = null, Action<CoreInteractiveObject> OnJustIntersectedAction = null, Action<CoreInteractiveObject> OnJustNotIntersectedAction = null,
            Action<CoreInteractiveObject> OnTriggerEnterSuccessAction = null, Action<CoreInteractiveObject> OnTriggerExitSuccessAction = null) : base(associatedRangeObject)
        {
            this.OnInterestedNothingAction = OnInterestedNothingAction;
            this.OnJustIntersectedAction = OnJustIntersectedAction;
            this.OnJustNotIntersectedAction = OnJustNotIntersectedAction;
            this.OnTriggerEnterSuccessAction = OnTriggerEnterSuccessAction;
            this.OnTriggerExitSuccessAction = OnTriggerExitSuccessAction;
            this.InteractiveObjectSelectionGuard = InteractiveObjectSelectionGuard;
        }

        public override bool ColliderSelectionGuard(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo)
        {
            return InteractiveObjectSelectionGuard.Compare(RangeObjectPhysicsTriggerInfo.OtherInteractiveObject.InteractiveObjectTag);
        }

        protected override void OnInterestedNothing(RangeIntersectionCalculatorV2 intersectionCalculator)
        {
            if (OnInterestedNothingAction != null) OnInterestedNothingAction.Invoke(intersectionCalculator.TrackedInteractiveObject);
        }

        protected override void OnJustIntersected(RangeIntersectionCalculatorV2 intersectionCalculator)
        {
            if (OnJustIntersectedAction != null) OnJustIntersectedAction.Invoke(intersectionCalculator.TrackedInteractiveObject);
        }

        protected override void OnJustNotIntersected(RangeIntersectionCalculatorV2 intersectionCalculator)
        {
            if (OnJustNotIntersectedAction != null) OnJustNotIntersectedAction.Invoke(intersectionCalculator.TrackedInteractiveObject);
        }

        protected override void OnTriggerEnterSuccess(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo)
        {
            if (OnTriggerEnterSuccessAction != null) OnTriggerEnterSuccessAction.Invoke(RangeObjectPhysicsTriggerInfo.OtherInteractiveObject);
        }

        protected override void OnTriggerExitSuccess(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo)
        {
            if (OnTriggerExitSuccessAction != null) OnTriggerExitSuccessAction.Invoke(RangeObjectPhysicsTriggerInfo.OtherInteractiveObject);
        }
    }
}