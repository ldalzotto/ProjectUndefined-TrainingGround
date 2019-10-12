using RTPuzzle;

namespace InteractiveObjectTest
{
    [System.Serializable]
    public class LevelCompletionZoneSystemDefinition
    {
        public BoxRangeTypeDefinition BoxRangeTypeDefinition;
    }

    #region Callback Events
    public delegate void OnLevelCompletionTriggerEnterPlayerDelegate(CoreInteractiveObject IntersectedInteractiveObject);
    #endregion

    public class LevelCompletionZoneSystem : AInteractiveObjectSystem
    {
        private RangeObjectV2 LevelCompletionZoneObject;

        public LevelCompletionZoneSystem(CoreInteractiveObject AssociatedInteractiveObject, LevelCompletionZoneSystemDefinition LevelCompletionZoneSystemDefinition,
            OnLevelCompletionTriggerEnterPlayerDelegate OnLevelCompletionTriggerEnterPlayer)
        {
            this.LevelCompletionZoneObject = new BoxRangeObjectV2(AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent,
                new BoxRangeObjectInitialization
                {
                    RangeTypeID = GameConfigurationID.RangeTypeID.TARGET_ZONE,
                    BoxRangeTypeDefinition = LevelCompletionZoneSystemDefinition.BoxRangeTypeDefinition
                }, AssociatedInteractiveObject, "LevelCompletionZoneListener");
            this.LevelCompletionZoneObject.ReceiveEvent(new RangeExternalPhysicsOnlyAddListener { ARangeObjectV2PhysicsEventListener = new LevelCompletionZonePhysicsListener(OnLevelCompletionTriggerEnterPlayer) });
        }

        public override void OnDestroy()
        {
            this.LevelCompletionZoneObject.OnDestroy();
        }
    }

    class LevelCompletionZonePhysicsListener : ARangeObjectV2PhysicsEventListener
    {
        private InteractiveObjectTagStruct ComparedInteractiveObjectTagStruct;

        private OnLevelCompletionTriggerEnterPlayerDelegate OnLevelCompletionTriggerEnterPlayer;

        public LevelCompletionZonePhysicsListener(OnLevelCompletionTriggerEnterPlayerDelegate OnLevelCompletionTriggerEnterPlayer)
        {
            this.OnLevelCompletionTriggerEnterPlayer = OnLevelCompletionTriggerEnterPlayer;
            this.ComparedInteractiveObjectTagStruct = new InteractiveObjectTagStruct { IsPlayer = 1 };
        }

        public override bool ColliderSelectionGuard(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo)
        {
            return this.ComparedInteractiveObjectTagStruct.Compare(RangeObjectPhysicsTriggerInfo.OtherInteractiveObject.InteractiveObjectTag);
        }

        public override void OnTriggerEnter(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            this.OnLevelCompletionTriggerEnterPlayer.Invoke(PhysicsTriggerInfo.OtherInteractiveObject);
        }
    }
}
