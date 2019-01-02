using System.Collections;
using UnityEngine;

public class ContextActionWheelManager : MonoBehaviour
{
    private ContextActionManager ContextActionManager;

    #region External dependecies
    private ContextActionWheel ContextActionWheel;
    #endregion
    private WheelActivityManager WheelActivityManager;
    private WheelActionActivationManager WheelActionActivationManager;

    private void Start()
    {
        #region External Dependencies
        ContextActionManager = GameObject.FindObjectOfType<ContextActionManager>();
        ContextActionWheel = GameObject.FindObjectOfType<ContextActionWheel>();
        GameInputManager GameInputManager = GameObject.FindObjectOfType<GameInputManager>();
        #endregion

        WheelActivityManager = new WheelActivityManager(GameInputManager);
        WheelActionActivationManager = new WheelActionActivationManager(GameInputManager, ContextActionWheel);
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
                var triggeredContextAction = WheelActionActivationManager.Tick();
                if (triggeredContextAction != null)
                {
                    StartCoroutine(TriggerContextAction(triggeredContextAction));
                }
            }

            ContextActionWheel.Tick(d);
        }
    }

    //TODO getting the POI for data valuate
    private IEnumerator TriggerContextAction(AContextAction contextAction)
    {
        yield return new WaitForEndOfFrame();
        SleepWheel();
        if (contextAction.GetType() == typeof(DummyContextAction))
        {
            var dummyInput = new DummyContextActionInput("TEST");
            ContextActionManager.AddAction(contextAction, dummyInput);
        }
        else if (contextAction.GetType() == typeof(DummyGrabAction))
        {
            ContextActionManager.AddAction(contextAction, null);
        }

    }

    #region External Events
    public void AwakeWheel(WheelDisabled onWheelDisabledCallback, PointOfInterestType triggeredPOI)
    {
        Debug.Log(triggeredPOI.name);
        ContextActionWheel.Init(triggeredPOI.ContextActions);
        WheelActivityManager.AwakeWheel();
        OnWheelDisabled += onWheelDisabledCallback;
    }
    #endregion

    #region Internal Events
    public void SleepWheel()
    {
        ContextActionWheel.Exit();
        WheelActivityManager.SleepWheel();
        OnWheelDisabled.Invoke();
        OnWheelDisabled = null;
    }

    public delegate void WheelDisabled();
    private event WheelDisabled OnWheelDisabled;
    #endregion

}


class WheelActionActivationManager
{
    private GameInputManager GameInputManager;
    private ContextActionWheel ContextActionWheel;

    public WheelActionActivationManager(GameInputManager gameInputManager, ContextActionWheel contextActionWheel)
    {
        GameInputManager = gameInputManager;
        ContextActionWheel = contextActionWheel;
    }

    public AContextAction Tick()
    {
        if (GameInputManager.CurrentInput.ActionButtonD())
        {
            return ContextActionWheel.GetSelectedAction();
        }
        return null;
    }
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
        Debug.Log(Time.frameCount + " Wheel enabled");
    }

    public void SleepWheel()
    {
        isEnabled = false;
        Debug.Log(Time.frameCount + " Wheel disabled");
    }

    public bool TickCancelInput()
    {
        return GameInputManager.CurrentInput.CancelButtonDH();
    }

}
#endregion