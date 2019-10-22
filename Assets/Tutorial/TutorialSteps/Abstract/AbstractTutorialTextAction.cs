using System;
using System.Collections.Generic;
using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace Tutorial
{
    public abstract class AbstractTutorialTextAction : SequencedAction
    {
        protected DiscussionTextID DiscussionTextID;

        [NonSerialized] protected DiscussionWindow DiscussionWindow;
        [NonSerialized] private bool discussionEnded = false;
        [NonSerialized] protected ITutorialTextActionManager TutorialTextActionManager;

        public AbstractTutorialTextAction(DiscussionTextID DiscussionTextID, List<SequencedAction> nextActions) : base(nextActions)
        {
            this.DiscussionTextID = DiscussionTextID;
        }

        public override bool ComputeFinishedConditions()
        {
            return this.discussionEnded;
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
            this.discussionEnded = false;

            this.TutorialTextActionManager = this.GetTutorialTextManager();
            this.DiscussionWindow = DiscussionWindow.Instanciate(CoreGameSingletonInstances.GameCanvas);
            this.DiscussionWindow.InitializeDependencies(() =>
            {
                MonoBehaviour.Destroy(this.DiscussionWindow.gameObject);
                this.discussionEnded = true;
            }, displayWorkflowIcon: false);
            this.TutorialTextActionManager.FirstExecutionAction(this.DiscussionTextID, this.DiscussionWindow);
        }

        protected abstract ITutorialTextActionManager GetTutorialTextManager();

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
        void FirstExecutionAction(DiscussionTextID DiscussionTextID, DiscussionWindow discussionWindow);
        bool Tick(float d);
    }
}