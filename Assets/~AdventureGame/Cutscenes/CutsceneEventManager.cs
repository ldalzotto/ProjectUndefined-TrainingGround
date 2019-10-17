using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace AdventureGame
{
    public class CutsceneEventManager : MonoBehaviour
    {
        #region External Dependencies
        private InventoryEventManager InventoryEventManager;
        private ContextActionWheelEventManager ContextActionWheelEventManager;
        private CameraMovementManager CameraMovementManager;
        private TutorialManager TutorialManager;
        private CutscenePlayerManagerV2 CutscenePlayerManagerV2;
        #endregion

        public void Init()
        {
            this.InventoryEventManager = AdventureGameSingletonInstances.InventoryEventManager;
            this.ContextActionWheelEventManager = AdventureGameSingletonInstances.ContextActionWheelEventManager;
            this.CameraMovementManager = CameraMovementManager.Get();
            this.TutorialManager = CoreGameSingletonInstances.TutorialManager;
            this.CutscenePlayerManagerV2 = AdventureGameSingletonInstances.CutscenePlayerManagerV2;
        }

        public void OnCutscneStarted()
        {
            this.InventoryEventManager.OnInventoryDisabled();
            this.ContextActionWheelEventManager.OnWheelDisabled();
            this.TutorialManager.Abort();
            this.CameraMovementManager.DisableInput();
        }

        public void OnCutsceneEnded()
        {
            this.CameraMovementManager.EnableInput();
        }

        public void PushDeferredPersistance(CutsceneDeferredPOIpersistanceInput CutsceneDeferredPOIpersistanceInput)
        {
            this.CutscenePlayerManagerV2.PushDeferredPersistance(CutsceneDeferredPOIpersistanceInput);
        }
    }
}