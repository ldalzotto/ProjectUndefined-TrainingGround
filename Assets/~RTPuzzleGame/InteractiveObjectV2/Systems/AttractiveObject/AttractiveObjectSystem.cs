using GameConfigurationID;
using RTPuzzle;
using System;

namespace InteractiveObjects
{
    public class AttractiveObjectSystem : AInteractiveObjectSystem
    {

        #region Internal Dependencies
        public RangeObjectV2 SphereRange { get; set; }
        #endregion

        private AttractiveObjectLifetimeTimer AttractiveObjectLifetimeTimer;
        public bool IsAskingTobedestroyed { get; private set; }

        public AttractiveObjectSystem(CoreInteractiveObject InteractiveObject, InteractiveObjectTagStruct physicsInteractionSelectionGuard, AttractiveObjectSystemDefinition AttractiveObjectSystemDefinition,
            Action<CoreInteractiveObject> onAttractiveSystemJustIntersected = null,
            Action<CoreInteractiveObject> onAttractiveSystemJustNotIntersected = null, Action<CoreInteractiveObject> onAttractiveSystemInterestedNothing = null)
        {
            this.SphereRange = new SphereRangeObjectV2(InteractiveObject.InteractiveGameObject.InteractiveGameObjectParent, new SphereRangeObjectInitialization
            {
                RangeTypeID = RangeTypeID.ATTRACTIVE_OBJECT_ACTIVE,
                IsTakingIntoAccountObstacles = true,
                SphereRangeTypeDefinition = new SphereRangeTypeDefinition
                {
                    Radius = AttractiveObjectSystemDefinition.EffectRange
                }
            }, InteractiveObject, InteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.name + "_AttractiveObjectRange");
            this.IsAskingTobedestroyed = false;

            this.SphereRange.ReceiveEvent(new RangeIntersectionAddIntersectionListenerEvent
            {
                ARangeIntersectionV2Listener = new RangeIntersectionV2Listener_Delegated(this.SphereRange, physicsInteractionSelectionGuard, OnJustIntersectedAction: onAttractiveSystemJustIntersected,
                                         OnJustNotIntersectedAction: onAttractiveSystemJustNotIntersected, OnInterestedNothingAction: onAttractiveSystemInterestedNothing)
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
}
