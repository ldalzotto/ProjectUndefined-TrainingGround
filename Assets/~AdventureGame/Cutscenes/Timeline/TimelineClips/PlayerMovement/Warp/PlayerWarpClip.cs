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
        [CustomEnum()]
        public CutscenePositionMarkerID Destination;
        [CustomEnum(isCreateable: true)]
        public CutsceneId cutsceneId;

        public CutsceneId CutsceneId { set => cutsceneId = value; }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            PlayerWarpBehavior = new PlayerWarpBehavior(this.Destination, this.cutsceneId, this.PointOfInterestId);
            return ScriptPlayable<PlayerWarpBehavior>.Create(graph, PlayerWarpBehavior);
        }

    }

}
