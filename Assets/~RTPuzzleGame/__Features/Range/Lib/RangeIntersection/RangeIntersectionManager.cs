using System.Collections.Generic;

namespace RTPuzzle
{
    public abstract class RangeIntersectionManager : RangeTypeObjectEventListener
    {
        protected List<RangeIntersectionCalculator> intersectionCalculators = new List<RangeIntersectionCalculator>();

        public abstract void OnRangeTriggerEnter(CollisionType other);
        public abstract void OnRangeTriggerStay(CollisionType other);
        public abstract void OnRangeTriggerExit(CollisionType other);

        protected abstract void OnJustIntersected(RangeIntersectionCalculator intersectionCalculator);
        protected abstract void OnJustNotIntersected(RangeIntersectionCalculator intersectionCalculator);
        protected abstract void OnInterestedNothing(RangeIntersectionCalculator intersectionCalculator);

        public void Tick()
        {
            foreach (var intersectionCalculator in this.intersectionCalculators)
            {
                this.SingleCalculation(intersectionCalculator);
            }
        }

        protected void AddTrackedCollider(RangeTypeObject rangeTypeObject, CollisionType trackedCollisionType)
        {
            var intersectionCalculator = new RangeIntersectionCalculator(rangeTypeObject, trackedCollisionType, forceObstacleOcclusionIfNecessary: true);
            this.intersectionCalculators.Add(intersectionCalculator);
            this.SingleCalculation(intersectionCalculator);
        }

        protected void RemoveTrackedCollider(CollisionType trackedCollider)
        {
            for (var i = this.intersectionCalculators.Count - 1; i >= 0; i--)
            {
                if (this.intersectionCalculators[i].TrackedCollider == trackedCollider)
                {
                    if (this.intersectionCalculators[i].IsInside)
                    {
                        this.OnJustNotIntersected(this.intersectionCalculators[i]);
                    }
                    this.intersectionCalculators.RemoveAt(i);
                }
            }
        }

        private void SingleCalculation(RangeIntersectionCalculator intersectionCalculator)
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