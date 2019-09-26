using System.Collections.Generic;
using System.Linq;

namespace RTPuzzle
{
    public class ObjectSightModule : InteractiveObjectModule, RangeTypeObjectEventListener
    {
        private RangeTypeObject sightVisionRange;
        private AISightIntersectionManager AISightInteresectionManager;

        public RangeTypeObject SightVisionRange { get => sightVisionRange; }

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval,
            IInteractiveObjectTypeEvents IInteractiveObjectTypeEvents)
        {
            this.ResolveInternalDependencies();
            this.AISightInteresectionManager = new AISightIntersectionManager(interactiveObjectInitializationObject.ParentAIObjectTypeReference);
            this.sightVisionRange.Init(new RangeTypeObjectInitializer(), new List<RangeTypeObjectEventListener>() { this });
        }

        public void ResolveInternalDependencies()
        {
            this.sightVisionRange = GetComponentInChildren<RangeTypeObject>();
        }

        public void TickBeforeAIUpdate(float d)
        {
            //Ranges are update in container
            this.AISightInteresectionManager.Tick(d);
        }

        #region Internal Events    
        public void OnTargetTriggerExit(CollisionType CollisionType)
        {
            this.AISightInteresectionManager.OnTargetTriggerExit(CollisionType);
        }
        #endregion

        #region Logical Conditions
        public bool IsPlayerInSight() { return this.AISightInteresectionManager.IsPlayerInSight(); }
        #endregion

        public void OnRangeTriggerEnter(CollisionType collisionType)
        {
            if (collisionType != null && collisionType.IsPlayer)
            {
                this.AISightInteresectionManager.OnTargetTriggerEnter(sightVisionRange, collisionType);
            }
        }
        public void OnRangeTriggerStay(CollisionType other) { }
        public void OnRangeTriggerExit(CollisionType collisionType)
        {
            if (collisionType != null && collisionType.IsPlayer)
            {
                this.OnTargetTriggerExit(collisionType);
            }

        }

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
    public class AISightIntersectionManager
    {
        private List<RangeIntersectionCalculator> intersectionCalculators = new List<RangeIntersectionCalculator>();

        private AIObjectDataRetriever AssociatedAI;

        public AISightIntersectionManager(AIObjectDataRetriever associatedAI)
        {
            AssociatedAI = associatedAI;
        }

        public void Tick(float d)
        {
            foreach (var intersectionCalculator in intersectionCalculators)
            {
                var intersectionOperation = intersectionCalculator.Tick();
                if (intersectionOperation == InterserctionOperationType.JustInteresected)
                {
                    this.SightInRangeEnter(intersectionCalculator.TrackedCollider);
                }
                else if (intersectionOperation == InterserctionOperationType.JustNotInteresected)
                {
                    this.SightInRangeExit(intersectionCalculator.TrackedCollider);
                }
            }
        }

        #region External Event
        public void OnTargetTriggerExit(CollisionType ColliderWithCollisionType)
        {
            for (var i = this.intersectionCalculators.Count - 1; i >= 0; i--)
            {
                if (this.intersectionCalculators[i].TrackedCollider == ColliderWithCollisionType)
                {
                    if (this.intersectionCalculators[i].IsInside)
                    {
                        this.SightInRangeExit(ColliderWithCollisionType);
                    }
                    this.intersectionCalculators.RemoveAt(i);
                }
            }
        }

        public void OnTargetTriggerEnter(RangeTypeObject sightVisionRange, CollisionType collisionType)
        {
            this.intersectionCalculators.Add(new RangeIntersectionCalculator(sightVisionRange, collisionType));
        }
        #endregion

        #region Internal Event
        private void SightInRangeEnter(CollisionType trackedCollider)
        {
            this.AssociatedAI.GetAIBehavior().ReceiveEvent(new SightInRangeEnterAIBehaviorEvent(trackedCollider));
        }
        private void SightInRangeExit(CollisionType trackedCollider)
        {
            this.AssociatedAI.GetAIBehavior().ReceiveEvent(new SightInRangeExitAIBehaviorEvent(trackedCollider));
        }
        #endregion

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
