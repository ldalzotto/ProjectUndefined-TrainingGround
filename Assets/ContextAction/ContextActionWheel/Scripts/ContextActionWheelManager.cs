using UnityEngine;

public class ContextActionWheelManager : MonoBehaviour
{

    [Header("TO REMOVE TEST")]
    public bool SimulateContextAction;

    private PlayerManager PlayerManager;
    private ContextActionManager ContextActionManager;

    private WheelActivityManager WheelActivityManager;

    private void Start()
    {
        #region External Dependencies
        PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
        ContextActionManager = GameObject.FindObjectOfType<ContextActionManager>();
        GameInputManager GameInputManager = GameObject.FindObjectOfType<GameInputManager>();
        #endregion

        WheelActivityManager = new WheelActivityManager(GameInputManager);
    }

    public void Tick(float d)
    {
        if (WheelActivityManager.IsEnabled)
        {

            if (WheelActivityManager.TickCancelInput())
            {
                SleepWheel();
            }
            else
            {
                #region TO DELETE FOR TEST
                if (SimulateContextAction)
                {
                    SleepWheel();
                    SimulateContextAction = false;
                    TriggerContextAction(GameObject.FindObjectOfType<DummyContextAction>());
                }
                #endregion
            }
        }
    }

    //TODO, the triggered POI must be passed here
    private void TriggerContextAction(AContextAction contextAction)
    {
        if (contextAction.GetType() == typeof(DummyContextAction))
        {
            var dummyInput = new DummyContextActionInput("TEST");
            PlayerManager.OnContextActionAdded(contextAction);
            ContextActionManager.AddAction(contextAction, dummyInput);
        }

    }

    #region External Events
    public void AwakeWheel(WheelDisabled onWheelDisabledCallback, PointOfInterestType triggeredPOI)
    {
        Debug.Log(triggeredPOI.name);
        WheelActivityManager.AwakeWheel();
        OnWheelDisabled += onWheelDisabledCallback;
    }
    #endregion

    #region Internal Events
    public void SleepWheel()
    {
        WheelActivityManager.SleepWheel();
        OnWheelDisabled.Invoke();
        OnWheelDisabled = null;
    }

    public delegate void WheelDisabled();
    private event WheelDisabled OnWheelDisabled;
    #endregion

}


#region wheel activity
class WheelActivityManager
{
    private bool isEnabled;

    private GameInputManager GameInputManager;

    public WheelActivityManager(GameInputManager gameInputManager)
    {
        GameInputManager = gameInputManager;
    }

    public bool IsEnabled { get => isEnabled; }

    public void AwakeWheel()
    {
        isEnabled = true;
        Debug.Log("Wheel enabled");
    }

    public void SleepWheel()
    {
        isEnabled = false;
        Debug.Log("Wheel disabled");
    }

    public bool TickCancelInput()
    {
        return GameInputManager.CurrentInput.CancelButtonDH();
    }

}
#endregion