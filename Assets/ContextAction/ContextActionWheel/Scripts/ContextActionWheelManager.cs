using System.Collections;
using UnityEngine;

public class ContextActionWheelManager : MonoBehaviour
{

    public WheelPositionManagerComponent WheelPositionManagerComponent;

    private ContextActionManager ContextActionManager;

    #region External dependecies
    private ContextActionWheel ContextActionWheel;
    private PlayerManager PlayerManager;
    #endregion
    private WheelActivityManager WheelActivityManager;
    private WheelActionActivationManager WheelActionActivationManager;
    private WheelPositionManager WheelPositionManager;

    private void Start()
    {
        #region External Dependencies
        ContextActionManager = GameObject.FindObjectOfType<ContextActionManager>();
        ContextActionWheel = GameObject.FindObjectOfType<ContextActionWheel>();
        PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
        GameInputManager GameInputManager = GameObject.FindObjectOfType<GameInputManager>();
        #endregion

        WheelActivityManager = new WheelActivityManager(GameInputManager);
        WheelActionActivationManager = new WheelActionActivationManager(GameInputManager, ContextActionWheel);
        WheelPositionManager = new WheelPositionManager(GameObject.FindGameObjectWithTag(TagConstants.PLAYER_TAG).transform, WheelPositionManagerComponent, ContextActionWheel.transform);
    }

    public void Tick(float d)
    {
        if (WheelActivityManager.IsEnabled)
        {

            WheelPositionManager.Tick();

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

    private IEnumerator TriggerContextAction(AContextAction contextAction)
    {
        yield return new WaitForEndOfFrame();
        SleepWheel();
        if (contextAction.GetType() == typeof(DummyContextAction))
        {
            var dummyInput = new DummyContextActionInput("TEST");
            ContextActionManager.AddAction(contextAction, dummyInput);
        }
        else if (contextAction.GetType() == typeof(GrabAction))
        {
            var grabInput = new GrabActionInput(PlayerManager.GetPlayerAnimator(),
                AnimationConstants.PlayerAnimationConstants[PlayerAnimatioNnamesEnum.PLAYER_ACTION_GRAB_DOWN].AnimationName,
                AnimationConstants.PlayerAnimationConstants[PlayerAnimatioNnamesEnum.PLAYER_ACTION_GRAB_DOWN].LayerIndex,
                contextAction.AttachedPointOfInterest.Item);
            ContextActionManager.AddAction(contextAction, grabInput);
        }

    }

    #region External Events
    public void AwakeWheel(WheelDisabled onWheelDisabledCallback, PointOfInterestType triggeredPOI)
    {
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

class WheelPositionManager
{
    private Transform PlayerTransform;
    private WheelPositionManagerComponent WheelPositionManagerComponent;
    private Transform ContextActionWheelTransform;

    public WheelPositionManager(Transform playerTransform, WheelPositionManagerComponent wheelPositionManagerComponent, Transform contextActionWheelTransform)
    {
        PlayerTransform = playerTransform;
        WheelPositionManagerComponent = wheelPositionManagerComponent;
        ContextActionWheelTransform = contextActionWheelTransform;
    }

    public void Tick()
    {
        ContextActionWheelTransform.position = Camera.main.WorldToScreenPoint(PlayerTransform.position + new Vector3(0, WheelPositionManagerComponent.UpDistanceFromPlayer, 0));
    }
}

[System.Serializable]
public class WheelPositionManagerComponent
{
    public float UpDistanceFromPlayer;
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
    }

    public void SleepWheel()
    {
        isEnabled = false;
    }

    public bool TickCancelInput()
    {
        return GameInputManager.CurrentInput.CancelButtonDH();
    }

}
#endregion