using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    public class PlayerManager : MonoBehaviour
    {

        #region External Dependencies
        private PlayerActionManager PlayerActionManager;
        #endregion

        public PlayerInputMoveManagerComponent PlayerInputMoveManagerComponent;

        private PlayerInputMoveManager PlayerInputMoveManager;
        private PlayerSelectionWheelManager PlayerSelectionWheelManager;

        public void Init()
        {
            #region External Dependencies
            PlayerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
            var PlayerActionEventManager = GameObject.FindObjectOfType<PlayerActionEventManager>();
            #endregion

            var playerRigidBody = GetComponent<Rigidbody>();
            var gameInputManager = GameObject.FindObjectOfType<GameInputManager>();

            var cameraPivotPoint = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG);
            PlayerInputMoveManager = new PlayerInputMoveManager(PlayerInputMoveManagerComponent, cameraPivotPoint.transform, gameInputManager, playerRigidBody);
            PlayerSelectionWheelManager = new PlayerSelectionWheelManager(gameInputManager, PlayerActionEventManager, PlayerActionManager);
        }

        public void Tick(float d)
        {
            if (!PlayerActionManager.IsActionExecuting())
            {
                if (!PlayerSelectionWheelManager.AwakeOrSleepWheel())
                {
                    if (!PlayerActionManager.IsWheelEnabled())
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
        private PlayerActionEventManager PlayerActionEventManager;
        private PlayerActionManager PlayerActionManager;

        public PlayerSelectionWheelManager(GameInputManager gameInputManager, PlayerActionEventManager PlayerActionEventManager, PlayerActionManager PlayerActionManager)
        {
            GameInputManager = gameInputManager;
            this.PlayerActionEventManager = PlayerActionEventManager;
            this.PlayerActionManager = PlayerActionManager;
        }

        public bool AwakeOrSleepWheel()
        {
            if (!PlayerActionManager.IsWheelEnabled())
            {
                if (GameInputManager.CurrentInput.ActionButtonD())
                {
                    PlayerActionEventManager.OnWheelAwake();
                    return true;
                }
            }
            else if (GameInputManager.CurrentInput.CancelButtonD())
            {
                PlayerActionEventManager.OnWheelSleep();
                return true;
            }
            return false;
        }

        public void TriggerActionOnInput()
        {
            if (GameInputManager.CurrentInput.ActionButtonD())
            {
                PlayerActionEventManager.OnCurrentNodeSelected();
            }
        }

    }
    #endregion
}

