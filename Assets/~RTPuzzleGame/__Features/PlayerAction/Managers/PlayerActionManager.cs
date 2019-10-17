using CoreGame;
using InteractiveObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public interface IPlayerActionManagerDataRetrieval
    {
        RTPPlayerAction GetCurrentSelectedAction();
        RTPPlayerAction GetCurrentRunningAction();
    }

    public interface IPlayerActionManagerEvent : SelectableObjectSelectionManagerEventListener<ISelectableModule>
    {
        void ExecuteAction(RTPPlayerAction rTPPlayerAction);
        void OnSelectionWheelAwake();
        void OnSelectionWheelSleep(bool destroyImmediate);
        void IncreaseOrAddActionsRemainingExecutionAmount(RTPPlayerAction RTPPlayerAction, int deltaRemaining);
        void AddActionToAvailable(RTPPlayerAction selectableObjectPlayerAction);
        void RemoveActionToAvailable(RTPPlayerAction selectableObjectPlayerAction);
    }

    public class PlayerActionManager : GameSingleton<PlayerActionManager>, IPlayerActionManagerEvent, IPlayerActionManagerDataRetrieval
    {
        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager = PuzzleEventsManager.Get();
        #endregion

        #region Internal Events
        private event Action OnPlayerActionFinishedEvent;
        #endregion

        private PlayerActionExecutionManager PlayerActionExecutionManager;
        private PlayerActionsAvailableManager PlayerActionsAvailableManager;
        private PLayerSelectionWheelManager PLayerSelectionWheelManager;
        private PlayerSelectioNWheelPositioner PlayerSelectioNWheelPositioner;

        private SelectionWheel SelectionWheel;

        public PlayerActionManager()
        {
            #region External Dependencies
            var puzzleGameConfigurationManager = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager;
            var puzzleStaticConfiguration = PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration;
            #endregion

            SelectionWheel = PuzzleGameSingletonInstances.PuzzleSelectionWheel;

            PlayerActionExecutionManager = new PlayerActionExecutionManager(() => this.OnPlayerActionFinishedEvent.Invoke());
            PlayerActionsAvailableManager = new PlayerActionsAvailableManager();
            PLayerSelectionWheelManager = new PLayerSelectionWheelManager(SelectionWheel, puzzleGameConfigurationManager);
            PlayerSelectioNWheelPositioner = new PlayerSelectioNWheelPositioner(
                puzzleStaticConfiguration.PuzzleGlobalStaticConfiguration.PlayerSelectioNWheelPositionerComponent,
                SelectionWheel, () => { return PlayerInteractiveObjectManager.Get().GetPlayerGameObject().InteractiveGameObjectParent.transform; }
            , Camera.main);

            #region Event Register
            this.OnPlayerActionFinishedEvent += this.OnPlayerActionFinished;
            #endregion
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
        
        public void ExecuteAction(RTPPlayerAction rTPPlayerAction)
        {
            PlayerActionExecutionManager.ExecuteAction(rTPPlayerAction);
        }
        private void OnPlayerActionFinished()
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

        public void IncreaseOrAddActionsRemainingExecutionAmount(RTPPlayerAction RTPPlayerAction, int deltaRemaining)
        {
            this.PlayerActionsAvailableManager.IncreaseOrAddActionsRemainingExecutionAmount(RTPPlayerAction, deltaRemaining);
        }

        public void AddActionToAvailable(RTPPlayerAction addedAction)
        {
            this.PlayerActionsAvailableManager.AddActionToAvailable(addedAction);
            if (this.PLayerSelectionWheelManager.WheelEnabled)
            {
                this.PuzzleEventsManager.PZ_EVT_OnPlayerActionWheelRefresh();
            }
        }
        public void AddActionsToAvailable(List<RTPPlayerAction> addedActions)
        {
            foreach (var addedAction in addedActions)
            {
                this.AddActionToAvailable(addedAction);
            }
        }

        public void RemoveActionToAvailable(RTPPlayerAction removedAction)
        {
            this.PlayerActionsAvailableManager.RemoveActionToAvailable(removedAction);
            if (this.PLayerSelectionWheelManager.WheelEnabled)
            {
                this.PuzzleEventsManager.PZ_EVT_OnPlayerActionWheelRefresh();
            }
        }

        public void RemoveActionsToAvailable(List<RTPPlayerAction> removedActions)
        {
            foreach (var removedAction in removedActions)
            {
                this.RemoveActionToAvailable(removedAction);
            }
        }

        public void OnSelectableObjectSelected(ISelectableModule SelectableObject)
        {
            this.AddActionToAvailable(SelectableObject.GetAssociatedPlayerAction());
        }
        public void OnSelectableObjectDeSelected(ISelectableModule SelectableObject)
        {
            this.RemoveActionToAvailable(SelectableObject.GetAssociatedPlayerAction());
        }

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
        #endregion
    }


    #region Action execution
    class PlayerActionExecutionManager
    {
        private Action TriggerOnPlayerActionFinichedEventAction;
        
        public PlayerActionExecutionManager(Action TriggerOnPlayerActionFinichedEventAction)
        {
            this.TriggerOnPlayerActionFinichedEventAction = TriggerOnPlayerActionFinichedEventAction;
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
                    this.TriggerOnPlayerActionFinichedEventAction.Invoke();
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

        private MultiValueDictionary<PlayerActionType, RTPPlayerAction> currentAvailableActions;

        public PlayerActionsAvailableManager()
        {
            this.currentAvailableActions = new MultiValueDictionary<PlayerActionType, RTPPlayerAction>();
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

        public void AddActionToAvailable(RTPPlayerAction rTPPlayerActionToAdd)
        {
            this.currentAvailableActions.MultiValueAdd(rTPPlayerActionToAdd.PlayerActionType, rTPPlayerActionToAdd);
        }
        public void RemoveActionToAvailable(RTPPlayerAction rTPPlayerActionToRemove)
        {
            this.currentAvailableActions.MultiValueRemove(rTPPlayerActionToRemove.PlayerActionType, rTPPlayerActionToRemove);
        }
        public void IncreaseOrAddActionsRemainingExecutionAmount(RTPPlayerAction RTPPlayerAction, int deltaRemaining)
        {
            if (RTPPlayerAction.PlayerActionType != PlayerActionType.UNCLASSIFIED)
            {
                this.currentAvailableActions.TryGetValue(RTPPlayerAction.PlayerActionType, out List<RTPPlayerAction> retrievedActions);
                if (retrievedActions != null && retrievedActions.Count > 0)
                {
                    foreach (var action in retrievedActions)
                    {
                        action.IncreaseActionRemainingExecutionAmount(deltaRemaining);
                    }
                }
                else //Wa add
                {
                    this.currentAvailableActions.MultiValueAdd(RTPPlayerAction.PlayerActionType, RTPPlayerAction);
                }

            }
            else //Wa add
            {
                this.currentAvailableActions.MultiValueAdd(RTPPlayerAction.PlayerActionType, RTPPlayerAction);
            }


        }

        public MultiValueDictionary<PlayerActionType, RTPPlayerAction> CurrentAvailableActions { get => currentAvailableActions; }
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
                availableActions.ConvertAll(rtpPlayerAction => new PlayerSelectionWheelNodeData(rtpPlayerAction) as SelectionWheelNodeData)
            );
            wheelEnabled = true;
        }
        public void OnWheelSleep(bool destroyImmediate)
        {
            SelectionWheel.Exit(destroyImmediate);
            wheelEnabled = false;
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
        private Func<Transform> playerTransformProvider;
        private Camera camera;

        public PlayerSelectioNWheelPositioner(PlayerSelectioNWheelPositionerComponent PlayerSelectioNWheelPositionerComponent, SelectionWheel selectionWheel, Func<Transform> playerTransformProvider, Camera camera)
        {
            this.PlayerSelectioNWheelPositionerComponent = PlayerSelectioNWheelPositionerComponent;
            SelectionWheel = selectionWheel;
            this.playerTransformProvider = playerTransformProvider;
            this.camera = camera;
        }

        public void Tick(float d)
        {
            SelectionWheel.transform.position = camera.WorldToScreenPoint(this.playerTransformProvider.Invoke().position) + PlayerSelectioNWheelPositionerComponent.DeltaDistanceFromTargetPoint;
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

        public override string NodeText => playerAction.GetDescriptionText();

        public override Sprite NodeSprite => playerAction.GetNodeIcon();
    }
    #endregion
}

