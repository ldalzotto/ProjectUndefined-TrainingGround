using GameConfigurationID;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

namespace AdventureGame
{
    [System.Serializable]
    public class PlayerAIMoveToBehavior : PlayableBehaviour
    {
        private PointOfInterestId pointOfInterestId;
        private CutsceneId cutsceneId;
        private CutscenePositionMarkerID cutscenePositionMarkerID;
        private float normalizedSpeedMagnitude;

        public PlayerAIMoveToBehavior(CutsceneId cutsceneId, CutscenePositionMarkerID cutscenePositionMarkerID, float normalizedSpeedMagnitude, PointOfInterestId pointOfInterestId)
        {
            this.cutscenePositionMarkerID = cutscenePositionMarkerID;
            this.cutsceneId = cutsceneId;
            this.normalizedSpeedMagnitude = normalizedSpeedMagnitude;
            this.pointOfInterestId = pointOfInterestId;
        }

        public PlayerAIMoveToBehavior()
        {
        }

        private PointOfInterestCutsceneController PointOfInterestCutsceneController;
        private CutscenePositionsManager CutscenePositionsManager;
        private bool isMoving;
        private bool destinationReached;
        private PlayableDirector PlayableDirector;

        public override void OnGraphStart(Playable playable)
        {
            base.OnGraphStart(playable);
            this.destinationReached = false;
            this.isMoving = false;
            var PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
            this.PointOfInterestCutsceneController = PointOfInterestManager.GetActivePointOfInterest(this.pointOfInterestId).GetPointOfInterestCutsceneController();
            this.CutscenePositionsManager = GameObject.FindObjectOfType<CutscenePositionsManager>();

            this.PlayableDirector = playable.GetGraph().GetResolver() as PlayableDirector;
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            Debug.Log(MyLog.Format("OnBehaviorPlay"));
            base.OnBehaviourPlay(playable, info);
            if (this.PointOfInterestCutsceneController != null && this.CutscenePositionsManager != null)
            {
                if (Coroutiner.Instance != null)
                {
                    Coroutiner.Instance.StartCoroutine(this.SetDestination());
                }
            }
        }

        private IEnumerator SetDestination()
        {
            this.isMoving = true;
            yield return this.PointOfInterestCutsceneController.SetAIDestination(this.CutscenePositionsManager.GetCutscenePosition(this.cutsceneId, this.cutscenePositionMarkerID).transform, this.normalizedSpeedMagnitude);
            this.destinationReached = true;
            this.PlayableDirector.Resume();
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            Debug.Log(MyLog.Format("OnBehaviourPause"));
            base.OnBehaviourPause(playable, info);
            if (!this.destinationReached && this.isMoving)
            {
                this.PlayableDirector.Pause();
            }
        }

    }

}
