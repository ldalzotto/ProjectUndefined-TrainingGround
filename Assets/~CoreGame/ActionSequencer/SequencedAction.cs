using UnityEngine;
using System.Collections;
using System;

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
        private SequencedAction nextAction;

        #region Internal Dependencies
        [NonSerialized]
        private SequencedActionInput actionInput;
        #endregion

        public SequencedAction(SequencedAction nextAction)
        {
            this.nextAction = nextAction;
        }

        public void OnTick(float d, Action<SequencedAction, SequencedActionInput> onActionFinished)
        {
            if (!isFinished)
            {
                Tick(d);

                if (ComputeFinishedConditions())
                {
                    isFinished = true;
                    onActionFinished.Invoke(this, actionInput);
                    AfterFinishedEventProcessed();
                }
            }
        }

        private bool isFinished;

        public SequencedActionInput ActionInput { get => actionInput; set => actionInput = value; }
        public SequencedAction NextAction { get => nextAction; }

        public void SetNextContextAction(SequencedAction nextAction)
        {
            this.nextAction = nextAction;
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
