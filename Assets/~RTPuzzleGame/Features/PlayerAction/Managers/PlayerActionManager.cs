using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public interface IPlayerActionManagerDataRetrieval
    {
        RTPPlayerAction GetCurrentSelectedAction();
        RTPPlayerAction GetCurrentRunningAction();
        MultiValueDictionary<PlayerActionId, RTPPlayerAction> GetCurrentAvailablePlayerActions();
    }

    public class PlayerActionManager : MonoBehaviour, IPlayerActionManagerEvent, IPlayerActionManagerDataRetrieval
    {
        #region External Dependencies
        private IPlayerActionManagerEventListener IPlayerActionManagerEventListener;
        #endregion

        public PlayerSelectioNWheelPositionerComponent PlayerSelectioNWheelPositionerComponent;

        private PlayerActionExecutionManager PlayerActionExecutionManager;
        private PlayerActionsAvailableManager PlayerActionsAvailableManager;
        private PLayerSelectionWheelManager PLayerSelectionWheelManager;
        private PlayerSelectioNWheelPositioner PlayerSelectioNWheelPositioner;

        private SelectionWheel SelectionWheel;

        public void Init()
        {
            #region External Dependencies
            this.IPlayerActionManagerEventListener = PuzzleGameSingletonInstances.PuzzleEventsManager;
            var PlayerActionEventManager = PuzzleGameSingletonInstances.PlayerActionEventManager;
            var PlayerManagerDataRetriever = PuzzleGameSingletonInstances.PlayerManagerDataRetriever;
            var puzzleGameConfigurationManager = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager;
            var levelManager = CoreGameSingletonInstances.LevelManager;
            #endregion

            SelectionWheel = PuzzleGameSingletonInstances.PuzzleSelectionWheel;

            PlayerActionExecutionManager = new PlayerActionExecutionManager(PlayerActionEventManager);
            PlayerActionsAvailableManager = new PlayerActionsAvailableManager(levelManager.GetCurrentLevel(), puzzleGameConfigurationManager);
            PLayerSelectionWheelManager = new PLayerSelectionWheelManager(SelectionWheel, puzzleGameConfigurationManager);
            PlayerSelectioNWheelPositioner = new PlayerSelectioNWheelPositioner(PlayerSelectioNWheelPositionerComponent, SelectionWheel, PlayerManagerDataRetriever.GetPlayerTransform(), Camera.main);
        }

        public void Tick(float d)
        {
            PlayerActionExecutionManager.Tick(d);
            if (PLayerSelectionWheelManager.WheelEnabled)
            {
                PLayerSelectionWheelManager.Tick(d);
                PlayerSelectioNWheelPositioner.Tick(d);
            }
        }

        public void TickWhenTimeFlows(float d, float timeAttenuation)
        {
            PlayerActionsAvailableManager.Tick(d, timeAttenuation);
        }

        public void LateTick(float d)
        {
            this.PlayerActionExecutionManager.LateTick(d);
        }

        public void GizmoTick()
        {
            PlayerActionExecutionManager.GizmoTick();
        }

        public void GUITick()
        {
            PlayerActionExecutionManager.GUITick();
        }

        #region IPlayerActionManagerEvent
        public void ExecuteAction(RTPPlayerAction rTPPlayerAction)
        {
            PlayerActionExecutionManager.ExecuteAction(rTPPlayerAction);
        }
        public void StopAction()
        {
            PlayerActionExecutionManager.StopAction();
        }
        public void OnSelectionWheelAwake()
        {
            PLayerSelectionWheelManager.OnWheelAwake(PlayerActionsAvailableManager.CurrentAvailableActions.MultiValueGetValues());
        }
        public void OnSelectionWheelSleep(bool destroyImmediate)
        {
            PLayerSelectionWheelManager.OnWheelSleep(destroyImmediate);
        }

        public void IncreaseOrAddActionsRemainingExecutionAmount(PlayerActionId playerActionId, int deltaRemaining)
        {
            this.PlayerActionsAvailableManager.IncreaseOrAddActionsRemainingExecutionAmount(playerActionId, deltaRemaining);
        }

        public void AddActionToAvailable(RTPPlayerAction addedAction)
        {
            this.PlayerActionsAvailableManager.AddActionToAvailable(PlayerActionId.NONE, addedAction);
            if (this.PLayerSelectionWheelManager.WheelEnabled)
            {
                this.IPlayerActionManagerEventListener.PZ_EVT_OnPlayerActionWheelRefresh();
            }
        }
        public void RemoveActionToAvailable(RTPPlayerAction removedAction)
        {
            this.PlayerActionsAvailableManager.RemoveActionToAvailable(PlayerActionId.NONE, removedAction);
        }

        public void OnSelectableObjectSelected(ISelectableModule SelectableObject)
        {
            this.AddActionToAvailable(SelectableObject.GetAssociatedPlayerAction());
        }
        public void OnSelectableObjectDeSelected(ISelectableModule SelectableObject)
        {
            this.RemoveActionToAvailable(SelectableObject.GetAssociatedPlayerAction());
        }
        #endregion

        #region Logical Conditions
        public bool IsActionExecuting()
        {
            return PlayerActionExecutionManager.IsActionExecuting;
        }
        public bool IsWheelEnabled()
        {
            return PLayerSelectionWheelManager.WheelEnabled;
        }
        #endregion

        #region Data Retrieval
        public RTPPlayerAction GetCurrentSelectedAction()
        {
            return (SelectionWheel.GetSelectedNodeData() as PlayerSelectionWheelNodeData).Data as RTPPlayerAction;
        }

        public RTPPlayerAction GetCurrentRunningAction()
        {
            if (!IsActionExecuting())
            {
                return null;
            }
            else
            {
                return PlayerActionExecutionManager.CurrentAction;
            }
        }

        public MultiValueDictionary<PlayerActionId, RTPPlayerAction> GetCurrentAvailablePlayerActions()
        {
            return PlayerActionsAvailableManager.CurrentAvailableActions;
        }

        #endregion
    }


    #region Action execution
    class PlayerActionExecutionManager
    {

        private PlayerActionEventManager PlayerActionEventManager;

        public PlayerActionExecutionManager(PlayerActionEventManager PlayerActionEventManager)
        {
            this.PlayerActionEventManager = PlayerActionEventManager;
        }

        private RTPPlayerAction currentAction;
        private bool isActionExecuting;

        public bool IsActionExecuting { get => isActionExecuting; }
        public RTPPlayerAction CurrentAction { get => currentAction; }

        public void Tick(float d)
        {
            if (currentAction != null)
            {
                if (currentAction.FinishedCondition())
                {
                    PlayerActionEventManager.OnRTPPlayerActionStop(currentAction);
                }
                else
                {
                    currentAction.Tick(d);
                }
            }
        }

        public void LateTick(float d)
        {
            if (currentAction != null)
            {
                currentAction.LateTick(d);
            }
        }

        public void GizmoTick()
        {
            if (currentAction != null)
            { currentAction.GizmoTick(); }
        }

        public void GUITick()
        {
            if (currentAction != null)
            {
                currentAction.GUITick();
            }
        }

        public void ExecuteAction(RTPPlayerAction PlayerAction)
        {
            currentAction = PlayerAction;
            isActionExecuting = true;
            currentAction.FirstExecution();
        }

        internal void StopAction()
        {
            currentAction = null;
            isActionExecuting = false;
        }
    }
    #endregion

    #region RTPPlayer actions availability
    class PlayerActionsAvailableManager
    {

        #region External Dependencies
        private PlayerActionConfiguration PlayerActionConfiguration;
        #endregion

        private MultiValueDictionary<PlayerActionId, RTPPlayerAction> currentAvailableActions;

        public PlayerActionsAvailableManager(LevelZonesID puzzleId, PuzzleGameConfigurationManager puzzleGameConfigurationManager)
        {
            this.currentAvailableActions = puzzleGameConfigurationManager.LevelConfiguration()[puzzleId].PlayerActions;
            this.PlayerActionConfiguration = puzzleGameConfigurationManager.PuzzleGameConfiguration.PlayerActionConfiguration;
        }

        public void Tick(float d, float timeAttenuation)
        {
            foreach (var availableAction in currentAvailableActions.MultiValueGetValues())
            {
                if (availableAction.IsOnCoolDown())
                {
                    availableAction.CoolDownTick(d * timeAttenuation);
                }
            }
        }

        public void AddActionToAvailable(PlayerActionId playerActionId, RTPPlayerAction rTPPlayerActionToAdd)
        {
            this.currentAvailableActions.MultiValueAdd(playerActionId, rTPPlayerActionToAdd);
        }
        public void RemoveActionToAvailable(PlayerActionId playerActionId, RTPPlayerAction rTPPlayerActionToRemove)
        {
            this.currentAvailableActions.MultiValueRemove(playerActionId, rTPPlayerActionToRemove);
        }
        public void IncreaseOrAddActionsRemainingExecutionAmount(PlayerActionId playerActionId, int deltaRemaining)
        {
            this.currentAvailableActions.TryGetValue(playerActionId, out List<RTPPlayerAction> retrievedActions);
            if (retrievedActions != null && retrievedActions.Count > 0)
            {
                foreach (var action in retrievedActions)
                {
                    action.IncreaseActionRemainingExecutionAmount(deltaRemaining);
                }
            }
            else //Wa add
            {
                this.currentAvailableActions.MultiValueAdd(playerActionId, this.PlayerActionConfiguration.ConfigurationInherentData[playerActionId].BuildPlayerAction());
            }

        }

        public MultiValueDictionary<PlayerActionId, RTPPlayerAction> CurrentAvailableActions { get => currentAvailableActions; }
    }
    #endregion

    #region Wheel Management
    class PLayerSelectionWheelManager
    {
        private SelectionWheel SelectionWheel;
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;

        public PLayerSelectionWheelManager(SelectionWheel selectionWheel, PuzzleGameConfigurationManager puzzleGameConfigurationManager)
        {
            SelectionWheel = selectionWheel;
            PuzzleGameConfigurationManager = puzzleGameConfigurationManager;
        }

        private bool wheelEnabled;

        public bool WheelEnabled { get => wheelEnabled; }

        public void Tick(float d)
        {
            SelectionWheel.Tick(d);
        }

        public void OnWheelAwake(List<RTPPlayerAction> availableActions)
        {
            SelectionWheel.Init(
                availableActions.ConvertAll(rtpPlayerAction => new PlayerSelectionWheelNodeData(rtpPlayerAction) as SelectionWheelNodeData),
                ResolveWheelNodeSpriteFromNodeData
            );
            wheelEnabled = true;
        }
        public void OnWheelSleep(bool destroyImmediate)
        {
            SelectionWheel.Exit(destroyImmediate);
            wheelEnabled = false;
        }

        private Sprite ResolveWheelNodeSpriteFromNodeData(SelectionWheelNodeData selectionWheelNodeData)
        {
            return PuzzleGameConfigurationManager.SelectionWheelNodeConfiguration()[(selectionWheelNodeData.Data as RTPPlayerAction).GetSelectionWheelConfigurationId()].WheelNodeIcon;
        }
    }

    [System.Serializable]
    public class PlayerSelectioNWheelPositionerComponent
    {
        public Vector3 DeltaDistanceFromTargetPoint;
    }

    class PlayerSelectioNWheelPositioner
    {
        private PlayerSelectioNWheelPositionerComponent PlayerSelectioNWheelPositionerComponent;
        private SelectionWheel SelectionWheel;
        private Transform PlayerTransform;
        private Camera camera;

        public PlayerSelectioNWheelPositioner(PlayerSelectioNWheelPositionerComponent PlayerSelectioNWheelPositionerComponent, SelectionWheel selectionWheel, Transform playerTransform, Camera camera)
        {
            this.PlayerSelectioNWheelPositionerComponent = PlayerSelectioNWheelPositionerComponent;
            SelectionWheel = selectionWheel;
            PlayerTransform = playerTransform;
            this.camera = camera;
        }

        public void Tick(float d)
        {
            SelectionWheel.transform.position = camera.WorldToScreenPoint(PlayerTransform.position) + PlayerSelectioNWheelPositionerComponent.DeltaDistanceFromTargetPoint;
        }
    }
    #endregion

    #region RTPPlayer action wheel node data
    public class PlayerSelectionWheelNodeData : SelectionWheelNodeData
    {

        private RTPPlayerAction playerAction;

        public PlayerSelectionWheelNodeData(RTPPlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override object Data => playerAction;

        public override bool IsOnCoolDown => playerAction.IsOnCoolDown();

        public override float GetRemainingCooldownTime => playerAction.GetCooldownRemainingTime();

        public override int GetRemainingExecutionAmount => playerAction.RemainingExecutionAmout;

        public override bool CanNodeBeExecuted => playerAction.CanBeExecuted();
    }
    #endregion
}

