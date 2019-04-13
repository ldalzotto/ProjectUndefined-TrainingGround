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
        public BodyGroundStickContactDistance BodyGroundStickContactDistance;
        public PlayerHairStrandAnimationManagerComponent PlayerHairStrandAnimationManagerComponent;
        public PlayerHoodAnimationManagerComponent PlayerHoodAnimationManagerComponent;
        public PlayerJacketCordAnimationManagerComponent PlayerJacketCordAnimationManagerComponent;

        private PlayerInputMoveManager PlayerInputMoveManager;
        private PlayerBodyPhysicsEnvironment PlayerBodyPhysicsEnvironment;
        private PlayerSelectionWheelManager PlayerSelectionWheelManager;

        #region Procedural Animation Managers
        private AnimationPositionTrackerManager HairObjectAnimationTracker;
        private AnimationPositionTrackerManager ChestObjectAnimationTracker;

        private PlayerAnimationDataManager PlayerAnimationDataManager;
        private PlayerHairStrandAnimationManager PlayerHairStrandAnimationManager;
        private PlayerHoodAnimationManager PlayerHoodAnimationManager;
        private PlayerJacketCordAnimationManager PlayerJacketCordAnimationManager;
        #endregion
        public void Init()
        {
            #region External Dependencies
            PlayerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
            var PlayerActionEventManager = GameObject.FindObjectOfType<PlayerActionEventManager>();
            #endregion

            var playerRigidBody = GetComponent<Rigidbody>();
            var gameInputManager = GameObject.FindObjectOfType<GameInputManager>();
            var animator = GetComponentInChildren<Animator>();

            var cameraPivotPoint = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG);
            PlayerInputMoveManager = new PlayerInputMoveManager(PlayerInputMoveManagerComponent, cameraPivotPoint.transform, gameInputManager, playerRigidBody);
            PlayerBodyPhysicsEnvironment = new PlayerBodyPhysicsEnvironment(playerRigidBody, BodyGroundStickContactDistance);
            PlayerSelectionWheelManager = new PlayerSelectionWheelManager(gameInputManager, PlayerActionEventManager, PlayerActionManager);
            PlayerAnimationDataManager = new PlayerAnimationDataManager(animator);

            var hairObject = playerRigidBody.gameObject.FindChildObjectRecursively(AnimationConstants.PlayerAnimationConstantsData.HAIR_OBJECT_NAME);
            var chestObject = PlayerBoneRetriever.GetPlayerBone(PlayerBone.CHEST, animator);

            HairObjectAnimationTracker = new AnimationPositionTrackerManager(hairObject);
            ChestObjectAnimationTracker = new AnimationPositionTrackerManager(chestObject);

            PlayerHairStrandAnimationManager = new PlayerHairStrandAnimationManager(HairObjectAnimationTracker, hairObject, PlayerHairStrandAnimationManagerComponent);
            PlayerHoodAnimationManager = new PlayerHoodAnimationManager(PlayerHoodAnimationManagerComponent, PlayerBoneRetriever.GetPlayerBone(PlayerBone.HOOD, animator).transform, playerRigidBody, PlayerInputMoveManagerComponent);
            PlayerJacketCordAnimationManager = new PlayerJacketCordAnimationManager(animator, ChestObjectAnimationTracker, PlayerJacketCordAnimationManagerComponent, PlayerInputMoveManagerComponent);
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
            PlayerInputMoveManager.FixedTick(d);
            PlayerBodyPhysicsEnvironment.FixedTick(d);

            #region Trackers Update
            HairObjectAnimationTracker.FixedTick(d);
            ChestObjectAnimationTracker.FixedTick(d);
            #endregion
        }

        public void LateTick(float d)
        {
            PlayerHairStrandAnimationManager.LateTick(d);
            PlayerHoodAnimationManager.LateTick(d);
            PlayerJacketCordAnimationManager.LateTick(d);
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

