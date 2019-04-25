using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    public class PlayerManager : MonoBehaviour
    {

        #region External Dependencies
        private PlayerActionManager PlayerActionManager;
        #endregion

        public BodyGroundStickContactDistance BodyGroundStickContactDistance;

        #region Player Common component
        private PlayerCommonComponents PlayerCommonComponents;
        #endregion

        private PlayerInputMoveManager PlayerInputMoveManager;
        private PlayerBodyPhysicsEnvironment PlayerBodyPhysicsEnvironment;
        private PlayerSelectionWheelManager PlayerSelectionWheelManager;

        #region Animation Managers
        private PlayerAnimationDataManager PlayerAnimationDataManager;
        private PlayerProceduralAnimationsManager PlayerProceduralAnimationsManager;
        #endregion
        public void Init()
        {
            #region External Dependencies
            PlayerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
            var PlayerActionEventManager = GameObject.FindObjectOfType<PlayerActionEventManager>();
            #endregion

            var playerRigidBody = GetComponent<Rigidbody>();
            var playerCollider = GetComponent<Collider>();
            var gameInputManager = GameObject.FindObjectOfType<GameInputManager>();
            var animator = GetComponentInChildren<Animator>();

            var cameraPivotPoint = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG);
            this.PlayerCommonComponents = GetComponentInChildren<PlayerCommonComponents>();
            PlayerInputMoveManager = new PlayerInputMoveManager(this.PlayerCommonComponents.PlayerInputMoveManagerComponent, cameraPivotPoint.transform, gameInputManager, playerRigidBody);
            PlayerBodyPhysicsEnvironment = new PlayerBodyPhysicsEnvironment(playerRigidBody, playerCollider, BodyGroundStickContactDistance);
            PlayerSelectionWheelManager = new PlayerSelectionWheelManager(gameInputManager, PlayerActionEventManager, PlayerActionManager);
            PlayerProceduralAnimationsManager = new PlayerProceduralAnimationsManager(this.PlayerCommonComponents, animator, playerRigidBody);
            PlayerAnimationDataManager = new PlayerAnimationDataManager(animator);
        }

        public void Tick(float d)
        {
            PlayerBodyPhysicsEnvironment.Tick(d);

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
            else
            {
                PlayerInputMoveManager.ResetSpeed();
            }
            PlayerAnimationDataManager.Tick(PlayerInputMoveManager.PlayerSpeedMagnitude);
        }

        public void FixedTick(float d)
        {
            PlayerProceduralAnimationsManager.FickedTick(d);
            PlayerInputMoveManager.FixedTick(d);
            PlayerBodyPhysicsEnvironment.FixedTick(d);
        }

        public void LateTick(float d)
        {
            PlayerProceduralAnimationsManager.LateTick(d);
        }

        #region Logical Conditions
        public bool HasPlayerMovedThisFrame()
        {
            return PlayerInputMoveManager.HasMoved;
        }
        #endregion

        #region Data Retrieval
        public Animator GetPlayerAnimator()
        {
            return PlayerAnimationDataManager.Animator;
        }
        #endregion

        public float GetPlayerSpeedMagnitude()
        {
            return PlayerInputMoveManager.PlayerSpeedMagnitude;
        }
        public Collider GetPlayerCollider()
        {
            return GetComponentInChildren<Collider>();
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

