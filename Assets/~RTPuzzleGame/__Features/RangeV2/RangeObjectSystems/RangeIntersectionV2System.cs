using InteractiveObjectTest;
using System.Collections.Generic;

namespace RTPuzzle
{
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
        protected List<RangeIntersectionCalculatorV2> intersectionCalculators = new List<RangeIntersectionCalculatorV2>();
        private List<RangeIntersectionCalculatorV2> justTriggerExitedCalculators = new List<RangeIntersectionCalculatorV2>();

        private Dictionary<CoreInteractiveObject, RangeIntersectionCalculatorV2> intersectionCalculatorsIndexedByInteractiveObject = new Dictionary<CoreInteractiveObject, RangeIntersectionCalculatorV2>();

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
            for (var intersectionCalculatorIndex = 0; intersectionCalculatorIndex < this.intersectionCalculators.Count; intersectionCalculatorIndex++)
            {
                this.SingleCalculation(this.intersectionCalculators[intersectionCalculatorIndex]);
            }

            for (var justTriggerExitedCalculatorIndex = this.justTriggerExitedCalculators.Count - 1; justTriggerExitedCalculatorIndex >= 0; justTriggerExitedCalculatorIndex--)
            {
                this.OnJustNotIntersected(this.justTriggerExitedCalculators[justTriggerExitedCalculatorIndex]);
                this.justTriggerExitedCalculators.RemoveAt(justTriggerExitedCalculatorIndex);
            }
        }

        public sealed override void OnTriggerEnter(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            var rangeIntersectionCalculator = new RangeIntersectionCalculatorV2(this.associatedRangeObject, PhysicsTriggerInfo.OtherInteractiveObject, forceObstacleOcclusionIfNecessary: true);
            this.intersectionCalculators.Add(rangeIntersectionCalculator);
            this.intersectionCalculatorsIndexedByInteractiveObject[PhysicsTriggerInfo.OtherInteractiveObject] = rangeIntersectionCalculator;
            this.OnTriggerEnterSuccess(PhysicsTriggerInfo);
        }

        public sealed override void OnTriggerExit(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            var rangeIntersectionCalculator = this.intersectionCalculatorsIndexedByInteractiveObject[PhysicsTriggerInfo.OtherInteractiveObject];

            if (rangeIntersectionCalculator.IsInside)
            {
                this.justTriggerExitedCalculators.Add(rangeIntersectionCalculator);
            }

            this.intersectionCalculators.Remove(rangeIntersectionCalculator);
            this.intersectionCalculatorsIndexedByInteractiveObject.Remove(PhysicsTriggerInfo.OtherInteractiveObject);
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
    }

}
