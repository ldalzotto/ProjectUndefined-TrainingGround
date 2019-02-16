using UnityEngine;

namespace RTPuzzle
{
    public class PlayerActionEventManager : MonoBehaviour
    {
        #region External Dependencies
        private PlayerActionManager PlayerActionManager;
        #endregion

        public void Init()
        {
            PlayerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
        }

        public void OnRTPPlayerActionStop(RTPPlayerAction stoppedAction)
        {
            PlayerActionManager.StopAction();
        }

        public void OnWheelAwake()
        {
            PlayerActionManager.OnWheelAwake();
        }
        public void OnWheelSleep()
        {
            PlayerActionManager.OnWheelSleep();
        }
        public void OnCurrentNodeSelected()
        {
            var selectedAction = PlayerActionManager.GetCurrentSelectedAction();
            if (!selectedAction.IsOnCoolDown())
            {
                OnRTPPlayerActionStart(selectedAction);
            }
        }
        private void OnRTPPlayerActionStart(RTPPlayerAction PlayerAction)
        {
            OnWheelSleep();
            PlayerActionManager.ExecuteAction(PlayerAction);
        }

    }

}
