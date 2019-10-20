using System;
using System.Collections.Generic;
using CoreGame;
using SelectableObjects_Interfaces;
using SelectionWheel;
using UnityEngine;

namespace RTPuzzle
{
    public interface IPlayerActionManagerDataRetrieval
    {
        RTPPlayerAction GetCurrentSelectedAction();
        RTPPlayerAction GetCurrentRunningAction();
    }

    public class PlayerActionManager : GameSingleton<PlayerActionManager>, IPlayerActionManagerDataRetrieval
    {
        private PlayerActionExecutionManager PlayerActionExecutionManager;
        private PlayerActionsAvailableManager PlayerActionsAvailableManager;

        #region External Dependencies

        private PuzzleEventsManager PuzzleEventsManager = PuzzleEventsManager.Get();

        #endregion

        private SelectionWheelObject SelectionWheel;

        #region Internal Events

        private event Action OnPlayerActionFinishedEvent;

        #endregion

        public void Init()
        {
            #region External Dependencies

            var puzzleGameConfigurationManager = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager;
            var puzzleStaticConfiguration = PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration;

            #endregion

            SelectionWheel = SelectionWheelObject.Get();

            PlayerActionExecutionManager = new PlayerActionExecutionManager(() => OnPlayerActionFinishedEvent.Invoke());
            PlayerActionsAvailableManager = new PlayerActionsAvailableManager();

            #region Event Register

            OnPlayerActionFinishedEvent += OnPlayerActionFinished;

            SelectableObjectEventsManager.Get().RegisterOnSelectableObjectSelectedEventAction(OnSelectableObjectSelected);
            SelectableObjectEventsManager.Get().RegisterOnSelectableObjectNoMoreSelectedEventAction(OnSelectableObjectDeSelected);

            #endregion
        }

        public void Tick(float d)
        {
            PlayerActionExecutionManager.Tick(d);
        }

        public void TickWhenTimeFlows(float d, float timeAttenuation)
        {
            PlayerActionsAvailableManager.Tick(d, timeAttenuation);
        }

        public void LateTick(float d)
        {
            PlayerActionExecutionManager.LateTick(d);
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

        public void IncreaseOrAddActionsRemainingExecutionAmount(RTPPlayerAction RTPPlayerAction, int deltaRemaining)
        {
            PlayerActionsAvailableManager.IncreaseOrAddActionsRemainingExecutionAmount(RTPPlayerAction, deltaRemaining);
        }

        public void AddActionToAvailable(RTPPlayerAction addedAction)
        {
            PlayerActionsAvailableManager.AddActionToAvailable(addedAction);
            PuzzleEventsManager.PZ_EVT_OnPlayerActionWheelRefresh();
        }

        public void AddActionsToAvailable(List<RTPPlayerAction> addedActions)
        {
            foreach (var addedAction in addedActions) AddActionToAvailable(addedAction);
        }

        public void RemoveActionToAvailable(RTPPlayerAction removedAction)
        {
            PlayerActionsAvailableManager.RemoveActionToAvailable(removedAction);
            PuzzleEventsManager.PZ_EVT_OnPlayerActionWheelRefresh();
        }

        public void RemoveActionsToAvailable(List<RTPPlayerAction> removedActions)
        {
            foreach (var removedAction in removedActions) RemoveActionToAvailable(removedAction);
        }

        private void OnSelectableObjectSelected(ISelectableObjectSystem SelectableObject)
        {
            AddActionToAvailable(SelectableObject.AssociatedPlayerAction as RTPPlayerAction);
        }

        private void OnSelectableObjectDeSelected(ISelectableObjectSystem SelectableObject)
        {
            RemoveActionToAvailable(SelectableObject.AssociatedPlayerAction as RTPPlayerAction);
        }

        #region Logical Conditions

        public bool IsActionExecuting()
        {
            return PlayerActionExecutionManager.IsActionExecuting;
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
                return null;
            else
                return PlayerActionExecutionManager.CurrentAction;
        }

        public List<RTPPlayerAction> GetCurrentAvailableActions()
        {
            return PlayerActionsAvailableManager.CurrentAvailableActions.MultiValueGetValues();
        }

        #endregion
    }


    #region Action execution

    internal class PlayerActionExecutionManager
    {
        private RTPPlayerAction currentAction;
        private bool isActionExecuting;
        private Action TriggerOnPlayerActionFinichedEventAction;

        public PlayerActionExecutionManager(Action TriggerOnPlayerActionFinichedEventAction)
        {
            this.TriggerOnPlayerActionFinichedEventAction = TriggerOnPlayerActionFinichedEventAction;
        }

        public bool IsActionExecuting => isActionExecuting;

        public RTPPlayerAction CurrentAction => currentAction;

        public void Tick(float d)
        {
            if (currentAction != null)
            {
                if (currentAction.FinishedCondition())
                    TriggerOnPlayerActionFinichedEventAction.Invoke();
                else
                    currentAction.Tick(d);
            }
        }

        public void LateTick(float d)
        {
            if (currentAction != null) currentAction.LateTick(d);
        }

        public void GizmoTick()
        {
            if (currentAction != null) currentAction.GizmoTick();
        }

        public void GUITick()
        {
            if (currentAction != null) currentAction.GUITick();
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

    internal class PlayerActionsAvailableManager
    {
        private MultiValueDictionary<PlayerActionType, RTPPlayerAction> currentAvailableActions;

        public PlayerActionsAvailableManager()
        {
            currentAvailableActions = new MultiValueDictionary<PlayerActionType, RTPPlayerAction>();
        }

        public MultiValueDictionary<PlayerActionType, RTPPlayerAction> CurrentAvailableActions => currentAvailableActions;

        public void Tick(float d, float timeAttenuation)
        {
            foreach (var availableAction in currentAvailableActions.MultiValueGetValues())
                if (availableAction.IsOnCoolDown())
                    availableAction.CoolDownTick(d * timeAttenuation);
        }

        public void AddActionToAvailable(RTPPlayerAction rTPPlayerActionToAdd)
        {
            currentAvailableActions.MultiValueAdd(rTPPlayerActionToAdd.PlayerActionType, rTPPlayerActionToAdd);
        }

        public void RemoveActionToAvailable(RTPPlayerAction rTPPlayerActionToRemove)
        {
            currentAvailableActions.MultiValueRemove(rTPPlayerActionToRemove.PlayerActionType, rTPPlayerActionToRemove);
        }

        public void IncreaseOrAddActionsRemainingExecutionAmount(RTPPlayerAction RTPPlayerAction, int deltaRemaining)
        {
            if (RTPPlayerAction.PlayerActionType != PlayerActionType.UNCLASSIFIED)
            {
                currentAvailableActions.TryGetValue(RTPPlayerAction.PlayerActionType, out var retrievedActions);
                if (retrievedActions != null && retrievedActions.Count > 0)
                    foreach (var action in retrievedActions)
                        action.IncreaseActionRemainingExecutionAmount(deltaRemaining);
                else //Wa add
                    currentAvailableActions.MultiValueAdd(RTPPlayerAction.PlayerActionType, RTPPlayerAction);
            }
            else //Wa add
            {
                currentAvailableActions.MultiValueAdd(RTPPlayerAction.PlayerActionType, RTPPlayerAction);
            }
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