using GameConfigurationID;
using UnityEngine;
using UnityEngine.Playables;

namespace AdventureGame
{
    public class PlayerWarpClip : PlayableAsset
    {
        public PlayerWarpBehavior PlayerWarpBehavior;

        [CustomEnum()]
        public PointOfInterestId PointOfInterestId;
        public Vector3 Destination;
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            PlayerWarpBehavior = new PlayerWarpBehavior(this.Destination, this.PointOfInterestId);
            return ScriptPlayable<PlayerWarpBehavior>.Create(graph, PlayerWarpBehavior);
        }

    }

}
