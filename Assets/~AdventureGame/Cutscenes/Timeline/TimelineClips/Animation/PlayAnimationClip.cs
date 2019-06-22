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
        public PlayAnimationBehavior PlayAnimationBehavior;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            this.PlayAnimationBehavior = new PlayAnimationBehavior(this.animationId, this.pointOfInterestId);
            return ScriptPlayable<PlayAnimationBehavior>.Create(graph, this.PlayAnimationBehavior);
        }

    }
}
