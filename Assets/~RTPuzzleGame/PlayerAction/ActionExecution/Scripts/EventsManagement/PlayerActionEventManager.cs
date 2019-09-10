using CoreGame;
using UnityEngine;

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
            PlayerActionManager = PuzzleGameSingletonInstances.PlayerActionManager;
            CooldownFeedManager = PuzzleGameSingletonInstances.CooldownFeedManager;
        }
        
        public void OnWheelAwake()
        {
            PlayerActionManager.OnWheelAwake();
        }
        public void OnWheelSleep(bool destroyImmediate)
        {
            PlayerActionManager.OnWheelSleep(destroyImmediate);
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
            OnWheelSleep(false);
            PlayerActionManager.ExecuteAction(PlayerAction);
        }
        
    }

}
