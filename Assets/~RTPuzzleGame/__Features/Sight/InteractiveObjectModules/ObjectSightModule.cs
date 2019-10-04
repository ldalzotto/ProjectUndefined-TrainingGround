namespace RTPuzzle
{
    public class ObjectSightModule : InteractiveObjectModule
    {
        public RangeObjectInitialization RangeObjectInitialization;
        public RangeObjectV2 SightVisionRange { get; set; }

        private AISightIntersectionManagerV2 AISightInteresectionManager;

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval,
            IInteractiveObjectTypeEvents IInteractiveObjectTypeEvents)
        {
            this.SightVisionRange = RangeObjectInitializer.FromRangeObjectInitialization(this.RangeObjectInitialization, this.transform.parent.gameObject);
            this.AISightInteresectionManager = new AISightIntersectionManagerV2(this.SightVisionRange, interactiveObjectInitializationObject.ParentAIObjectTypeReference);
            this.SightVisionRange.ReceiveEvent(new RangeIntersectionAddIntersectionListenerEvent { ARangeIntersectionV2Listener = new AISightIntersectionManagerV2(this.SightVisionRange, interactiveObjectInitializationObject.ParentAIObjectTypeReference) });
        }

        public void TickBeforeAIUpdate(float d)
        {
            //Ranges are update in container
            this.AISightInteresectionManager.Tick();
        }

        public override void OnInteractiveObjectDestroyed()
        {
            this.SightVisionRange.OnDestroy();
        }

        #region Logical Conditions
        public bool IsPlayerInSight() { return this.AISightInteresectionManager.IsPlayerInSight(); }
        #endregion

#if UNITY_EDITOR
        public void HandlesTick()
        {
        }
#endif

    }

    public class AISightIntersectionManagerV2 : ARangeIntersectionV2Listener
    {
        private AIObjectDataRetriever AssociatedAI;

        public AISightIntersectionManagerV2(RangeObjectV2 associatedRangeObject, AIObjectDataRetriever associatedAI) : base(associatedRangeObject)
        {
            this.AssociatedAI = associatedAI;
        }

        public override bool ColliderSelectionGuard(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo)
        {
            //  return RangeObjectPhysicsTriggerInfo.OtherCollisionType.IsPlayer;
            return false;
        }

        protected override void OnJustIntersected(RangeIntersectionCalculatorV2 intersectionCalculator)
        {
          //  this.AssociatedAI.GetAIBehavior().ReceiveEvent(new SightInRangeEnterAIBehaviorEvent(intersectionCalculator.TrackedCollider));
        }

        protected override void OnJustNotIntersected(RangeIntersectionCalculatorV2 intersectionCalculator)
        {
        //    this.AssociatedAI.GetAIBehavior().ReceiveEvent(new SightInRangeExitAIBehaviorEvent(intersectionCalculator.TrackedCollider));
        }

        #region Logical Conditions
        public bool IsPlayerInSight()
        {
            foreach (var intersectionCalculator in this.intersectionCalculators)
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
