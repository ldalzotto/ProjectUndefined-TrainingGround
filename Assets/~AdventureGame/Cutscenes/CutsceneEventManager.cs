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
        #endregion

        public void Init()
        {
            this.InventoryEventManager = GameObject.FindObjectOfType<InventoryEventManager>();
            this.ContextActionWheelEventManager = GameObject.FindObjectOfType<ContextActionWheelEventManager>();
        }

        public void OnCutscneStarted()
        {
            this.InventoryEventManager.OnInventoryDisabled();
            this.ContextActionWheelEventManager.OnWheelDisabled();
            CoreGameSingletonInstances.TutorialManager.AbortAllTutorials();
        }
    }
}