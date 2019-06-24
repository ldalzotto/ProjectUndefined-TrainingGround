using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{

    public class ContextActionManager : MonoBehaviour
    {

        #region External Dependencies
        private ContextActionEventManager ContextActionEventManager;
        #endregion

        private void Start()
        {
            ContextActionEventManager = GameObject.FindObjectOfType<ContextActionEventManager>();
        }

        private List<AContextAction> ExecutedContextActions = new List<AContextAction>();
        private List<AContextAction> CurrentNexContextActions = new List<AContextAction>();

        public void Tick(float d)
        {
            foreach (var contextAction in ExecutedContextActions)
            {
                ProcessTick(d, contextAction);
                if (contextAction.IsFinished())
                {
                    OnContextActionFinished(contextAction);
                    if (contextAction.NextContextAction != null)
                    {
                        CurrentNexContextActions.Add(contextAction.NextContextAction);
                    }
                }
            }

            if (CurrentNexContextActions.Count >= 0)
            {
                foreach (var nextContextAction in CurrentNexContextActions)
                {
                    ContextActionEventManager.OnContextActionAdd(nextContextAction);
                }
                CurrentNexContextActions.Clear();
            }
        }

        private void ProcessTick(float d, AContextAction contextAction)
        {
            contextAction.OnTick(d, ContextActionEventManager);
        }

        #region Internal Events
        private void OnContextActionFinished(AContextAction contextAction)
        {
            StartCoroutine(RemoveContextAction(contextAction));
        }

        IEnumerator RemoveContextAction(AContextAction contextAction)
        {
            yield return new WaitForEndOfFrame();
            ExecutedContextActions.Remove(contextAction);
            contextAction.ResetState();
        }
        #endregion

        #region External Events
        public void OnAddAction(AContextAction contextAction, AContextActionInput contextActionInput)
        {
            contextAction.ResetState();
            contextAction.ContextActionInput = contextActionInput;
            contextAction.FirstExecutionAction(contextActionInput);
            ExecutedContextActions.Add(contextAction);
            //first tick for removing at the same frame if necessary
            ProcessTick(0f, contextAction);
        }
        #endregion
    }
}