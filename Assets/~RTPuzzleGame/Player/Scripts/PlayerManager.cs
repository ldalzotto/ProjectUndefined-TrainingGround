using CoreGame;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class PlayerManager : PlayerManagerType
    {

        #region External Dependencies
        private PlayerActionManager PlayerActionManager;
        private BlockingCutscenePlayer BlockingCutscenePlayer;
        #endregion

        #region Internal Components
        private Rigidbody playerRigidbody;
        private NavMeshAgent navMeshAgent;
        private RootPuzzleLogicCollider rootPuzzleLogicCollider;
        private InteractiveObjectType associatedInteractiveObject;
        #endregion

        private PlayerPhysicsMovementComponent playerPhysicsMovementComponent;

        #region Player Common component
        private PlayerCommonComponents PlayerCommonComponents;
        private DataComponentContainer PlayerDataComponentContainer;
        #endregion

        private PlayerInputMoveManager PlayerInputMoveManager;
        private PlayerBodyPhysicsEnvironment PlayerBodyPhysicsEnvironment;
        private PlayerSelectionWheelManager PlayerSelectionWheelManager;

        #region Level Reset Manager
        private LevelResetManager LevelResetManager;
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
            var coreConfigurationManager = GameObject.FindObjectOfType<CoreConfigurationManager>();
            this.BlockingCutscenePlayer = GameObject.FindObjectOfType<BlockingCutscenePlayer>();
            #endregion

            this.playerRigidbody = GetComponent<Rigidbody>();
            var animator = GetComponentInChildren<Animator>();
            this.navMeshAgent = GetComponent<NavMeshAgent>();
            this.rootPuzzleLogicCollider = GetComponentInChildren<RootPuzzleLogicCollider>();
            this.associatedInteractiveObject = GetComponent<InteractiveObjectType>();

            var cameraPivotPoint = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG);
            this.PlayerCommonComponents = GetComponentInChildren<PlayerCommonComponents>();
            this.PlayerDataComponentContainer = GetComponentInChildren<DataComponentContainer>();
            this.PlayerDataComponentContainer.Init();
            this.rootPuzzleLogicCollider.Init();

            #region Data Components
            var TransformMoveManagerComponentV2 = this.PlayerDataComponentContainer.GetDataComponent<TransformMoveManagerComponentV2>();
            this.playerPhysicsMovementComponent = this.PlayerDataComponentContainer.GetDataComponent<PlayerPhysicsMovementComponent>();
            #endregion

            PlayerInputMoveManager = new PlayerInputMoveManager(TransformMoveManagerComponentV2, cameraPivotPoint.transform, gameInputManager, this.playerRigidbody);
            PlayerBodyPhysicsEnvironment = new PlayerBodyPhysicsEnvironment(this.playerRigidbody, this.rootPuzzleLogicCollider.GetRootCollider(), playerPhysicsMovementComponent);
            PlayerSelectionWheelManager = new PlayerSelectionWheelManager(gameInputManager, PuzzleEventsManager, PlayerActionManager);
            PlayerProceduralAnimationsManager = new PlayerProceduralAnimationsManager(this.PlayerCommonComponents, TransformMoveManagerComponentV2, animator, this.playerRigidbody, coreConfigurationManager);
            PlayerAnimationDataManager = new PlayerAnimationDataManager(animator);
            LevelResetManager = new LevelResetManager(gameInputManager, PuzzleEventsManager);

            GenericAnimatorHelper.SetMovementLayer(animator, coreConfigurationManager.AnimationConfiguration(), LevelType.PUZZLE);
        }

        public void Tick(float d)
        {
            if (!LevelResetManager.Tick(d))
            {

                if (!PlayerActionManager.IsActionExecuting() && !this.IsPlayerDirectedByCutscene())
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
        public bool IsPlayerDirectedByCutscene()
        {
            return this.associatedInteractiveObject.GetModule<InteractiveObjectCutsceneControllerModule>().IsCutscenePlaying() || this.BlockingCutscenePlayer.Playing;
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
        public Collider PlayerPuzzleLogicRootCollier { get => this.rootPuzzleLogicCollider.GetRootCollider(); }
        public PlayerPhysicsMovementComponent PlayerPhysicsMovementComponent { get => playerPhysicsMovementComponent; }
        public NavMeshAgent NavMeshAgent { get => navMeshAgent; }
    }

    #region Player Action Selection Manager
    class PlayerSelectionWheelManager
    {
        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        private IGameInputManager GameInputManager;
        private PlayerActionManager PlayerActionManager;

        public PlayerSelectionWheelManager(IGameInputManager gameInputManager, PuzzleEventsManager PuzzleEventsManager, PlayerActionManager PlayerActionManager)
        {
            GameInputManager = gameInputManager;
            this.PuzzleEventsManager = PuzzleEventsManager;
            this.PlayerActionManager = PlayerActionManager;
        }

        public bool AwakeOrSleepWheel()
        {
            if (!PlayerActionManager.IsWheelEnabled())
            {
                if (GameInputManager.CurrentInput.ActionButtonD())
                {
                    this.PuzzleEventsManager.PZ_EVT_OnPlayerActionWheelAwake();
                    return true;
                }
            }
            else if (GameInputManager.CurrentInput.CancelButtonD())
            {
                this.PuzzleEventsManager.PZ_EVT_OnPlayerActionWheelSleep();
                return true;
            }
            return false;
        }

        public void TriggerActionOnInput()
        {
            if (GameInputManager.CurrentInput.ActionButtonD())
            {
                this.PuzzleEventsManager.PZ_EVT_OnPlayerActionWheelNodeSelected();
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

