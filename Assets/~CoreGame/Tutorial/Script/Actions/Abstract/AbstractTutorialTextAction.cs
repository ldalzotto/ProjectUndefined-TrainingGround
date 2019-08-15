using GameConfigurationID;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    [System.Serializable]
    public abstract class AbstractTutorialTextAction : SequencedAction
    {
        [CustomEnum()]
        [SerializeField]
        public DiscussionTextID DiscussionTextID;

        [NonSerialized]
        protected DiscussionWindow DiscussionWindow;
        [NonSerialized]
        private bool discussionEnded = false;
        [NonSerialized]
        protected TutorialActionInput TutorialActionInput;
        [NonSerialized]
        protected ITutorialTextActionManager TutorialTextActionManager;

        public AbstractTutorialTextAction(List<SequencedAction> nextActions) : base(nextActions)
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return this.discussionEnded;
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
            this.discussionEnded = false;
            this.TutorialActionInput = (TutorialActionInput)ContextActionInput;

            this.TutorialTextActionManager = this.GetTutorialTextManager(this.TutorialActionInput);
            this.DiscussionWindow = DiscussionWindow.Instanciate(this.TutorialActionInput.MainCanvas);
            this.DiscussionWindow.InitializeDependencies(() =>
            {
                MonoBehaviour.Destroy(this.DiscussionWindow.gameObject);
                this.discussionEnded = true;
            }, displayWorkflowIcon: false);
            this.TutorialTextActionManager.FirstExecutionAction(this.TutorialActionInput, this.DiscussionTextID, this.DiscussionWindow);
        }

        protected abstract ITutorialTextActionManager GetTutorialTextManager(TutorialActionInput tutorialActionInput);

        public override void Tick(float d)
        {
            this.DiscussionWindow.Tick(d);
            if (this.TutorialTextActionManager.Tick(d))
            {
                this.DiscussionWindow.PlayDiscussionCloseAnimation();
            }
        }

        public override void Interupt()
        {
            base.Interupt();
            MonoBehaviour.Destroy(this.DiscussionWindow.gameObject);
        }
    }

    public interface ITutorialTextActionManager
    {
        void FirstExecutionAction(TutorialActionInput TutorialActionInput, DiscussionTextID DiscussionTextID, DiscussionWindow discussionWindow);
        bool Tick(float d);
    }
}
