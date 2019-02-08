using CoreGame;
using UnityEngine;

public class RTPlayerManager : MonoBehaviour
{

    #region External Dependencies
    private RTPPlayerActionManager RTPPlayerActionManager;
    #endregion

    public PlayerInputMoveManagerComponent PlayerInputMoveManagerComponent;

    private PlayerInputMoveManager PlayerInputMoveManager;

    public void Init()
    {
        RTPPlayerActionManager = GameObject.FindObjectOfType<RTPPlayerActionManager>();

        var playerRigidBody = GetComponent<Rigidbody>();
        var gameInputManager = GameObject.FindObjectOfType<GameInputManager>();

        var cameraPivotPoint = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG);
        PlayerInputMoveManager = new PlayerInputMoveManager(PlayerInputMoveManagerComponent, cameraPivotPoint.transform, gameInputManager, playerRigidBody);
    }

    public void Tick(float d)
    {
        if (!RTPPlayerActionManager.IsActionExecuting())
        {
            PlayerInputMoveManager.Tick(d);
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
