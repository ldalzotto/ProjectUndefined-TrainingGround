using GameConfigurationID;
using UnityEngine;
using UnityEngine.Playables;

namespace AdventureGame
{
    public class PlayerAIMoveToClip : PlayableAsset
    {
        public PlayerAIMoveToBehavior PlayerAIMoveToBehavior;

        [CustomEnum()]
        public PointOfInterestId PointOfInterestId;
        public Vector3 Destination;

        [Range(0f, 1f)]
        public float NormalizedSpeedMagnitude = 1f;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            this.PlayerAIMoveToBehavior = new PlayerAIMoveToBehavior(this.Destination, this.NormalizedSpeedMagnitude, this.PointOfInterestId);
            return ScriptPlayable<PlayerAIMoveToBehavior>.Create(graph, this.PlayerAIMoveToBehavior);
        }

    }

}
