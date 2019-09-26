using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public interface IAttractiveObjectModuleDataRetriever
    {
        ModelObjectModule GetModelObjectModule();
        Transform GetTransform();
        IAttractiveObjectModuleEvent GetIAttractiveObjectModuleEvent();
    }

    public partial class AttractiveObjectModule : InteractiveObjectModule, IAttractiveObjectModuleDataRetriever, RangeTypeObjectEventListener
    {

        public static IAttractiveObjectModuleDataRetriever GetAttractiveObjectFromCollisionType(CollisionType collisionType)
        {
            var sphereRange = RangeType.RetrieveFromCollisionType(collisionType);
            if (sphereRange != null)
            {
                return sphereRange.GetComponentInParent<AttractiveObjectModule>();
            }
            return null;
        }

        #region ModuleDependencies
        private ModelObjectModule modelObjectModule;
        #endregion

        #region Internal Dependencies
        private RangeTypeObject sphereRange;
        private AttractiveObjectIntersectionManager AttractiveObjectIntersectionManager;
        #endregion

        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        #region Data Retrieval
        public RangeTypeObject SphereRange { get => sphereRange; }
        #endregion

        #region Range Events
        public void OnRangeTriggerEnter(CollisionType other)
        {
            if (other != null && other.IsAI)
            {
                this.AttractiveObjectIntersectionManager.AddTrackedAI(other, AIObjectType.FromCollisionType(other));
            }
        }

        public void OnRangeTriggerStay(CollisionType other)
        { }

        public void OnRangeTriggerExit(CollisionType other)
        {
            if (other != null && other.IsAI)
            {
                this.AttractiveObjectIntersectionManager.RemoveTrackedAI(other, AIObjectType.FromCollisionType(other));
            }
        }
        #endregion

        #region IAttractiveObjectModuleDataRetriever
        public ModelObjectModule GetModelObjectModule()
        {
            return this.modelObjectModule;
        }
        public Transform GetTransform() { return this.transform; }
        public IAttractiveObjectModuleEvent GetIAttractiveObjectModuleEvent() { return this; }
        #endregion

        public AttractiveObjectId AttractiveObjectId;

        private AttractiveObjectLifetimeTimer AttractiveObjectLifetimeTimer;

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval,
            IInteractiveObjectTypeEvents IInteractiveObjectTypeEvents)
        {
            AttractiveObjectInherentConfigurationData AttractiveObjectInherentConfigurationData = interactiveObjectInitializationObject.AttractiveObjectInherentConfigurationData;

            if (interactiveObjectInitializationObject.AttractiveObjectInherentConfigurationData == null)
            {
                AttractiveObjectInherentConfigurationData = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager.AttractiveObjectsConfiguration()[this.AttractiveObjectId];
            }

            this.modelObjectModule = IInteractiveObjectTypeDataRetrieval.GetModelObjectModule();
            this.sphereRange = GetComponentInChildren<RangeTypeObject>();
            this.sphereRange.Init(new RangeTypeObjectInitializer(), new List<RangeTypeObjectEventListener>() { this });
            this.sphereRange.SetIsAttractiveObject();
            this.AttractiveObjectLifetimeTimer = new AttractiveObjectLifetimeTimer(AttractiveObjectInherentConfigurationData.EffectiveTime);
            this.PuzzleEventsManager = PuzzleGameSingletonInstances.PuzzleEventsManager;
            this.AttractiveObjectIntersectionManager = new AttractiveObjectIntersectionManager(this);
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
            this.AttractiveObjectIntersectionManager.Tick();
            this.AttractiveObjectLifetimeTimer.Tick(d, timeAttenuationFactor);
        }

        public override void OnInteractiveObjectDestroyed()
        {
            this.sphereRange.OnRangeDestroyed();
            this.AttractiveObjectIntersectionManager.OnRangeDestroyed();
        }

        #region Logical Conditions
        public bool IsAskingToBeDestroyed()
        {
            return this.AttractiveObjectLifetimeTimer.IsTimeOver();
        }
        #endregion
    }

    class AttractiveObjectLifetimeTimer
    {
        private float effectiveTime;

        public AttractiveObjectLifetimeTimer(float effectiveTime)
        {
            this.effectiveTime = effectiveTime;
        }

        private float elapsedTime;

        #region Logical Condition
        public bool IsTimeOver()
        {
            return elapsedTime >= effectiveTime;
        }
        #endregion

        public void Tick(float d, float timeAttenuationFactor)
        {
            this.elapsedTime += (d * timeAttenuationFactor);
        }

    }

    class AttractiveObjectIntersectionManager
    {
        private List<RangeIntersectionCalculator> intersectionCalculators = new List<RangeIntersectionCalculator>();
        private Dictionary<CollisionType, AIObjectDataRetriever> AIDataRetireverLookup = new Dictionary<CollisionType, AIObjectDataRetriever>();

        private AttractiveObjectModule associatedAttractiveObject;

        public AttractiveObjectIntersectionManager(AttractiveObjectModule associatedAttractiveObject)
        {
            this.associatedAttractiveObject = associatedAttractiveObject;
        }

        public void AddTrackedAI(CollisionType AICollisionType, AIObjectDataRetriever AIObjectDataRetriever)
        {
            this.AIDataRetireverLookup.Add(AICollisionType, AIObjectDataRetriever);
            var intersectionCalculator = new RangeIntersectionCalculator(this.associatedAttractiveObject.SphereRange, AICollisionType);
            this.intersectionCalculators.Add(intersectionCalculator);
            this.SingleCalculation(intersectionCalculator);
        }

        public void RemoveTrackedAI(CollisionType AICollisionType, AIObjectDataRetriever AIObjectDataRetriever)
        {
            this.AIDataRetireverLookup.Remove(AICollisionType);
            for (var i = this.intersectionCalculators.Count - 1; i >= 0; i--)
            {
                if (this.intersectionCalculators[i].TrackedCollider == AICollisionType)
                {
                    if (this.intersectionCalculators[i].IsInside)
                    {
                        this.AttractiveObjectExit(AIObjectDataRetriever);
                    }
                    this.intersectionCalculators.RemoveAt(i);
                }
            }
        }

        public void Tick()
        {
            foreach (var intersectionCalculator in this.intersectionCalculators)
            {
                this.SingleCalculation(intersectionCalculator);
            }
        }

        private void SingleCalculation(RangeIntersectionCalculator intersectionCalculator)
        {
            var intersectionOperation = intersectionCalculator.Tick();
            if (intersectionOperation == InterserctionOperationType.JustInteresected)
            {
                this.AttractiveObjectEnter(this.AIDataRetireverLookup[intersectionCalculator.TrackedCollider]);
            }
            else if (intersectionOperation == InterserctionOperationType.JustNotInteresected)
            {
                this.AttractiveObjectExit(this.AIDataRetireverLookup[intersectionCalculator.TrackedCollider]);
            }
            else if (intersectionOperation == InterserctionOperationType.IntersectedNothing)
            {
                this.AttractiveObjectStay(this.AIDataRetireverLookup[intersectionCalculator.TrackedCollider]);
            }
            Debug.Log(MyLog.Format(intersectionOperation));
        }

        public void OnRangeDestroyed()
        {
            foreach (var involvedAI in this.AIDataRetireverLookup.Values)
            {
                involvedAI.GetAIBehavior().ReceiveEvent(new AttractiveObjectDestroyedAIBehaviorEvent(this.associatedAttractiveObject));
            }
        }

        private void AttractiveObjectEnter(AIObjectDataRetriever attractedAI)
        {
            attractedAI.GetAIBehavior()
                      .ReceiveEvent(new AttractiveObjectTriggerEnterAIBehaviorEvent(this.associatedAttractiveObject.transform.position, this.associatedAttractiveObject));
        }

        private void AttractiveObjectStay(AIObjectDataRetriever attractedAI)
        {
            attractedAI.GetAIBehavior()
                        .ReceiveEvent(new AttractiveObjectTriggerStayAIBehaviorEvent(this.associatedAttractiveObject.transform.position, this.associatedAttractiveObject));
        }

        private void AttractiveObjectExit(AIObjectDataRetriever attractedAI)
        {
            attractedAI.GetAIBehavior()
                         .ReceiveEvent(new AttractiveObjectTriggerExitAIBehaviorEvent(this.associatedAttractiveObject));
        }

    }
}
