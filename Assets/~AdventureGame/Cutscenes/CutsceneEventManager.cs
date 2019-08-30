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
        #endregion

        public void Init()
        {
            this.InventoryEventManager = GameObject.FindObjectOfType<InventoryEventManager>();
            this.ContextActionWheelEventManager = GameObject.FindObjectOfType<ContextActionWheelEventManager>();
            this.CameraMovementManager = GameObject.FindObjectOfType<CameraMovementManager>();
            this.TutorialManager = GameObject.FindObjectOfType<TutorialManager>();
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
    }
}