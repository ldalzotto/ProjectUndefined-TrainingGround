using CoreGame;
using System;
using UnityEngine;

namespace AdventureGame
{
    public class ContextActionManager : MonoBehaviour
    {
        #region External Dependencies
        private ContextActionEventManager ContextActionEventManager;
        #endregion

        private SequencedActionManager SequencedActionManager;

        private void Start()
        {
            ContextActionEventManager = AdventureGameSingletonInstances.ContextActionEventManager;

            this.SequencedActionManager = new SequencedActionManager(OnActionAdd: ContextActionEventManager.OnContextActionAdd, OnActionFinished: ContextActionEventManager.OnContextActionFinished);
        }

        public void Tick(float d)
        {
            this.SequencedActionManager.Tick(d);
        }

        #region External Events
        public void OnAddAction(SequencedAction action, SequencedActionInput actionInput)
        {
            this.SequencedActionManager.OnAddAction(action, actionInput);
        }
        #endregion
    }
}