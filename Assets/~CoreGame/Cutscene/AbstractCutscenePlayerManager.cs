using GameConfigurationID;
using System;
using System.Collections;
using UnityEngine;

namespace CoreGame
{
    public abstract class AbstractCutscenePlayerManager : SequencedActionManager
    {
        private SequencedActionInput currentInput;
        protected override Action<SequencedAction> OnActionAdd
        {
            get
            {
                return (SequencedAction SequencedAction) =>
                {
                    this.OnAddAction(SequencedAction, this.currentInput);
                };
            }
        }

        protected override Action<SequencedAction, SequencedActionInput> OnActionFinished => null;

        private Action OnCutsceneStart;
        private Action OnCutsceneEnd;

        protected void BaseInit(Action OnCutsceneStart, Action OnCutsceneEnd)
        {
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
            this.OnAddAction(cutsceneGraph.GetRootAction(), this.currentInput);
            yield return new WaitUntil(() => { return !this.isCutscenePlaying; });
            if(this.OnCutsceneEnd != null) { this.OnCutsceneEnd.Invoke(); }
        }

        protected override void OnNoMoreActionToPlay()
        {
            this.isCutscenePlaying = false;
        }
    }

}
