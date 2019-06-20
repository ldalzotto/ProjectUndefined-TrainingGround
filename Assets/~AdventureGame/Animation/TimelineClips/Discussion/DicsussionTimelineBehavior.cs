using GameConfigurationID;
using UnityEngine;
using UnityEngine.Playables;

namespace AdventureGame
{
    [System.Serializable]
    public class DicsussionTimelineBehavior : PlayableBehaviour
    {

        public DiscussionTreeId DiscussionTreeID;
        private ContextActionEventManager ContextActionEventManager;

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
        }

        private bool discussionTriggered;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);
            if (this.ContextActionEventManager != null && !this.discussionTriggered)
            {
                this.ContextActionEventManager.OnContextActionAdd(new TalkAction(this.DiscussionTreeID, null));
                this.discussionTriggered = true;
            }
        }
    }

}
