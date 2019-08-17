using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    public abstract class AbstractCutscenePlayerManager : MonoBehaviour
    {
        private SequencedActionInput currentInput;

        private SequencedActionManager SequencedActionManager;

        private Action OnCutsceneStart;
        private Action OnCutsceneEnd;

        protected void BaseInit(Action OnCutsceneStart, Action OnCutsceneEnd)
        {
            this.SequencedActionManager = new SequencedActionManager((action) => this.SequencedActionManager.OnAddAction(action, this.currentInput), OnActionFinished: null, OnNoMoreActionToPlay: this.OnNoMoreActionToPlay);
            this.OnCutsceneStart = OnCutsceneStart;
            this.OnCutsceneEnd = OnCutsceneEnd;
        }

        #region state
        private bool isCutscenePlaying = false;
        public bool IsCutscenePlaying { get => isCutscenePlaying; }
        #endregion
        
        protected IEnumerator PlayCutscene(AbstractCutsceneGraph cutsceneGraph, SequencedActionInput cutsceneInput)
        {
            this.isCutscenePlaying = true;
            this.currentInput = cutsceneInput;
            if (this.OnCutsceneStart != null) { this.OnCutsceneStart.Invoke(); }
            this.OnActionsAdd(cutsceneGraph.GetRootActions());
            yield return new WaitUntil(() => { return !this.isCutscenePlaying; });
            if (this.OnCutsceneEnd != null) { this.OnCutsceneEnd.Invoke(); }
        }

        public void Tick(float d)
        {
            this.SequencedActionManager.Tick(d);
        }

        private void OnActionsAdd(List<SequencedAction> SequencedActions)
        {
            this.SequencedActionManager.OnAddActions(SequencedActions, this.currentInput);
        }

        private void OnNoMoreActionToPlay()
        {
            this.isCutscenePlaying = false;
        }
    }

}
