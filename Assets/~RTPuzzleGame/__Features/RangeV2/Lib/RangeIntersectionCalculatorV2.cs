using CoreGame;
using InteractiveObjectTest;
using System.Collections.Generic;

namespace RTPuzzle
{


    public class RangeIntersectionCalculatorV2
    {
        public int RangeIntersectionCalculatorV2UniqueID { get; private set; }

        #region External Dependencies
        private RangeIntersectionCalculatorV2Manager RangeIntersectionCalculatorV2Manager = RangeIntersectionCalculatorV2Manager.Get();
        private RangeIntersectionCalculationManagerV2 RangeIntersectionCalculationManagerV2 = RangeIntersectionCalculationManagerV2.Get();
        #endregion

        private RangeObjectV2 AssociatedRangeObject;
        public CoreInteractiveObject TrackedInteractiveObject { get; private set; }
        public bool IsInside { get; private set; }

        private BlittableTransformChangeListenerManager sightModuleMovementChangeTracker;
        private BlittableTransformChangeListenerManager inRangeCollidersMovementChangeTracker;

        public RangeIntersectionCalculatorV2(RangeObjectV2 RangeObject, CoreInteractiveObject TrackedInteractiveObject)
        {
            this.AssociatedRangeObject = RangeObject;
            this.TrackedInteractiveObject = TrackedInteractiveObject;
            this.sightModuleMovementChangeTracker = new BlittableTransformChangeListenerManager(true, true);
            this.inRangeCollidersMovementChangeTracker = new BlittableTransformChangeListenerManager(true, true);
            this.RangeIntersectionCalculatorV2UniqueID = RangeIntersectionCalculatorV2Manager.OnRangeIntersectionCalculatorV2ManagerCreation(this);
            RangeIntersectionCalculationManagerV2.ManualCalculation(new List<RangeIntersectionCalculatorV2>() { this }, forceCalculation: true);
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
                var newInside = this.RangeIntersectionCalculationManagerV2.GetRangeIntersectionResult(this);
               // var newInside = RangeIntersectionOperations.IsInsideAndNotOccluded(this.AssociatedRangeObject, this.TrackedInteractiveObject.InteractiveGameObject.GetLogicColliderBoxDefinition(), this.forceObstacleOcclusionIfNecessary);
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
        public RangeObjectV2 GetAssociatedRangeObject() { return this.AssociatedRangeObject; }
        public RangeType GetAssociatedRangeObjectType() { return this.AssociatedRangeObject.RangeType; }

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
