using GameConfigurationID;
using UnityEngine;
using UnityEngine.Playables;

namespace AdventureGame
{
    public class DicsussionTimelineClip : PlayableAsset
    {
        public DiscussionTreeId DiscussionTreeID;
        public DicsussionTimelineBehavior DicsussionTimelineBehavior;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            this.DicsussionTimelineBehavior = new DicsussionTimelineBehavior(this.DiscussionTreeID);
            return ScriptPlayable<DicsussionTimelineBehavior>.Create(graph, this.DicsussionTimelineBehavior);
        }
    }
}
