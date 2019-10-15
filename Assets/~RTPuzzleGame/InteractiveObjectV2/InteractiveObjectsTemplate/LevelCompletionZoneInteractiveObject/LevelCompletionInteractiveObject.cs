using RTPuzzle;
using UnityEngine;

namespace InteractiveObjectTest
{
    public class LevelCompletionInteractiveObject : CoreInteractiveObject
    {
        #region External Dependencies
        private LevelCompletionManager LevelCompletionManager = LevelCompletionManager.Get();
        #endregion

        private LevelCompletionZoneSystem LevelCompletionZoneSystem;

        public LevelCompletionInteractiveObject(LevelCompletionInteractiveObjectInitializerData LevelCompletionInitializerData,
            InteractiveGameObject interactiveGameObject, bool IsUpdatedInMainManager = true) : base(interactiveGameObject, IsUpdatedInMainManager)
        {
            this.LevelCompletionZoneSystem = new LevelCompletionZoneSystem(this, LevelCompletionInitializerData.LevelCompletionZoneSystemDefinition, new InteractiveObjectTagStruct { IsPlayer = 1 },
                this.OnLevelCompletionTriggerEnterPlayer);
            this.interactiveObjectTag = new InteractiveObjectTag { IsLevelCompletionZone = true };
        }

        public override void Destroy()
        {
            this.LevelCompletionZoneSystem.OnDestroy();
            base.Destroy();
        }

        private void OnLevelCompletionTriggerEnterPlayer(CoreInteractiveObject IntersectedInteractiveObject)
        {
            LevelCompletionManager.OnLevelCompleted();
        }
    }

}
