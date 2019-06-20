using GameConfigurationID;
using UnityEngine;
using UnityEngine.Playables;

namespace AdventureGame
{
    [System.Serializable]
    public class PlayerWarpBehavior : PlayableBehaviour
    {
        private Vector3 destination;
        private PointOfInterestId pointOfInterestId;

        public PlayerWarpBehavior()
        {
        }

        public PlayerWarpBehavior(Vector3 destination, PointOfInterestId pointOfInterestId)
        {
            this.destination = destination;
            this.pointOfInterestId = pointOfInterestId;
        }

        private PointOfInterestCutsceneController PointOfInterestCutsceneController;

        public override void OnPlayableCreate(Playable playable)
        {
            base.OnPlayableCreate(playable);
            this.PointOfInterestCutsceneController = GameObject.FindObjectOfType<PointOfInterestManager>().GetActivePointOfInterest(this.pointOfInterestId).PointOfInterestCutsceneController;
        }

        private bool warped = false;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);
            if (this.PointOfInterestCutsceneController != null && !this.warped)
            {
                this.PointOfInterestCutsceneController.Warp(this.destination);
                this.warped = true;
            }
        }

    }

}
