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

    public partial class AttractiveObjectModule : InteractiveObjectModule, IAttractiveObjectModuleDataRetriever
    {

        #region ModuleDependencies
        private ModelObjectModule modelObjectModule;
        #endregion

        #region Internal Dependencies
        public RangeObjectV2 SphereRange { get; set; }
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
            this.SphereRange = new SphereRangeObjectV2(this.transform.parent.gameObject, new SphereRangeObjectInitialization
            {
                RangeTypeID = RangeTypeID.ATTRACTIVE_OBJECT_ACTIVE,
                IsTakingIntoAccountObstacles = true,
                SphereRangeTypeDefinition = new SphereRangeTypeDefinition
                {
                    Radius = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager.AttractiveObjectsConfiguration()[this.AttractiveObjectId].EffectRange
                }
            }, null);

            AttractiveObjectInherentConfigurationData AttractiveObjectInherentConfigurationData = interactiveObjectInitializationObject.AttractiveObjectInherentConfigurationData;

            if (interactiveObjectInitializationObject.AttractiveObjectInherentConfigurationData == null)
            {
                AttractiveObjectInherentConfigurationData = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager.AttractiveObjectsConfiguration()[this.AttractiveObjectId];
            }

            this.modelObjectModule = IInteractiveObjectTypeDataRetrieval.GetModelObjectModule();
            this.SphereRange.ReceiveEvent(new RangeIntersectionAddIntersectionListenerEvent { ARangeIntersectionV2Listener = new AttractiveObjectIntersectionManagerV2(this, this.SphereRange) });
            this.AttractiveObjectLifetimeTimer = new AttractiveObjectLifetimeTimer(AttractiveObjectInherentConfigurationData.EffectiveTime);
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
            this.AttractiveObjectLifetimeTimer.Tick(d, timeAttenuationFactor);
        }

        public override void OnInteractiveObjectDestroyed()
        {
            this.SphereRange.OnDestroy();
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

    class AttractiveObjectIntersectionManagerV2 : ARangeIntersectionV2Listener
    {
        private Dictionary<CollisionType, AIObjectDataRetriever> AIDataRetrieverLookup = new Dictionary<CollisionType, AIObjectDataRetriever>();
        private AttractiveObjectModule AssociatedAttractiveObjectModule;

        public AttractiveObjectIntersectionManagerV2(AttractiveObjectModule AssociatedAttractiveObjectModule, RangeObjectV2 associatedRangeObject) : base(associatedRangeObject)
        {
            this.AssociatedAttractiveObjectModule = AssociatedAttractiveObjectModule;
        }

        public override bool ColliderSelectionGuard(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo)
        {
            // return RangeObjectPhysicsTriggerInfo.OtherCollisionType.IsAI;
            return false;
        }

        protected override void OnJustIntersected(RangeIntersectionCalculatorV2 intersectionCalculator)
        {
      //      this.AIDataRetrieverLookup[intersectionCalculator.TrackedCollider].GetAIBehavior()
         //             .ReceiveEvent(new AttractiveObjectTriggerEnterAIBehaviorEvent(this.AssociatedAttractiveObjectModule.transform.position, this.AssociatedAttractiveObjectModule));
        }

        protected override void OnJustNotIntersected(RangeIntersectionCalculatorV2 intersectionCalculator)
        {
       //     this.AIDataRetrieverLookup[intersectionCalculator.TrackedCollider].GetAIBehavior()
       //                  .ReceiveEvent(new AttractiveObjectTriggerExitAIBehaviorEvent(this.AssociatedAttractiveObjectModule));
        }

        protected override void OnInterestedNothing(RangeIntersectionCalculatorV2 intersectionCalculator)
        {
       //     this.AIDataRetrieverLookup[intersectionCalculator.TrackedCollider].GetAIBehavior()
        //                .ReceiveEvent(new AttractiveObjectTriggerStayAIBehaviorEvent(this.AssociatedAttractiveObjectModule.transform.position, this.AssociatedAttractiveObjectModule));
        }

        protected override void OnTriggerEnterSuccess(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo)
        {
       //     this.AIDataRetrieverLookup[RangeObjectPhysicsTriggerInfo.OtherCollisionType] = AIObjectType.FromCollisionType(RangeObjectPhysicsTriggerInfo.OtherCollisionType);
        }

        protected override void OnTriggerExitSuccess(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo)
        {
      //      this.AIDataRetrieverLookup.Remove(RangeObjectPhysicsTriggerInfo.OtherCollisionType);
        }
        
        public void OnRangeDestroyed()
        {
            foreach (var involvedAI in this.AIDataRetrieverLookup.Values)
            {
                involvedAI.GetAIBehavior().ReceiveEvent(new AttractiveObjectDestroyedAIBehaviorEvent(this.AssociatedAttractiveObjectModule));
            }
        }
    }
}
