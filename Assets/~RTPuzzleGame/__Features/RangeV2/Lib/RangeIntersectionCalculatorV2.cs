using CoreGame;
using InteractiveObjectTest;
using System.Collections.Generic;

namespace RTPuzzle
{


    public class RangeIntersectionCalculatorV2
    {
        #region External Dependencies
        private RangeIntersectionCalculatorV2Manager RangeIntersectionCalculatorV2Manager = RangeIntersectionCalculatorV2Manager.Get();
        #endregion

        private int RangeIntersectionCalculatorV2UniqueID;

        private RangeObjectV2 AssociatedRangeObject;
        public CoreInteractiveObject TrackedInteractiveObject { get; private set; }
        public bool IsInside { get; private set; }

        private BlittableTransformChangeListenerManager sightModuleMovementChangeTracker;
        private BlittableTransformChangeListenerManager inRangeCollidersMovementChangeTracker;
        private bool forceObstacleOcclusionIfNecessary;

        public RangeIntersectionCalculatorV2(RangeObjectV2 RangeObject, CoreInteractiveObject TrackedInteractiveObject, bool forceObstacleOcclusionIfNecessary)
        {
            this.AssociatedRangeObject = RangeObject;
            this.TrackedInteractiveObject = TrackedInteractiveObject;
            this.forceObstacleOcclusionIfNecessary = forceObstacleOcclusionIfNecessary;
            this.sightModuleMovementChangeTracker = new BlittableTransformChangeListenerManager(true, true);
            this.inRangeCollidersMovementChangeTracker = new BlittableTransformChangeListenerManager(true, true);
            this.RangeIntersectionCalculatorV2UniqueID = RangeIntersectionCalculatorV2Manager.OnRangeIntersectionCalculatorV2ManagerCreation(this);
        }

        public InterserctionOperationType Tick()
        {
            InterserctionOperationType returnOperation = InterserctionOperationType.Nothing;
            this.sightModuleMovementChangeTracker.Tick(this.AssociatedRangeObject.RangeGameObjectV2.BoundingCollider.transform.position,
                      this.AssociatedRangeObject.RangeGameObjectV2.BoundingCollider.transform.rotation);
            var TrackedInteractiveGameObjectTransform = this.TrackedInteractiveObject.InteractiveGameObject.GetTransform();
            this.inRangeCollidersMovementChangeTracker.Tick(TrackedInteractiveGameObjectTransform.WorldPosition, TrackedInteractiveGameObjectTransform.WorldRotation);

            if (this.inRangeCollidersMovementChangeTracker.TransformChangedThatFrame() ||
                this.sightModuleMovementChangeTracker.TransformChangedThatFrame())
            {
                var newInside = RangeIntersectionOperations.IsInsideAndNotOccluded(this.AssociatedRangeObject, this.TrackedInteractiveObject.InteractiveGameObject.LogicCollider, this.forceObstacleOcclusionIfNecessary);
                if (this.IsInside && !newInside)
                {
                    returnOperation = InterserctionOperationType.JustNotInteresected;
                }
                else if (!this.IsInside && newInside)
                {
                    returnOperation = InterserctionOperationType.JustInteresected;
                }
                else if (this.IsInside && newInside)
                {
                    returnOperation = InterserctionOperationType.IntersectedNothing;
                }
                this.IsInside = newInside;
            }
            else
            {
                if (this.IsInside)
                {
                    returnOperation = InterserctionOperationType.IntersectedNothing;
                }
            }

            return returnOperation;
        }

        public ObstacleListener GetAssociatedObstacleListener() { return this.AssociatedRangeObject.GetObstacleListener(); }

        public void Destroy()
        {
            this.RangeIntersectionCalculatorV2Manager.OnRangeIntersectionCalculatorV2ManagerDestroyed(this);
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
