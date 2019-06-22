using UnityEngine;
using UnityEngine.Playables;

namespace AdventureGame
{
    public class PlayerItemHolderClip : PlayableAsset
    {

        public PlayerItemHolderBehavior PlayerItemHolderBehavior = new PlayerItemHolderBehavior();

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<PlayerItemHolderBehavior>.Create(graph, PlayerItemHolderBehavior);
        }

    }
}
