using CoreGame;
using RTPuzzle;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveObjectTest
{
    public class PlayerInteractiveObject : CoreInteractiveObject
    {
        public PlayerInteractiveObjectInitializerData PlayerInteractiveObjectInitializerData { get; private set; }

        #region External Dependencies
        private PlayerActionManager PlayerActionManager;
        [VE_Nested]
        private BlockingCutscenePlayerManager BlockingCutscenePlayer;
        #endregion

        [VE_Ignore]
        private PlayerInputMoveManager PlayerInputMoveManager;
        [VE_Ignore]
        private PlayerBodyPhysicsEnvironment PlayerBodyPhysicsEnvironment;
        [VE_Ignore]
        private PlayerSelectionWheelManager PlayerSelectionWheelManager;
        [VE_Ignore]
        private LevelResetManager LevelResetManager;
        [VE_Ignore]
        private LevelDependenatPlayerActionsManager LevelDependenatPlayerActionsManager;

        #region Systems
        [VE_Ignore]
        private AnimationObjectSystem AnimationObjectSystem;
        #endregion

        public PlayerInteractiveObject(InteractiveGameObject interactiveGameObject, InteractiveObjectLogicCollider InteractiveObjectLogicCollider) : base(interactiveGameObject, false)
        {
            interactiveGameObject.CreateLogicCollider(InteractiveObjectLogicCollider);
            this.interactiveObjectTag = new InteractiveObjectTag { IsPlayer = true };

            this.PlayerInteractiveObjectInitializerData = PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.PuzzleGlobalStaticConfiguration.PlayerInteractiveObjectInitializerData;

            #region External Dependencies
            this.PlayerActionManager = PlayerActionManager.Get();
            var PlayerActionEventManager = PuzzleGameSingletonInstances.PlayerActionEventManager;
            var PuzzleEventsManager = PuzzleGameSingletonInstances.PuzzleEventsManager;
            var coreConfigurationManager = CoreGameSingletonInstances.CoreConfigurationManager;
            this.BlockingCutscenePlayer = PuzzleGameSingletonInstances.BlockingCutscenePlayer;
            var GameInputManager = CoreGameSingletonInstances.GameInputManager;
            var LevelConfiguration = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager.PuzzleGameConfiguration.LevelConfiguration;
            var LevelManager = CoreGameSingletonInstances.LevelManager;
            #endregion

            this.AnimationObjectSystem = new AnimationObjectSystem(this);

            var cameraPivotPoint = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG);

            this.PlayerInputMoveManager = new PlayerInputMoveManager(PlayerInteractiveObjectInitializerData.SpeedMultiplicationFactor, cameraPivotPoint.transform, GameInputManager, interactiveGameObject.PhysicsRigidbody);
            this.PlayerBodyPhysicsEnvironment = new PlayerBodyPhysicsEnvironment(interactiveGameObject.PhysicsRigidbody, interactiveGameObject.GetLogicCollider(), PlayerInteractiveObjectInitializerData.MinimumDistanceToStick);
            this.PlayerSelectionWheelManager = new PlayerSelectionWheelManager(GameInputManager, PuzzleEventsManager, this.PlayerActionManager);
            this.LevelResetManager = new LevelResetManager(GameInputManager, PuzzleEventsManager);
            this.LevelDependenatPlayerActionsManager = new LevelDependenatPlayerActionsManager(this, LevelConfiguration, LevelManager);

            this.AfterConstructor();
        }

        public override void TickAlways(float d)
        {
            if (!LevelResetManager.Tick(d))
            {

                if (!PlayerActionManager.IsActionExecuting() && !this.BlockingCutscenePlayer.Playing)
                {
                    if (!PlayerSelectionWheelManager.AwakeOrSleepWheel(this.LevelDependenatPlayerActionsManager))
                    {
                        if (!PlayerActionManager.IsWheelEnabled())
                        {
                            PlayerInputMoveManager.Tick(d);
                        }
                        else
                        {
                            PlayerInputMoveManager.ResetSpeed();
                            PlayerSelectionWheelManager.TriggerActionOnInput(this.LevelDependenatPlayerActionsManager);
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

    #region Player Action Managers
    class LevelDependenatPlayerActionsManager
    {
        private PlayerInteractiveObject PlayerInteractiveObjectRef;
        private LevelConfigurationData CurrentLevelConfigurationData;
        private List<RTPPlayerAction> PlayerActionsAssociatedToLevel;

        public LevelDependenatPlayerActionsManager(PlayerInteractiveObject PlayerInteractiveObjectRef, LevelConfiguration levelConfiguration, LevelManager LevelManager)
        {
            this.PlayerInteractiveObjectRef = PlayerInteractiveObjectRef;
            this.PlayerActionsAssociatedToLevel = new List<RTPPlayerAction>();
            this.CurrentLevelConfigurationData = levelConfiguration.ConfigurationInherentData[LevelManager.LevelID];
        }

        public List<RTPPlayerAction> GetPlayerActionsAssociatedToLevel()
        {
            if (this.PlayerActionsAssociatedToLevel.Count == 0)
            {
                foreach (var levelStaticPlayerActionInherentData in this.CurrentLevelConfigurationData.ConfiguredPlayerActions)
                {
                    this.PlayerActionsAssociatedToLevel.Add(levelStaticPlayerActionInherentData.BuildPlayerAction(this.PlayerInteractiveObjectRef));
                }
            }
            return this.PlayerActionsAssociatedToLevel;
        }
    }

    class PlayerSelectionWheelManager
    {
        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        private IGameInputManager GameInputManager;
        private PlayerActionManager PlayerActionManager;

        private LevelDependenatPlayerActionsManager LevelDependenatPlayerActionsManagerRef;

        public PlayerSelectionWheelManager(IGameInputManager gameInputManager, PuzzleEventsManager PuzzleEventsManager, PlayerActionManager PlayerActionManager)
        {
            GameInputManager = gameInputManager;
            this.PuzzleEventsManager = PuzzleEventsManager;
            this.PlayerActionManager = PlayerActionManager;
        }

        public bool AwakeOrSleepWheel(LevelDependenatPlayerActionsManager LevelDependenatPlayerActionsManager)
        {
            if (!PlayerActionManager.IsWheelEnabled())
            {
                if (GameInputManager.CurrentInput.ActionButtonD())
                {
                    PlayerActionManager.AddActionsToAvailable(LevelDependenatPlayerActionsManager.GetPlayerActionsAssociatedToLevel());
                    this.PuzzleEventsManager.PZ_EVT_OnPlayerActionWheelAwake();
                    return true;
                }
            }
            else if (GameInputManager.CurrentInput.CancelButtonD())
            {
                this.PuzzleEventsManager.PZ_EVT_OnPlayerActionWheelSleep();
                PlayerActionManager.RemoveActionsToAvailable(LevelDependenatPlayerActionsManager.GetPlayerActionsAssociatedToLevel());
                return true;
            }
            return false;
        }

        public void TriggerActionOnInput(LevelDependenatPlayerActionsManager LevelDependenatPlayerActionsManager)
        {
            if (GameInputManager.CurrentInput.ActionButtonD())
            {
                this.PuzzleEventsManager.PZ_EVT_OnPlayerActionWheelNodeSelected();
                PlayerActionManager.RemoveActionsToAvailable(LevelDependenatPlayerActionsManager.GetPlayerActionsAssociatedToLevel());
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