﻿using UnityEngine;

namespace RTPuzzle
{
    public class PlayerActionEventManager : MonoBehaviour
    {
        #region External Dependencies
        private PlayerActionManager PlayerActionManager;
        private CooldownFeedManager CooldownFeedManager;
        #endregion

        public void Init()
        {
            PlayerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
            CooldownFeedManager = GameObject.FindObjectOfType<CooldownFeedManager>();
        }
        
        public void OnWheelAwake()
        {
            PlayerActionManager.OnWheelAwake();
        }
        public void OnWheelSleep()
        {
            PlayerActionManager.OnWheelSleep();
        }
        public void OnRTPPlayerActionStop(RTPPlayerAction stoppedAction)
        {
            PlayerActionManager.StopAction();
            CooldownFeedManager.OnRTPPlayerActionStop(stoppedAction);
        }
        public void OnCurrentNodeSelected()
        {
            var selectedAction = PlayerActionManager.GetCurrentSelectedAction();
            if (selectedAction.CanBeExecuted())
            {
                OnRTPPlayerActionStart(selectedAction);
            }
        }
        public void OnCooldownEnded(RTPPlayerAction involvedAction)
        {
            CooldownFeedManager.OnCooldownEnded(involvedAction);
        }
        
        private void OnRTPPlayerActionStart(RTPPlayerAction PlayerAction)
        {
            OnWheelSleep();
            PlayerActionManager.ExecuteAction(PlayerAction);
        }
        
    }

}
