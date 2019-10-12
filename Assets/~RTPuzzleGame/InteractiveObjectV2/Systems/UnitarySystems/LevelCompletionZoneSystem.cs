using RTPuzzle;

namespace InteractiveObjectTest
{
    [System.Serializable]
    public class LevelCompletionZoneSystemDefinition
    {
        public RangeObjectInitialization TriggerRangeObjectInitialization;
    }

    #region Callback Events
    public delegate void OnLevelCompletionTriggerEnterPlayerDelegate(CoreInteractiveObject IntersectedInteractiveObject);
    #endregion

    public class LevelCompletionZoneSystem : AInteractiveObjectSystem
    {
        private RangeObjectV2 LevelCompletionZoneObject;

        public LevelCompletionZoneSystem(CoreInteractiveObject AssociatedInteractiveObject, LevelCompletionZoneSystemDefinition LevelCompletionZoneSystemDefinition,
            InteractiveObjectTagStruct ComparedInteractiveObjectTagStruct,
            OnLevelCompletionTriggerEnterPlayerDelegate OnLevelCompletionTriggerEnterPlayer)
        {
            this.LevelCompletionZoneObject = RangeObjectV2Builder.Build(AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent,
              LevelCompletionZoneSystemDefinition.TriggerRangeObjectInitialization, AssociatedInteractiveObject, "LevelCompletionZoneListener");
            this.LevelCompletionZoneObject.ReceiveEvent(new RangeExternalPhysicsOnlyAddListener { ARangeObjectV2PhysicsEventListener = new LevelCompletionZonePhysicsListener(ComparedInteractiveObjectTagStruct,
                OnLevelCompletionTriggerEnterPlayer) });
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

        public LevelCompletionZonePhysicsListener(InteractiveObjectTagStruct ComparedInteractiveObjectTagStruct, 
            OnLevelCompletionTriggerEnterPlayerDelegate OnLevelCompletionTriggerEnterPlayer)
        {
            this.OnLevelCompletionTriggerEnterPlayer = OnLevelCompletionTriggerEnterPlayer;
            this.ComparedInteractiveObjectTagStruct = ComparedInteractiveObjectTagStruct;
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
