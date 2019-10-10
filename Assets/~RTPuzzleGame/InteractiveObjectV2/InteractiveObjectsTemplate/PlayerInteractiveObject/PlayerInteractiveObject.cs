using CoreGame;
using RTPuzzle;
using UnityEngine;

namespace InteractiveObjectTest
{
    public class PlayerInteractiveObject : CoreInteractiveObject
    {
        public PlayerInteractiveObjectInitializerData PlayerInteractiveObjectInitializerData { get; private set; }

        #region External Dependencies
        private PlayerActionManager PlayerActionManager;
        private BlockingCutscenePlayerManager BlockingCutscenePlayer;
        #endregion

        private PlayerInputMoveManager PlayerInputMoveManager;
        private PlayerBodyPhysicsEnvironment PlayerBodyPhysicsEnvironment;
        private PlayerSelectionWheelManager PlayerSelectionWheelManager;
        private LevelResetManager LevelResetManager;

        #region Systems
        private AnimationObjectSystem AnimationObjectSystem;
        #endregion

        public PlayerInteractiveObject(InteractiveGameObject interactiveGameObject) : base(interactiveGameObject, false)
        {
            this.InteractiveObjectTag = new InteractiveObjectTag { IsPlayer = true };

            this.PlayerInteractiveObjectInitializerData = PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.PuzzleGlobalStaticConfiguration.PlayerInteractiveObjectInitializerData;

            #region External Dependencies
            this.PlayerActionManager = PuzzleGameSingletonInstances.PlayerActionManager;
            var PlayerActionEventManager = PuzzleGameSingletonInstances.PlayerActionEventManager;
            var PuzzleEventsManager = PuzzleGameSingletonInstances.PuzzleEventsManager;
            var coreConfigurationManager = CoreGameSingletonInstances.CoreConfigurationManager;
            this.BlockingCutscenePlayer = PuzzleGameSingletonInstances.BlockingCutscenePlayer;
            var GameInputManager = CoreGameSingletonInstances.GameInputManager;
            #endregion

            this.AnimationObjectSystem = new AnimationObjectSystem(this);

            var cameraPivotPoint = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG);

            this.PlayerInputMoveManager = new PlayerInputMoveManager(PlayerInteractiveObjectInitializerData.SpeedMultiplicationFactor, cameraPivotPoint.transform, GameInputManager, interactiveGameObject.Rigidbody);
            this.PlayerBodyPhysicsEnvironment = new PlayerBodyPhysicsEnvironment(interactiveGameObject.Rigidbody, interactiveGameObject.GetLogicCollider(), PlayerInteractiveObjectInitializerData.MinimumDistanceToStick);
            this.PlayerSelectionWheelManager = new PlayerSelectionWheelManager(GameInputManager, PuzzleEventsManager, this.PlayerActionManager);
            this.LevelResetManager = new LevelResetManager(GameInputManager, PuzzleEventsManager);
        }

        public override void TickAlways(float d)
        {
            if (!LevelResetManager.Tick(d))
            {

                if (!PlayerActionManager.IsActionExecuting() && !this.BlockingCutscenePlayer.Playing)
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
            }
            this.AnimationObjectSystem.SetUnscaledSpeedMagnitude(new AnimationObjectSetUnscaledSpeedMagnitudeEvent { UnscaledSpeedMagnitude = this.GetNormalizedSpeed() });
            this.AnimationObjectSystem.TickAlways(d);
        }

        public override void FixedTick(float d)
        {
            PlayerInputMoveManager.FixedTick(d);
            PlayerBodyPhysicsEnvironment.FixedTick(d);
        }

        #region Logical Conditions
        public bool HasPlayerMovedThisFrame()
        {
            return PlayerInputMoveManager.HasMoved;
        }
        public float GetNormalizedSpeed()
        {
            return PlayerInputMoveManager.PlayerSpeedMagnitude;
        }
        #endregion
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