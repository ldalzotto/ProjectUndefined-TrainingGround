using GameConfigurationID;
using UnityEngine;
using UnityEngine.Playables;

namespace AdventureGame
{
    [System.Serializable]
    public class DicsussionTimelineBehavior : PlayableBehaviour, IContextActionDyamicWorkflowListener
    {

        public DiscussionTreeId DiscussionTreeID;
        private ContextActionEventManager ContextActionEventManager;
        private PlayableDirector PlayableDirector;

        public DicsussionTimelineBehavior()
        {
        }

        public DicsussionTimelineBehavior(DiscussionTreeId discussionTreeID)
        {
            DiscussionTreeID = discussionTreeID;
        }

        public override void OnGraphStart(Playable playable)
        {
            base.OnGraphStart(playable);
            this.ContextActionEventManager = GameObject.FindObjectOfType<ContextActionEventManager>();
            this.PlayableDirector = playable.GetGraph().GetResolver() as PlayableDirector;
        }

        private bool discussionTriggered;
        private bool discussionFinished;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);
            if (this.ContextActionEventManager != null && !this.discussionTriggered && !this.discussionFinished)
            {
                this.ContextActionEventManager.OnContextActionAdd(new TalkAction(this.DiscussionTreeID, null), this);
                this.discussionTriggered = true;

                this.PlayableDirector.Pause();
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            base.OnBehaviourPause(playable, info);
            if (this.discussionTriggered && !this.discussionFinished)
            {
                this.PlayableDirector.Pause();
            }
        }

        public void OnContextActionFinished(AContextAction contextAction)
        {
            this.discussionTriggered = false;
            this.discussionFinished = true;
            this.PlayableDirector.Resume();
        }
    }

}
