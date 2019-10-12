using UnityEngine;

namespace InteractiveObjectTest
{
    public class LevelCompletionInteractiveObject : CoreInteractiveObject
    {
        private LevelCompletionZoneSystem LevelCompletionZoneSystem;

        public LevelCompletionInteractiveObject(LevelCompletionInitializerData LevelCompletionInitializerData,
            InteractiveGameObject interactiveGameObject, bool IsUpdatedInMainManager = true) : base(interactiveGameObject, IsUpdatedInMainManager)
        {
            this.LevelCompletionZoneSystem = new LevelCompletionZoneSystem(this, LevelCompletionInitializerData.LevelCompletionZoneSystemDefinition,
                this.OnLevelCompletionTriggerEnterPlayer);
            this.InteractiveObjectTag = new InteractiveObjectTag { IsLevelCompletionZone = true };
        }

        public override void Destroy()
        {
            this.LevelCompletionZoneSystem.OnDestroy();
            base.Destroy();
        }

        protected override void OnLevelCompletionTriggerEnterPlayer(CoreInteractiveObject IntersectedInteractiveObject)
        {
            Debug.Log("TRY LEVEL COMPLETION");
        }
    }

}
