using UnityEngine;
using System.Collections;
using CoreGame;

namespace RTPuzzle
{

    public class RangeIntersectionCalculatorV2
    {
        private RangeObjectV2 RangeObject;
        private CollisionType trackedCollider;

        private BlittableTransformChangeListenerManager sightModuleMovementChangeTracker;
        private BlittableTransformChangeListenerManager inRangeCollidersMovementChangeTracker;
        private bool isInside;
        private bool forceObstacleOcclusionIfNecessary;

        public RangeIntersectionCalculatorV2(RangeObjectV2 RangeObject, CollisionType trackedCollider, bool forceObstacleOcclusionIfNecessary)
        {
            this.RangeObject = RangeObject;
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
            this.sightModuleMovementChangeTracker.Tick(this.RangeObject.RangeGameObjectV2.BoundingCollider.transform.position,
                      this.RangeObject.RangeGameObjectV2.BoundingCollider.transform.rotation);
            this.inRangeCollidersMovementChangeTracker.Tick(this.TrackedCollider.transform.position, this.trackedCollider.transform.rotation);

            if (this.inRangeCollidersMovementChangeTracker.TransformChangedThatFrame() ||
                this.sightModuleMovementChangeTracker.TransformChangedThatFrame())
            {
                var newInside = RangeIntersectionOperations.IsInsideAndNotOccluded(this.RangeObject, (BoxCollider)trackedCollider.GetAssociatedCollider(), this.forceObstacleOcclusionIfNecessary);
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
