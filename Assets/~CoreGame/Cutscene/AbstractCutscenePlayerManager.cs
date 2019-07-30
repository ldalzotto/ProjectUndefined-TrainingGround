using System;
using System.Collections;
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
            this.SequencedActionManager = new SequencedActionManager(OnActionAdd: this.OnActionAdd, OnActionFinished: null, OnNoMoreActionToPlay: this.OnNoMoreActionToPlay);
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
            this.OnActionAdd(cutsceneGraph.GetRootAction());
            yield return new WaitUntil(() => { return !this.isCutscenePlaying; });
            if (this.OnCutsceneEnd != null) { this.OnCutsceneEnd.Invoke(); }
        }

        public void Tick(float d)
        {
            this.SequencedActionManager.Tick(d);
        }

        private void OnActionAdd(SequencedAction SequencedAction)
        {
            this.SequencedActionManager.OnAddAction(SequencedAction, this.currentInput);
        }

        private void OnNoMoreActionToPlay()
        {
            this.isCutscenePlaying = false;
        }
    }

}
