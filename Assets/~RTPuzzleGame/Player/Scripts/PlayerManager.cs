using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    public class PlayerManager : MonoBehaviour
    {

        #region External Dependencies
        private PlayerActionManager PlayerActionManager;
        #endregion

        #region Internal Components
        private Rigidbody playerRigidbody;
        private Collider playerCollier;
        #endregion

        public BodyGroundStickContactDistance BodyGroundStickContactDistance;

        #region Player Common component
        private PlayerCommonComponents PlayerCommonComponents;
        #endregion

        private PlayerInputMoveManager PlayerInputMoveManager;
        private PlayerBodyPhysicsEnvironment PlayerBodyPhysicsEnvironment;
        private PlayerSelectionWheelManager PlayerSelectionWheelManager;

        #region Level Reset Manager
        private LevelResetManager LevelResetManager;
        #endregion

        #region Camera Managers
        private CameraFollowManager CameraFollowManager;
        private CameraOrientationManager CameraOrientationManager;
        #endregion

        #region Animation Managers
        private PlayerAnimationDataManager PlayerAnimationDataManager;
        private PlayerProceduralAnimationsManager PlayerProceduralAnimationsManager;
        #endregion

        public void Init(IGameInputManager gameInputManager)
        {
            #region External Dependencies
            PlayerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
            var PlayerActionEventManager = GameObject.FindObjectOfType<PlayerActionEventManager>();
            var PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            #endregion

            this.playerRigidbody = GetComponent<Rigidbody>();
            this.playerCollier = GetComponent<Collider>();
            var animator = GetComponentInChildren<Animator>();

            var cameraPivotPoint = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG);
            this.PlayerCommonComponents = GetComponentInChildren<PlayerCommonComponents>();
            PlayerInputMoveManager = new PlayerInputMoveManager(this.PlayerCommonComponents.PlayerInputMoveManagerComponent, cameraPivotPoint.transform, gameInputManager, this.playerRigidbody);
            PlayerBodyPhysicsEnvironment = new PlayerBodyPhysicsEnvironment(this.playerRigidbody, this.playerCollier, BodyGroundStickContactDistance);
            PlayerSelectionWheelManager = new PlayerSelectionWheelManager(gameInputManager, PlayerActionEventManager, PlayerActionManager);
            PlayerProceduralAnimationsManager = new PlayerProceduralAnimationsManager(this.PlayerCommonComponents, animator, this.playerRigidbody);
            PlayerAnimationDataManager = new PlayerAnimationDataManager(animator);
            LevelResetManager = new LevelResetManager(gameInputManager, PuzzleEventsManager);

            CameraFollowManager = new CameraFollowManager(this.playerRigidbody.transform, cameraPivotPoint.transform, PlayerCommonComponents.CameraFollowManagerComponent);
            CameraOrientationManager = new CameraOrientationManager(cameraPivotPoint.transform, gameInputManager, PlayerCommonComponents.CameraOrientationManagerComponent);
        }

        public void Tick(float d)
        {
            if (!LevelResetManager.Tick(d))
            {
                #region Camera
                CameraFollowManager.Tick(d);
                CameraOrientationManager.Tick(d);
                #endregion

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
        public bool IsCameraRotating()
        {
            return this.CameraOrientationManager.IsRotating;
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
        public Rigidbody PlayerRigidbody { get => playerRigidbody; }
        public Collider PlayerCollier { get => playerCollier; }

    }

    #region Player Action Selection Manager
    class PlayerSelectionWheelManager
    {
        private IGameInputManager GameInputManager;
        private PlayerActionEventManager PlayerActionEventManager;
        private PlayerActionManager PlayerActionManager;

        public PlayerSelectionWheelManager(IGameInputManager gameInputManager, PlayerActionEventManager PlayerActionEventManager, PlayerActionManager PlayerActionManager)
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

    #region Level Reset Manager
    class LevelResetManager
    {
        private const float TIME_PUSHED_TO_RESET_S = 1f;
        private IGameInputManager GameInputManager;
        private PuzzleEventsManager PuzzleEventsManager;

        public LevelResetManager(IGameInputManager gameInputManager, PuzzleEventsManager puzzleEventsManager)
        {
            GameInputManager = gameInputManager;
            PuzzleEventsManager = puzzleEventsManager;
        }

        private float currentTimePressed = 0f;

        public bool Tick(float d)
        {
            if (GameInputManager.CurrentInput.PuzzleResetButton())
            {
                currentTimePressed += d;
                if (currentTimePressed >= TIME_PUSHED_TO_RESET_S)
                {
                    currentTimePressed = 0f;
                    PuzzleEventsManager.PZ_EVT_LevelReseted();
                    return true;
                }
            }
            else
            {
                currentTimePressed = 0f;
            }
            return false;
        }
    }
    #endregion
}

