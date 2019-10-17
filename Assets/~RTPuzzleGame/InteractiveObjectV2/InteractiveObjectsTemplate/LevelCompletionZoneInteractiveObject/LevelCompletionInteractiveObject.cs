using RTPuzzle;
using UnityEngine;

namespace InteractiveObjects
{
    public class LevelCompletionInteractiveObject : CoreInteractiveObject
    {
        #region External Dependencies
        private LevelCompletionManager LevelCompletionManager = LevelCompletionManager.Get();
        #endregion

        private LevelCompletionZoneSystem LevelCompletionZoneSystem;

        public LevelCompletionInteractiveObject(LevelCompletionInteractiveObjectInitializerData LevelCompletionInitializerData,
            IInteractiveGameObject interactiveGameObject, bool IsUpdatedInMainManager = true) : base(interactiveGameObject, IsUpdatedInMainManager)
        {
            this.LevelCompletionZoneSystem = new LevelCompletionZoneSystem(this, LevelCompletionInitializerData.LevelCompletionZoneSystemDefinition, new InteractiveObjectTagStruct { IsPlayer = 1 },
                this.OnLevelCompletionTriggerEnterPlayer);
            this.interactiveObjectTag = new InteractiveObjectTag { IsLevelCompletionZone = true };

            this.AfterConstructor();
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
