using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    public interface ISequencedActionDrawable
    {
#if UNITY_EDITOR
        void ActionGUI();
#endif
    }

    [System.Serializable]
    public abstract class SequencedAction : ISequencedActionDrawable
    {
        public abstract void FirstExecutionAction(SequencedActionInput ContextActionInput);
        public abstract bool ComputeFinishedConditions();
        public abstract void AfterFinishedEventProcessed();
        public abstract void Tick(float d);

        [SerializeField]
        private List<SequencedAction> nextActions;

        #region Internal Dependencies
        [NonSerialized]
        private SequencedActionInput actionInput;
        #endregion

        public SequencedAction(List<SequencedAction> nextActions)
        {
            this.nextActions = nextActions;
        }

        public void OnTick(float d, Action<SequencedAction, SequencedActionInput> onActionFinished)
        {
            if (!isFinished)
            {
                Tick(d);

                if (ComputeFinishedConditions())
                {
                    isFinished = true;
                    if (onActionFinished != null)
                    {
                        onActionFinished.Invoke(this, actionInput);
                    }
                    AfterFinishedEventProcessed();
                }
            }
        }

        public virtual void Interupt()
        {
            this.isFinished = true;
        }

        private bool isFinished;

        public SequencedActionInput ActionInput { get => actionInput; set => actionInput = value; }
        public List<SequencedAction> NextActions { get => nextActions; }

        public void SetNextContextAction(List<SequencedAction> nextActions)
        {
            this.nextActions = nextActions;
        }

        public bool IsFinished()
        {
            return isFinished;
        }

        public void ResetState()
        {
            isFinished = false;
        }

#if UNITY_EDITOR
        public virtual void ActionGUI() { }
#endif

    }

    [System.Serializable]
    public abstract class SequencedActionInput
    {

    }
}
