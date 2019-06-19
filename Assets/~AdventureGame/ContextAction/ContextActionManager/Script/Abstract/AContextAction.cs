using UnityEngine;
using System.Collections;
using System;
using GameConfigurationID;

namespace AdventureGame
{
    public interface IContextActionDrawable
    {
#if UNITY_EDITOR
        void ActionGUI();
#endif
    }

    [System.Serializable]
    public abstract class AContextAction : IContextActionDrawable
    {
        public abstract void FirstExecutionAction(AContextActionInput ContextActionInput);
        public abstract bool ComputeFinishedConditions();
        public abstract void AfterFinishedEventProcessed();
        public abstract void Tick(float d);

        [SerializeField]
        protected SelectionWheelNodeConfigurationId contextActionWheelNodeConfigurationId;
        [SerializeField]
        private AContextAction nextContextAction;

        #region Internal Dependencies
        [NonSerialized]
        private AContextActionInput contextActionInput;
        #endregion

        public AContextAction(AContextAction nextAction)
        {
            nextContextAction = nextAction;
        }

        public void OnTick(float d, ContextActionEventManager contextActionEventManager)
        {
            if (!isFinished)
            {
                Tick(d);

                if (ComputeFinishedConditions())
                {
                    isFinished = true;
                    contextActionEventManager.OnContextActionFinished(this, contextActionInput);
                    AfterFinishedEventProcessed();
                }
            }
        }

        private bool isFinished;

        public AContextActionInput ContextActionInput { get => contextActionInput; set => contextActionInput = value; }
        public AContextAction NextContextAction { get => nextContextAction; }
        public SelectionWheelNodeConfigurationId ContextActionWheelNodeConfigurationId { get => contextActionWheelNodeConfigurationId; set => contextActionWheelNodeConfigurationId = value; }

        public void SetNextContextAction(AContextAction nextContextAction)
        {
            this.nextContextAction = nextContextAction;
        }

        public bool IsFinished()
        {
            return isFinished;
        }

        internal void ResetState()
        {
            isFinished = false;
        }

        #region Logical Conditions
        public bool IsTalkAction()
        {
            return GetType() == typeof(TalkAction);
        }
        #endregion

#if UNITY_EDITOR
        public virtual void ActionGUI() { }
#endif

    }

    [System.Serializable]
    public abstract class AContextActionInput
    {

    }
}
