using CoreGame;
using UnityEngine;

namespace RTPuzzle
{

    public class RangeIntersectionCalculator
    {
        private RangeTypeObject RangeTypeObjectRef;
        private CollisionType trackedCollider;

        private BlittableTransformChangeListenerManager sightModuleMovementChangeTracker;
        private BlittableTransformChangeListenerManager inRangeCollidersMovementChangeTracker;
        private bool isInside;
        private bool forceObstacleOcclusionIfNecessary;

        public RangeIntersectionCalculator(RangeTypeObject associatedRange, CollisionType trackedCollider, bool forceObstacleOcclusionIfNecessary)
        {
            this.RangeTypeObjectRef = associatedRange;
            this.trackedCollider = trackedCollider;
            this.forceObstacleOcclusionIfNecessary = forceObstacleOcclusionIfNecessary;
            this.sightModuleMovementChangeTracker = new BlittableTransformChangeListenerManager(true, true);
            this.inRangeCollidersMovementChangeTracker = new BlittableTransformChangeListenerManager(true, true);
        }

        public CollisionType TrackedCollider { get => trackedCollider; }
        public bool IsInside { get => isInside; }

        public InterserctionOperationType Tick()
        {
            InterserctionOperationType returnOperation = InterserctionOperationType.Nothing;
            this.sightModuleMovementChangeTracker.Tick(this.RangeTypeObjectRef.transform.position, this.RangeTypeObjectRef.transform.rotation);
            this.inRangeCollidersMovementChangeTracker.Tick(this.TrackedCollider.transform.position, this.trackedCollider.transform.rotation);

            if (this.inRangeCollidersMovementChangeTracker.TransformChangedThatFrame() ||
                this.sightModuleMovementChangeTracker.TransformChangedThatFrame())
            {
                var newInside = this.RangeTypeObjectRef.IsInsideAndNotOccluded((BoxCollider)trackedCollider.GetAssociatedCollider(), this.forceObstacleOcclusionIfNecessary);
                if (this.isInside && !newInside)
                {
                    returnOperation = InterserctionOperationType.JustNotInteresected;
                }
                else if (!this.isInside && newInside)
                {
                    returnOperation = InterserctionOperationType.JustInteresected;
                }
                else if (this.isInside && newInside)
                {
                    returnOperation = InterserctionOperationType.IntersectedNothing;
                }
                this.isInside = newInside;
            }
            else
            {
                if (this.isInside)
                {
                    returnOperation = InterserctionOperationType.IntersectedNothing;
                }
            }

            return returnOperation;
        }
    }

    public enum InterserctionOperationType
    {
        JustInteresected = 0,
        JustNotInteresected = 1,
        IntersectedNothing = 2,
        Nothing = 3
    }

    public interface IRangeIntersectionChangeEventListener
    {
        void HasJustIntersected(CollisionType CollisionType);
        void HasJustNotInteresected(CollisionType CollisionType);
    }
}
