using System.Collections.Generic;
using System.Linq;

namespace RTPuzzle
{
    public class ObjectSightModule : InteractiveObjectModule
    {
        private RangeTypeObject sightVisionRange;
        private AISightIntersectionManagerV2 AISightInteresectionManager;

        public RangeTypeObject SightVisionRange { get => sightVisionRange; }

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval,
            IInteractiveObjectTypeEvents IInteractiveObjectTypeEvents)
        {
            this.ResolveInternalDependencies();
            this.AISightInteresectionManager = new AISightIntersectionManagerV2(this, interactiveObjectInitializationObject.ParentAIObjectTypeReference);
            this.sightVisionRange.Init(new RangeTypeObjectInitializer(), new List<RangeTypeObjectEventListener>() { this.AISightInteresectionManager });
        }

        public void ResolveInternalDependencies()
        {
            this.sightVisionRange = GetComponentInChildren<RangeTypeObject>();
        }

        public void TickBeforeAIUpdate(float d)
        {
            //Ranges are update in container
            this.AISightInteresectionManager.Tick();
        }
        
        #region Logical Conditions
        public bool IsPlayerInSight() { return this.AISightInteresectionManager.IsPlayerInSight(); }
        #endregion
        
#if UNITY_EDITOR
        public void HandlesTick()
        {
            foreach (var rangeType in GetComponentsInChildren<RangeType>().ToList())
            {
                rangeType.HandlesDraw();
            }
        }
#endif

    }

    public class AISightIntersectionManagerV2 : RangeIntersectionManager
    {
        private ObjectSightModule associatedObjectSightModule;
        private AIObjectDataRetriever AssociatedAI;

        public AISightIntersectionManagerV2(ObjectSightModule associatedObjectSightModule, AIObjectDataRetriever associatedAI)
        {
            this.associatedObjectSightModule = associatedObjectSightModule;
            this.AssociatedAI = associatedAI;
        }

        public override void OnRangeTriggerEnter(CollisionType other)
        {
            if (other != null && other.IsPlayer)
            {
                this.AddTrackedCollider(associatedObjectSightModule.SightVisionRange, other);
            }
        }

        public override void OnRangeTriggerExit(CollisionType other)
        {
            if (other != null && other.IsPlayer)
            {
                this.RemoveTrackedCollider(other);
            }
        }

        public override void OnRangeTriggerStay(CollisionType other)
        {
        }
        
        protected override void OnJustIntersected(RangeIntersectionCalculator intersectionCalculator)
        {
            this.AssociatedAI.GetAIBehavior().ReceiveEvent(new SightInRangeEnterAIBehaviorEvent(intersectionCalculator.TrackedCollider));
        }

        protected override void OnJustNotIntersected(RangeIntersectionCalculator intersectionCalculator)
        {
            this.AssociatedAI.GetAIBehavior().ReceiveEvent(new SightInRangeExitAIBehaviorEvent(intersectionCalculator.TrackedCollider));
        }

        protected override void OnInterestedNothing(RangeIntersectionCalculator intersectionCalculator)
        {
        }

        #region Logical Conditions
        public bool IsPlayerInSight()
        {
            foreach (var intersectionCalculator in intersectionCalculators)
            {
                if (intersectionCalculator.IsInside)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }

}
