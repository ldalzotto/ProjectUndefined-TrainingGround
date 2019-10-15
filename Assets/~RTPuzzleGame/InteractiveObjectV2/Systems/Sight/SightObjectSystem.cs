using GameConfigurationID;
using RTPuzzle;
using System;
using System.Collections.Generic;

namespace InteractiveObjectTest
{
    [SceneHandleDraw]
    public class SightObjectSystem : AInteractiveObjectSystem
    {
        [DrawNested]
        private RangeObjectV2 SightRange;

        [VE_Array]
        private List<CoreInteractiveObject> currentlyIntersectedInteractiveObjects = new List<CoreInteractiveObject>();
        public List<CoreInteractiveObject> CurrentlyIntersectedInteractiveObjects { get => currentlyIntersectedInteractiveObjects; }

        private Action<CoreInteractiveObject> OnSightObjectSystemJustIntersected;
        private Action<CoreInteractiveObject> OnSightObjectSystemNoMoreIntersected;

        public SightObjectSystem(CoreInteractiveObject AssocaitedInteractiveObject, SightObjectSystemDefinition SightObjectSystemDefinition, InteractiveObjectTagStruct PhysicsTagEventGuard,
            Action<CoreInteractiveObject> OnSightObjectSystemJustIntersected,
            Action<CoreInteractiveObject> OnSightObjectSystemIntersectedNothing,
            Action<CoreInteractiveObject> OnSightObjectSystemNoMoreIntersected)
        {
            this.OnSightObjectSystemJustIntersected = OnSightObjectSystemJustIntersected;
            this.OnSightObjectSystemNoMoreIntersected = OnSightObjectSystemNoMoreIntersected;

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
                    OnJustIntersectedAction: this.OnSightJustIntersected, OnInterestedNothingAction: OnSightObjectSystemIntersectedNothing, OnJustNotIntersectedAction: this.OnSightNoMoreIntersected)
            });
        }

        private void OnSightJustIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            this.currentlyIntersectedInteractiveObjects.Add(IntersectedInteractiveObject);
            if (this.OnSightObjectSystemJustIntersected != null) { this.OnSightObjectSystemJustIntersected.Invoke(IntersectedInteractiveObject); }
        }
        private void OnSightNoMoreIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            this.currentlyIntersectedInteractiveObjects.Remove(IntersectedInteractiveObject);
            if (this.OnSightObjectSystemNoMoreIntersected != null) { this.OnSightObjectSystemNoMoreIntersected.Invoke(IntersectedInteractiveObject); }
        }

    }
}

