using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace CoreGame
{
    public abstract class SequencedActionManager : MonoBehaviour
    {

        private List<SequencedAction> ExecutedActions = new List<SequencedAction>();
        private List<SequencedAction> CurrentNexActions = new List<SequencedAction>();

        protected abstract Action<SequencedAction> OnActionAdd { get; }
        protected abstract Action<SequencedAction, SequencedActionInput> OnActionFinished { get;}

        public void Tick(float d)
        {
            foreach (var action in ExecutedActions)
            {
                ProcessTick(d, action);
                if (action.IsFinished())
                {
                    OnContextActionFinished(action);
                    if (action.NextAction != null)
                    {
                        CurrentNexActions.Add(action.NextAction);
                    }
                }
            }

            if (CurrentNexActions.Count >= 0)
            {
                foreach (var nextContextAction in CurrentNexActions)
                {
                    OnActionAdd.Invoke(nextContextAction);
                }
                CurrentNexActions.Clear();
            }
        }
        
        private void ProcessTick(float d, SequencedAction contextAction)
        {
            contextAction.OnTick(d, this.OnActionFinished);
        }

        #region Internal Events
        private void OnContextActionFinished(SequencedAction contextAction)
        {
            StartCoroutine(RemoveContextAction(contextAction));
        }

        IEnumerator RemoveContextAction(SequencedAction contextAction)
        {
            yield return new WaitForEndOfFrame();
            ExecutedActions.Remove(contextAction);
            contextAction.ResetState();
        }
        #endregion

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
        #endregion
    }
}