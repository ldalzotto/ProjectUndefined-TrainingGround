using UnityEngine;

public class RTPPlayerActionEventManager : MonoBehaviour
{
    #region External Dependencies
    private RTPPlayerActionManager RTPPlayerActionManager;
    #endregion

    public void Init()
    {
        RTPPlayerActionManager = GameObject.FindObjectOfType<RTPPlayerActionManager>();
    }

    public void OnRTPPlayerActionStart(RTPPlayerAction rTPPlayerAction)
    {
        RTPPlayerActionManager.ExecuteAction(rTPPlayerAction);
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

}
