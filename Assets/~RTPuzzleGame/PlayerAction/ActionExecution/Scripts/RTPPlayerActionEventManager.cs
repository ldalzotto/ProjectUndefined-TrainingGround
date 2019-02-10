using UnityEngine;

namespace RTPuzzle
{
    public class RTPPlayerActionEventManager : MonoBehaviour
    {
        #region External Dependencies
        private RTPPlayerActionManager RTPPlayerActionManager;
        #endregion

        public void Init()
        {
            RTPPlayerActionManager = GameObject.FindObjectOfType<RTPPlayerActionManager>();
        }

        public void OnRTPPlayerActionStop(RTPPlayerAction stoppedAction)
        {
            RTPPlayerActionManager.StopAction();
        }

        public void OnWheelAwake()
        {
            RTPPlayerActionManager.OnWheelAwake();
        }
        public void OnWheelSleep()
        {
            RTPPlayerActionManager.OnWheelSleep();
        }
        public void OnCurrentNodeSelected()
        {
            OnRTPPlayerActionStart(RTPPlayerActionManager.GetCurrentSelectedAction());
        }
        private void OnRTPPlayerActionStart(RTPPlayerAction rTPPlayerAction)
        {
            OnWheelSleep();
            RTPPlayerActionManager.ExecuteAction(rTPPlayerAction);
        }

    }

}
