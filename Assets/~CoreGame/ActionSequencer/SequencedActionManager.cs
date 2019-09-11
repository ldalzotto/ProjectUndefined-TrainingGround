using System;
using System.Collections.Generic;

namespace CoreGame
{
    public class SequencedActionManager
    {

        private List<SequencedAction> ExecutedActions = new List<SequencedAction>();
        private List<SequencedAction> FinishedActions = new List<SequencedAction>();
        private List<SequencedAction> CurrentNexActions = new List<SequencedAction>();

        private Action<SequencedAction> OnActionAdd;
        private Action<SequencedAction, SequencedActionInput> OnActionFinished;
        private Action OnNoMoreActionToPlay;

        public SequencedActionManager(Action<SequencedAction> OnActionAdd, Action<SequencedAction, SequencedActionInput> OnActionFinished, Action OnNoMoreActionToPlay = null)
        {
            this.OnActionAdd = OnActionAdd;
            this.OnActionFinished = OnActionFinished;
            this.OnNoMoreActionToPlay = OnNoMoreActionToPlay;
        }

        public void Tick(float d, bool eventsTriggered = true)
        {
            foreach (var action in ExecutedActions)
            {
                ProcessTick(d, action);
                if (action.IsFinished())
                {
                    FinishedActions.Add(action);
                }
            }

            foreach (var finishedAction in FinishedActions)
            {
                if (finishedAction.NextActions != null)
                {
                    CurrentNexActions.AddRange(finishedAction.NextActions);
                }
                finishedAction.ResetState();
                ExecutedActions.Remove(finishedAction);
            }

            FinishedActions.Clear();

            if (CurrentNexActions.Count > 0)
            {
                foreach (var nextContextAction in CurrentNexActions)
                {
                    if (OnActionAdd != null)
                    {
                        OnActionAdd.Invoke(nextContextAction);
                    }
                }
                CurrentNexActions.Clear();

                //first tick for removing at the same frame if necessary
                this.Tick(0f, eventsTriggered: false);
            }

            if (eventsTriggered)
            {
                if (ExecutedActions.Count == 0)
                {
                    if (this.OnNoMoreActionToPlay != null)
                    {
                        this.OnNoMoreActionToPlay.Invoke();
                    }
                }
            }
        }

        private void ProcessTick(float d, SequencedAction contextAction)
        {
            contextAction.OnTick(d, this.OnActionFinished);
        }


        #region External Events
        public void OnAddAction(SequencedAction action, SequencedActionInput actionInput)
        {
            action.ResetState();
            action.ActionInput = actionInput;
            action.FirstExecutionAction(actionInput);
            ExecutedActions.Add(action);
            //first tick for removing at the same frame if necessary
            ProcessTick(0f, action);
        }
        public void OnAddActions(List<SequencedAction> actions, SequencedActionInput actionInput)
        {
            foreach (var action in actions)
            {
                this.OnAddAction(action, actionInput);
            }
        }
        public void InterruptAllActions()
        {
            foreach (var action in ExecutedActions)
            {
                action.Interupt();
            }
            this.ExecutedActions.Clear();
        }
        public void CleatAllActions()
        {
            this.ExecutedActions.Clear();
            this.FinishedActions.Clear();
            this.CurrentNexActions.Clear();
        }
        #endregion

        #region Data Retrieval
        public List<SequencedAction> GetCurrentActions(bool includeWorkflowNested = false)
        {
            if (includeWorkflowNested)
            {
                var returnActions = new List<SequencedAction>();
                foreach (var executedAction in this.ExecutedActions)
                {
                    if (executedAction.GetType() == typeof(BranchInfiniteLoopAction))
                    {
                        var BranchInfiniteLoopAction = (BranchInfiniteLoopAction)executedAction;
                        returnActions.AddRange(BranchInfiniteLoopAction.GetCurrentActions(includeWorkflowNested));
                    }
                    returnActions.Add(executedAction);
                }
                return returnActions;
            }
            else
            {
                return this.ExecutedActions;
            }
        }
        public bool IsPlaying()
        {
            return this.GetCurrentActions().Count > 0;
        }
        #endregion
    }
}