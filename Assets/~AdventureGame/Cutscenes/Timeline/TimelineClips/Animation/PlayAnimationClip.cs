using GameConfigurationID;
using UnityEngine;
using UnityEngine.Playables;

namespace AdventureGame
{
    public class PlayAnimationClip : PlayableAsset
    {
        [CustomEnum()]
        public PointOfInterestId pointOfInterestId;
        [CustomEnum()]
        public AnimationID animationId;
        public bool waitForEnd = false;

        public PlayAnimationBehavior PlayAnimationBehavior;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            this.PlayAnimationBehavior = new PlayAnimationBehavior(this.animationId, this.pointOfInterestId, this.waitForEnd);
            return ScriptPlayable<PlayAnimationBehavior>.Create(graph, this.PlayAnimationBehavior);
        }

    }
}
