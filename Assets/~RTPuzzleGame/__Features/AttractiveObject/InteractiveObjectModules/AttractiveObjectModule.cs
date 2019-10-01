using GameConfigurationID;
using System;
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

    public partial class AttractiveObjectModule : InteractiveObjectModule, IAttractiveObjectModuleDataRetriever
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
        private AttractiveObjectIntersectionManagerV2 AttractiveObjectIntersectionManager;
        #endregion
        
        #region Data Retrieval
        public RangeTypeObject SphereRange { get => sphereRange; }
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
            this.AttractiveObjectIntersectionManager = new AttractiveObjectIntersectionManagerV2(this);
            this.sphereRange.Init(new RangeTypeObjectInitializer(), new List<RangeTypeObjectEventListener>() { this.AttractiveObjectIntersectionManager });
            this.sphereRange.SetIsAttractiveObject();
            this.AttractiveObjectLifetimeTimer = new AttractiveObjectLifetimeTimer(AttractiveObjectInherentConfigurationData.EffectiveTime);
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

    class AttractiveObjectIntersectionManagerV2 : RangeIntersectionManager
    {
        private Dictionary<CollisionType, AIObjectDataRetriever> AIDataRetireverLookup = new Dictionary<CollisionType, AIObjectDataRetriever>();
        private AttractiveObjectModule AssociatedAttractiveObjectModule;

        public AttractiveObjectIntersectionManagerV2(AttractiveObjectModule associatedAttractiveObjectModule)
        {
            AssociatedAttractiveObjectModule = associatedAttractiveObjectModule;
        }

        public override void OnRangeTriggerEnter(CollisionType other)
        {
            if (other != null && other.IsAI)
            {
                this.AIDataRetireverLookup.Add(other, AIObjectType.FromCollisionType(other));
                this.AddTrackedCollider(this.AssociatedAttractiveObjectModule.SphereRange, other);
            }
        }

        public override void OnRangeTriggerExit(CollisionType other)
        {
            if (other != null && other.IsAI)
            {
                this.AIDataRetireverLookup.Remove(other);
                this.RemoveTrackedCollider(other);
            }
        }

        public override void OnRangeTriggerStay(CollisionType other)
        {
        }

        protected override void OnJustIntersected(RangeIntersectionCalculator intersectionCalculator)
        {
            this.AttractiveObjectEnter(this.AIDataRetireverLookup[intersectionCalculator.TrackedCollider]);
        }

        protected override void OnJustNotIntersected(RangeIntersectionCalculator intersectionCalculator)
        {
            this.AttractiveObjectExit(this.AIDataRetireverLookup[intersectionCalculator.TrackedCollider]);
        }

        protected override void OnInterestedNothing(RangeIntersectionCalculator intersectionCalculator)
        {
            this.AttractiveObjectStay(this.AIDataRetireverLookup[intersectionCalculator.TrackedCollider]);
        }

        private void AttractiveObjectEnter(AIObjectDataRetriever attractedAI)
        {
            attractedAI.GetAIBehavior()
                      .ReceiveEvent(new AttractiveObjectTriggerEnterAIBehaviorEvent(this.AssociatedAttractiveObjectModule.transform.position, this.AssociatedAttractiveObjectModule));
        }

        private void AttractiveObjectStay(AIObjectDataRetriever attractedAI)
        {
            attractedAI.GetAIBehavior()
                        .ReceiveEvent(new AttractiveObjectTriggerStayAIBehaviorEvent(this.AssociatedAttractiveObjectModule.transform.position, this.AssociatedAttractiveObjectModule));
        }

        private void AttractiveObjectExit(AIObjectDataRetriever attractedAI)
        {
            attractedAI.GetAIBehavior()
                         .ReceiveEvent(new AttractiveObjectTriggerExitAIBehaviorEvent(this.AssociatedAttractiveObjectModule));
        }

        public void OnRangeDestroyed()
        {
            foreach (var involvedAI in this.AIDataRetireverLookup.Values)
            {
                involvedAI.GetAIBehavior().ReceiveEvent(new AttractiveObjectDestroyedAIBehaviorEvent(this.AssociatedAttractiveObjectModule));
            }
        }
    }
}
