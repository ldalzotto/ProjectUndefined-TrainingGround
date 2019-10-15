using GameConfigurationID;
using RTPuzzle;
using System;

namespace InteractiveObjectTest
{
    public class SightObjectSystem : AInteractiveObjectSystem
    {
        private RangeObjectV2 SightRange;

        public SightObjectSystem(CoreInteractiveObject AssocaitedInteractiveObject, SightObjectSystemDefinition SightObjectSystemDefinition, InteractiveObjectTagStruct PhysicsTagEventGuard,
            Action<CoreInteractiveObject> OnSightObjectSystemJustIntersected,
            Action<CoreInteractiveObject> OnSightObjectSystemIntersectedNothing,
            Action<CoreInteractiveObject> OnSightObjectSystemNoMoreIntersected)
        {
            this.SightRange = new RoundedFrustumRangeObjectV2(AssocaitedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent, new RoundedFrustumRangeObjectInitialization
            {
                IsTakingIntoAccountObstacles = true,
                RangeTypeID = RangeTypeID.SIGHT_VISION,
                RoundedFrustumRangeTypeDefinition = new RoundedFrustumRangeTypeDefinition
                {
                    FrustumV2 = SightObjectSystemDefinition.Frustum
                }
            }, AssocaitedInteractiveObject, AssocaitedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.name + "_SightObject");
            this.SightRange.ReceiveEvent(new RangeIntersectionAddIntersectionListenerEvent
            {
                ARangeIntersectionV2Listener = new RangeIntersectionV2Listener_Delegated(this.SightRange, PhysicsTagEventGuard,
                    OnJustIntersectedAction: OnSightObjectSystemJustIntersected, OnInterestedNothingAction: OnSightObjectSystemIntersectedNothing, OnJustNotIntersectedAction: OnSightObjectSystemNoMoreIntersected)
            });
        }
    }
}

