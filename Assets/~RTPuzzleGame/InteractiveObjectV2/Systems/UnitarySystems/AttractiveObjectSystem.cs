using GameConfigurationID;
using RTPuzzle;

namespace InteractiveObjectTest
{
    [System.Serializable]
    public class AttractiveObjectSystemDefinition
    {
        public float EffectRange;
        public float EffectiveTime;
    }

    #region Callback Events
    public delegate void OnAttractiveSystemJustIntersectedDelegate(CoreInteractiveObject IntersectedInteractiveObject);
    public delegate void OnAttractiveSystemJustNotIntersectedDelegate(CoreInteractiveObject IntersectedInteractiveObject);
    public delegate void OnAttractiveSystemInterestedNothingDelegate(CoreInteractiveObject IntersectedInteractiveObject);
    #endregion

    public class AttractiveObjectSystem : AInteractiveObjectSystem
    {

        #region Internal Dependencies
        public RangeObjectV2 SphereRange { get; set; }
        #endregion

        private AttractiveObjectLifetimeTimer AttractiveObjectLifetimeTimer;
        public bool IsAskingTobedestroyed { get; private set; }

        public AttractiveObjectSystem(CoreInteractiveObject InteractiveObject, InteractiveObjectTagStruct physicsInteractionSelectionGuard, AttractiveObjectSystemDefinition AttractiveObjectSystemDefinition,
            OnAttractiveSystemJustIntersectedDelegate onAttractiveSystemJustIntersected = null,
            OnAttractiveSystemJustNotIntersectedDelegate onAttractiveSystemJustNotIntersected = null, OnAttractiveSystemInterestedNothingDelegate onAttractiveSystemInterestedNothing = null)
        {
            this.SphereRange = new SphereRangeObjectV2(InteractiveObject.InteractiveGameObject.InteractiveGameObjectParent, new SphereRangeObjectInitialization
            {
                RangeTypeID = RangeTypeID.ATTRACTIVE_OBJECT_ACTIVE,
                IsTakingIntoAccountObstacles = true,
                SphereRangeTypeDefinition = new SphereRangeTypeDefinition
                {
                    Radius = AttractiveObjectSystemDefinition.EffectRange
                }
            }, InteractiveObject);
            this.IsAskingTobedestroyed = false;

            this.SphereRange.ReceiveEvent(new RangeIntersectionAddIntersectionListenerEvent
            {
                ARangeIntersectionV2Listener = new AttractiveObjectV2IntersectionManagerV2(this.SphereRange, physicsInteractionSelectionGuard,
               onAttractiveSystemJustIntersected, onAttractiveSystemJustNotIntersected, onAttractiveSystemInterestedNothing)
            });
            this.AttractiveObjectLifetimeTimer = new AttractiveObjectLifetimeTimer(AttractiveObjectSystemDefinition.EffectiveTime);
        }

        public override void Tick(float d, float timeAttenuationFactor)
        {
            this.AttractiveObjectLifetimeTimer.Tick(d, timeAttenuationFactor);
            this.IsAskingTobedestroyed = this.AttractiveObjectLifetimeTimer.IsTimeOver();
        }

        public override void OnDestroy()
        {
            this.SphereRange.OnDestroy();
        }
    }

    class AttractiveObjectV2IntersectionManagerV2 : ARangeIntersectionV2Listener
    {
        private CoreInteractiveObject SelfInteractiveObject;
        private InteractiveObjectTagStruct InteractiveObjectSelectionGuard;

        private OnAttractiveSystemJustIntersectedDelegate OnAttractiveSystemJustIntersected;
        private OnAttractiveSystemJustNotIntersectedDelegate OnAttractiveSystemJustNotIntersected;
        private OnAttractiveSystemInterestedNothingDelegate OnAttractiveSystemInterestedNothing;

        public AttractiveObjectV2IntersectionManagerV2(RangeObjectV2 associatedRangeObject,
            InteractiveObjectTagStruct InteractiveObjectSelectionGuard,
            OnAttractiveSystemJustIntersectedDelegate onAttractiveSystemJustIntersected = null,
            OnAttractiveSystemJustNotIntersectedDelegate onAttractiveSystemJustNotIntersected = null, OnAttractiveSystemInterestedNothingDelegate onAttractiveSystemInterestedNothing = null) : base(associatedRangeObject)
        {
            this.InteractiveObjectSelectionGuard = InteractiveObjectSelectionGuard;
            OnAttractiveSystemJustIntersected = onAttractiveSystemJustIntersected;
            OnAttractiveSystemJustNotIntersected = onAttractiveSystemJustNotIntersected;
            OnAttractiveSystemInterestedNothing = onAttractiveSystemInterestedNothing;
        }

        protected override bool ColliderSelectionGuard(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo)
        {
            return this.InteractiveObjectSelectionGuard.Equals(RangeObjectPhysicsTriggerInfo.OtherInteractiveObject.InteractiveObjectTag);
        }

        protected override void OnJustIntersected(RangeIntersectionCalculatorV2 intersectionCalculator)
        {
            if (OnAttractiveSystemJustIntersected != null) { OnAttractiveSystemJustIntersected.Invoke(intersectionCalculator.TrackedInteractiveObject); }
        }

        protected override void OnJustNotIntersected(RangeIntersectionCalculatorV2 intersectionCalculator)
        {
            if (OnAttractiveSystemJustNotIntersected != null) { OnAttractiveSystemJustNotIntersected.Invoke(intersectionCalculator.TrackedInteractiveObject); }
        }

        protected override void OnInterestedNothing(RangeIntersectionCalculatorV2 intersectionCalculator)
        {
            if (OnAttractiveSystemInterestedNothing != null) { OnAttractiveSystemInterestedNothing.Invoke(intersectionCalculator.TrackedInteractiveObject); }
        }
    }
}
