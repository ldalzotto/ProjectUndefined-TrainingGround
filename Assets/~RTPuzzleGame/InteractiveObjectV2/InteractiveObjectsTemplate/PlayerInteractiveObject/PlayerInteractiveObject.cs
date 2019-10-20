using System.Collections.Generic;
using CoreGame;
using RTPuzzle;
using SelectionWheel;
using UnityEngine;

namespace InteractiveObjects
{
    public class PlayerInteractiveObject : CoreInteractiveObject
    {
        #region Systems

        [VE_Nested] private AnimationObjectSystem AnimationObjectSystem;

        #endregion

        [VE_Ignore] private LevelDependenatPlayerActionsManager LevelDependenatPlayerActionsManager;
        [VE_Ignore] private LevelResetManager LevelResetManager;
        [VE_Ignore] private PlayerBodyPhysicsEnvironment PlayerBodyPhysicsEnvironment;

        [VE_Ignore] private PlayerInputMoveManager PlayerInputMoveManager;
        [VE_Ignore] private PlayerSelectionWheelManager PlayerSelectionWheelManager;

        public PlayerInteractiveObject(IInteractiveGameObject interactiveGameObject, InteractiveObjectLogicCollider InteractiveObjectLogicCollider) : base(interactiveGameObject, false)
        {
            interactiveGameObject.CreateLogicCollider(InteractiveObjectLogicCollider);
            interactiveObjectTag = new InteractiveObjectTag {IsPlayer = true};

            PlayerInteractiveObjectInitializerData = PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.PuzzleGlobalStaticConfiguration.PlayerInteractiveObjectInitializerData;

            #region External Dependencies

            PlayerActionManager = PlayerActionManager.Get();
            SelectionWheelObject = SelectionWheelObject.Get();
            var puzzleEventsManager = PuzzleEventsManager.Get();
            BlockingCutscenePlayer = PuzzleGameSingletonInstances.BlockingCutscenePlayer;
            var GameInputManager = CoreGameSingletonInstances.GameInputManager;
            var LevelConfiguration = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager.PuzzleGameConfiguration.LevelConfiguration;
            var LevelManager = CoreGameSingletonInstances.LevelManager;

            #endregion

            AnimationObjectSystem = new AnimationObjectSystem(this);

            var cameraPivotPoint = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG);

            PlayerInputMoveManager = new PlayerInputMoveManager(PlayerInteractiveObjectInitializerData.SpeedMultiplicationFactor, cameraPivotPoint.transform, GameInputManager, interactiveGameObject.PhysicsRigidbody);
            PlayerBodyPhysicsEnvironment = new PlayerBodyPhysicsEnvironment(interactiveGameObject.PhysicsRigidbody, interactiveGameObject.LogicCollider, PlayerInteractiveObjectInitializerData.MinimumDistanceToStick);
            PlayerSelectionWheelManager = new PlayerSelectionWheelManager(GameInputManager, puzzleEventsManager, PlayerActionManager, SelectionWheelObject);
            LevelResetManager = new LevelResetManager(GameInputManager, puzzleEventsManager);
            LevelDependenatPlayerActionsManager = new LevelDependenatPlayerActionsManager(this, LevelConfiguration, LevelManager);

            AfterConstructor();
        }

        public PlayerInteractiveObjectInitializerData PlayerInteractiveObjectInitializerData { get; private set; }

        public override void Tick(float d)
        {
            if (!LevelResetManager.Tick(d))
            {
                if (!PlayerActionManager.IsActionExecuting() && !BlockingCutscenePlayer.Playing)
                {
                    if (!PlayerSelectionWheelManager.AwakeOrSleepWheel(LevelDependenatPlayerActionsManager))
                    {
                        if (!SelectionWheelObject.IsWheelEnabled)
                        {
                            PlayerInputMoveManager.Tick(d);
                        }
                        else
                        {
                            PlayerInputMoveManager.ResetSpeed();
                            PlayerSelectionWheelManager.TriggerActionOnInput(LevelDependenatPlayerActionsManager);
                        }
                    }
                }
                else
                {
                    PlayerInputMoveManager.ResetSpeed();
                }
            }

            AnimationObjectSystem.SetUnscaledSpeedMagnitude(GetNormalizedSpeed());
            AnimationObjectSystem.Tick(d);
        }

        public override void FixedTick(float d)
        {
            PlayerInputMoveManager.FixedTick(d);
            PlayerBodyPhysicsEnvironment.FixedTick(d);
        }

        #region External Dependencies

        private PlayerActionManager PlayerActionManager;
        [VE_Nested] private BlockingCutscenePlayerManager BlockingCutscenePlayer;
        private SelectionWheelObject SelectionWheelObject;

        #endregion

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

    internal class LevelDependenatPlayerActionsManager
    {
        private LevelConfigurationData CurrentLevelConfigurationData;
        private List<RTPPlayerAction> PlayerActionsAssociatedToLevel;
        private PlayerInteractiveObject PlayerInteractiveObjectRef;

        public LevelDependenatPlayerActionsManager(PlayerInteractiveObject PlayerInteractiveObjectRef, LevelConfiguration levelConfiguration, LevelManager LevelManager)
        {
            this.PlayerInteractiveObjectRef = PlayerInteractiveObjectRef;
            PlayerActionsAssociatedToLevel = new List<RTPPlayerAction>();
            CurrentLevelConfigurationData = levelConfiguration.ConfigurationInherentData[LevelManager.LevelID];
        }

        public List<RTPPlayerAction> GetPlayerActionsAssociatedToLevel()
        {
            if (PlayerActionsAssociatedToLevel.Count == 0)
                foreach (var levelStaticPlayerActionInherentData in CurrentLevelConfigurationData.ConfiguredPlayerActions)
                    PlayerActionsAssociatedToLevel.Add(levelStaticPlayerActionInherentData.BuildPlayerAction(PlayerInteractiveObjectRef));

            return PlayerActionsAssociatedToLevel;
        }
    }

    internal class PlayerSelectionWheelManager
    {
        private IGameInputManager GameInputManager;
        private LevelDependenatPlayerActionsManager LevelDependenatPlayerActionsManagerRef;
        private PlayerActionManager PlayerActionManager;

        #region External Dependencies

        private PuzzleEventsManager PuzzleEventsManager;

        #endregion

        private SelectionWheelObject SelectionWheelObject;

        public PlayerSelectionWheelManager(IGameInputManager gameInputManager, PuzzleEventsManager PuzzleEventsManager, PlayerActionManager PlayerActionManager, SelectionWheelObject SelectionWheelObject)
        {
            GameInputManager = gameInputManager;
            this.PuzzleEventsManager = PuzzleEventsManager;
            this.PlayerActionManager = PlayerActionManager;
            this.SelectionWheelObject = SelectionWheelObject;
        }

        public bool AwakeOrSleepWheel(LevelDependenatPlayerActionsManager LevelDependenatPlayerActionsManager)
        {
            if (!SelectionWheelObject.IsWheelEnabled)
            {
                if (GameInputManager.CurrentInput.ActionButtonD())
                {
                    PlayerActionManager.AddActionsToAvailable(LevelDependenatPlayerActionsManager.GetPlayerActionsAssociatedToLevel());
                    PuzzleEventsManager.PZ_EVT_OnPlayerActionWheelAwake();
                    return true;
                }
            }
            else if (GameInputManager.CurrentInput.CancelButtonD())
            {
                PuzzleEventsManager.PZ_EVT_OnPlayerActionWheelSleep();
                PlayerActionManager.RemoveActionsToAvailable(LevelDependenatPlayerActionsManager.GetPlayerActionsAssociatedToLevel());
                return true;
            }

            return false;
        }

        public void TriggerActionOnInput(LevelDependenatPlayerActionsManager LevelDependenatPlayerActionsManager)
        {
            if (GameInputManager.CurrentInput.ActionButtonD())
            {
                PuzzleEventsManager.PZ_EVT_OnPlayerActionWheelNodeSelected();
                PlayerActionManager.RemoveActionsToAvailable(LevelDependenatPlayerActionsManager.GetPlayerActionsAssociatedToLevel());
            }
        }
    }

    #endregion

    #region Level Reset Manager

    internal class LevelResetManager
    {
        private const float TIME_PUSHED_TO_RESET_S = 1f;

        private float currentTimePressed = 0f;
        private IGameInputManager GameInputManager;
        private PuzzleEventsManager PuzzleEventsManager;

        public LevelResetManager(IGameInputManager gameInputManager, PuzzleEventsManager puzzleEventsManager)
        {
            GameInputManager = gameInputManager;
            PuzzleEventsManager = puzzleEventsManager;
        }

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