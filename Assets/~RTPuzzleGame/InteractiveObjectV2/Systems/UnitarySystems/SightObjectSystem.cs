using CoreGame;
using GameConfigurationID;
using RTPuzzle;

namespace InteractiveObjectTest
{
    [System.Serializable]
    public class SightObjectSystemDefinition
    {
        public FrustumV2 Frustum;
    }

    #region Callback Events
    public delegate void OnSightObjectSystemJustIntersectedDelegate(CoreInteractiveObject IntersectedInteractiveObject);
    public delegate void OnSightObjectSystemIntersectedNothingDelegate(CoreInteractiveObject IntersectedInteractiveObject);
    public delegate void OnSightObjectSystemNoMoreIntersectedDelegate(CoreInteractiveObject IntersectedInteractiveObject);
    #endregion

    public class SightObjectSystem : AInteractiveObjectSystem
    {
        private RangeObjectV2 SightRange;

        public SightObjectSystem(CoreInteractiveObject AssocaitedInteractiveObject, SightObjectSystemDefinition SightObjectSystemDefinition, InteractiveObjectTagStruct PhysicsTagEventGuard,
            OnSightObjectSystemJustIntersectedDelegate OnSightObjectSystemJustIntersected, 
            OnSightObjectSystemIntersectedNothingDelegate OnSightObjectSystemIntersectedNothing,
            OnSightObjectSystemNoMoreIntersectedDelegate OnSightObjectSystemNoMoreIntersected)
        {
            this.SightRange = new RoundedFrustumRangeObjectV2(AssocaitedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent, new RoundedFrustumRangeObjectInitialization
            {
                IsTakingIntoAccountObstacles = true,
                RangeTypeID = RangeTypeID.SIGHT_VISION,
                RoundedFrustumRangeTypeDefinition = new RoundedFrustumRangeTypeDefinition
                {
                    FrustumV2 = SightObjectSystemDefinition.Frustum
                }
            }, AssocaitedInteractiveObject);
            this.SightRange.ReceiveEvent(new RangeIntersectionAddIntersectionListenerEvent { ARangeIntersectionV2Listener = new SightObjectRangeListener(this.SightRange, PhysicsTagEventGuard, 
                OnSightObjectSystemJustIntersected, OnSightObjectSystemIntersectedNothing, OnSightObjectSystemNoMoreIntersected) });
        }
    }

    public class SightObjectRangeListener : ARangeIntersectionV2Listener
    {
        private InteractiveObjectTagStruct PhysicsTagEventGuard;

        private OnSightObjectSystemJustIntersectedDelegate OnSightObjectSystemJustIntersected;
        private OnSightObjectSystemIntersectedNothingDelegate OnSightObjectSystemIntersectedNothing;
        private OnSightObjectSystemNoMoreIntersectedDelegate OnSightObjectSystemNoMoreIntersected;

        public SightObjectRangeListener(RangeObjectV2 associatedRangeObject, InteractiveObjectTagStruct PhysicsTagEventGuard, OnSightObjectSystemJustIntersectedDelegate OnSightObjectSystemJustIntersected,
       OnSightObjectSystemIntersectedNothingDelegate OnSightObjectSystemIntersectedNothing,     OnSightObjectSystemNoMoreIntersectedDelegate OnSightObjectSystemNoMoreIntersected) : base(associatedRangeObject)
        {
            this.OnSightObjectSystemJustIntersected = OnSightObjectSystemJustIntersected;
            this.OnSightObjectSystemIntersectedNothing = OnSightObjectSystemIntersectedNothing;
            this.OnSightObjectSystemNoMoreIntersected = OnSightObjectSystemNoMoreIntersected;
            this.PhysicsTagEventGuard = PhysicsTagEventGuard;
        }

        public override bool ColliderSelectionGuard(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo)
        {
            return this.PhysicsTagEventGuard.Compare(RangeObjectPhysicsTriggerInfo.OtherInteractiveObject.InteractiveObjectTag);
        }

        protected override void OnJustIntersected(RangeIntersectionCalculatorV2 intersectionCalculator)
        {
            this.OnSightObjectSystemJustIntersected.Invoke(intersectionCalculator.TrackedInteractiveObject);
        }

        protected override void OnInterestedNothing(RangeIntersectionCalculatorV2 intersectionCalculator)
        {
            this.OnSightObjectSystemIntersectedNothing.Invoke(intersectionCalculator.TrackedInteractiveObject);
        }

        protected override void OnJustNotIntersected(RangeIntersectionCalculatorV2 intersectionCalculator)
        {
            this.OnSightObjectSystemNoMoreIntersected.Invoke(intersectionCalculator.TrackedInteractiveObject);
        }
    }
}

