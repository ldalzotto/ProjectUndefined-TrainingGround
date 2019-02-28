using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AdventureGame
{
    public class PlayerItemHolderClip : PlayableAsset, ITimelineClipAsset
    {

        public PlayerItemHolderBehavior PlayerItemHolderBehavior = new PlayerItemHolderBehavior();

        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<PlayerItemHolderBehavior>.Create(graph, PlayerItemHolderBehavior);
        }

    }
}
