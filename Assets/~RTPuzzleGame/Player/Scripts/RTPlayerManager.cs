using CoreGame;
using UnityEngine;

public class RTPlayerManager : MonoBehaviour
{

    #region External Dependencies
    private RTPPlayerActionManager RTPPlayerActionManager;
    #endregion

    public PlayerInputMoveManagerComponent PlayerInputMoveManagerComponent;

    private PlayerInputMoveManager PlayerInputMoveManager;
    private PlayerSelectionWheelManager PlayerSelectionWheelManager;

    public void Init()
    {
        #region External Dependencies
        RTPPlayerActionManager = GameObject.FindObjectOfType<RTPPlayerActionManager>();
        var RTPPlayerActionEventManager = GameObject.FindObjectOfType<RTPPlayerActionEventManager>();
        #endregion

        var playerRigidBody = GetComponent<Rigidbody>();
        var gameInputManager = GameObject.FindObjectOfType<GameInputManager>();

        var cameraPivotPoint = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG);
        PlayerInputMoveManager = new PlayerInputMoveManager(PlayerInputMoveManagerComponent, cameraPivotPoint.transform, gameInputManager, playerRigidBody);
        PlayerSelectionWheelManager = new PlayerSelectionWheelManager(gameInputManager, RTPPlayerActionEventManager, RTPPlayerActionManager);
    }

    public void Tick(float d)
    {
        if (!RTPPlayerActionManager.IsActionExecuting())
        {
            if (!PlayerSelectionWheelManager.AwakeOrSleepWheel())
            {
                if (!RTPPlayerActionManager.IsWheelEnabled())
                {
                    PlayerInputMoveManager.Tick(d);
                }
                else
                {
                    PlayerInputMoveManager.ResetSpeed();
                    PlayerSelectionWheelManager.TriggerActionOnInput();
                }
            }
        }
    }

    public void FixedTick(float d)
    {
        PlayerInputMoveManager.FixedTick(d);
    }

    #region Logical Conditions
    public bool HasPlayerMovedThisFrame()
    {
        return PlayerInputMoveManager.HasMoved;
    }
    #endregion

    public float GetPlayerSpeedMagnitude()
    {
        return PlayerInputMoveManager.PlayerSpeedMagnitude;
    }

}

#region Player Action Selection Manager
class PlayerSelectionWheelManager
{
    private GameInputManager GameInputManager;
    private RTPPlayerActionEventManager RTPPlayerActionEventManager;
    private RTPPlayerActionManager RTPPlayerActionManager;

    public PlayerSelectionWheelManager(GameInputManager gameInputManager, RTPPlayerActionEventManager rTPPlayerActionEventManager, RTPPlayerActionManager RTPPlayerActionManager)
    {
        GameInputManager = gameInputManager;
        RTPPlayerActionEventManager = rTPPlayerActionEventManager;
        this.RTPPlayerActionManager = RTPPlayerActionManager;
    }

    public bool AwakeOrSleepWheel()
    {
        if (!RTPPlayerActionManager.IsWheelEnabled())
        {
            if (GameInputManager.CurrentInput.ActionButtonD())
            {
                RTPPlayerActionEventManager.OnWheelAwake();
                return true;
            }
        }
        else if (GameInputManager.CurrentInput.CancelButtonD())
        {
            RTPPlayerActionEventManager.OnWheelSleep();
            return true;
        }
        return false;
    }

    public void TriggerActionOnInput()
    {
        if (GameInputManager.CurrentInput.ActionButtonD())
        {
            RTPPlayerActionEventManager.OnCurrentNodeSelected();
        }
    }

}
#endregion