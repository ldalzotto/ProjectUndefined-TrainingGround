using RTPuzzle;
using System;

namespace InteractiveObjectTest
{
    [System.Serializable]
    [SceneHandleDraw]
    public class LevelCompletionZoneSystemDefinition
    {
        [DrawNested]
        public RangeObjectInitialization TriggerRangeObjectInitialization;
    }

    public class LevelCompletionZoneSystem : AInteractiveObjectSystem
    {
        private RangeObjectV2 LevelCompletionZoneObject;

        public LevelCompletionZoneSystem(CoreInteractiveObject AssociatedInteractiveObject, LevelCompletionZoneSystemDefinition LevelCompletionZoneSystemDefinition,
            InteractiveObjectTagStruct ComparedInteractiveObjectTagStruct,
            Action<CoreInteractiveObject> OnLevelCompletionTriggerEnterPlayer)
        {
            this.LevelCompletionZoneObject = RangeObjectV2Builder.Build(AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent,
              LevelCompletionZoneSystemDefinition.TriggerRangeObjectInitialization, AssociatedInteractiveObject, AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.name + "_LevelCompletionZoneListener");
            this.LevelCompletionZoneObject.ReceiveEvent(new RangeExternalPhysicsOnlyAddListener
            {
                ARangeObjectV2PhysicsEventListener = new RangeObjectV2PhysicsEventListener_Delegated(ComparedInteractiveObjectTagStruct, onTriggerEnterAction: OnLevelCompletionTriggerEnterPlayer)
            });
        }

        public override void OnDestroy()
        {
            this.LevelCompletionZoneObject.OnDestroy();
        }
    }
}
