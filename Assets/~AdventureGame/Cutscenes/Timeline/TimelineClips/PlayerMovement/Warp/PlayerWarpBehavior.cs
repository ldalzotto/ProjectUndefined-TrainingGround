using GameConfigurationID;
using UnityEngine;
using UnityEngine.Playables;

namespace AdventureGame
{
    [System.Serializable]
    public class PlayerWarpBehavior : PlayableBehaviour
    {
        private PointOfInterestId pointOfInterestId;
        private CutscenePositionMarkerID destination;
        private CutsceneId cutsceneId;

        public PlayerWarpBehavior()
        {
        }

        public PlayerWarpBehavior(CutscenePositionMarkerID destination, CutsceneId cutsceneId,PointOfInterestId pointOfInterestId)
        {
            this.cutsceneId = cutsceneId;
            this.destination = destination;
            this.pointOfInterestId = pointOfInterestId;
        }

        private PointOfInterestCutsceneController PointOfInterestCutsceneController;
        private CutscenePositionsManager CutscenePositionsManager;

        public override void OnPlayableCreate(Playable playable)
        {
            base.OnPlayableCreate(playable);
            this.PointOfInterestCutsceneController = GameObject.FindObjectOfType<PointOfInterestManager>().GetActivePointOfInterest(this.pointOfInterestId).PointOfInterestCutsceneController;
            this.CutscenePositionsManager = GameObject.FindObjectOfType<CutscenePositionsManager>();
        }

        private bool warped = false;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);
            if (this.PointOfInterestCutsceneController != null && this.CutscenePositionsManager != null && !this.warped)
            {
                this.PointOfInterestCutsceneController.Warp(this.CutscenePositionsManager.GetCutscenePosition(this.cutsceneId, this.destination).transform);
                this.warped = true;
            }
        }

    }

}
