using System;
using System.Collections.Generic;
using CoreGame;
using SequencedAction;
using UnityEngine;

namespace Tutorial
{
    public abstract class AbstractTutorialTextAction : ASequencedAction
    {
        [NonSerialized] protected DiscussionWindow DiscussionWindow;
        [NonSerialized] private bool discussionEnded = false;
        [NonSerialized] protected ITutorialTextActionManager TutorialTextActionManager;

        public AbstractTutorialTextAction(Func<List<ASequencedAction>> nextActionsDeferred) : base(nextActionsDeferred)
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return this.discussionEnded;
        }

        public override void FirstExecutionAction()
        {
            this.discussionEnded = false;

            this.TutorialTextActionManager = this.GetTutorialTextManager();
            this.DiscussionWindow = DiscussionWindow.Instanciate(CoreGameSingletonInstances.GameCanvas);
            this.DiscussionWindow.InitializeDependencies(() =>
            {
                MonoBehaviour.Destroy(this.DiscussionWindow.gameObject);
                this.discussionEnded = true;
            }, displayWorkflowIcon: false);
            this.TutorialTextActionManager.FirstExecutionAction();
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
        void FirstExecutionAction();
        bool Tick(float d);
    }
}