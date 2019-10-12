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

        public LevelCompletionInteractiveObject(LevelCompletionInitializerData LevelCompletionInitializerData,
            InteractiveGameObject interactiveGameObject, bool IsUpdatedInMainManager = true) : base(interactiveGameObject, IsUpdatedInMainManager)
        {
            this.LevelCompletionZoneSystem = new LevelCompletionZoneSystem(this, LevelCompletionInitializerData.LevelCompletionZoneSystemDefinition, new InteractiveObjectTagStruct { IsPlayer = 1 },
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
            LevelCompletionManager.OnLevelCompleted();
        }
    }

}
