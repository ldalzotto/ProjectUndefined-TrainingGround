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
        private Vector3 destination;
        private float normalizedSpeedMagnitude;

        public PlayerAIMoveToBehavior(Vector3 destination, float normalizedSpeedMagnitude, PointOfInterestId pointOfInterestId)
        {
            this.destination = destination;
            this.normalizedSpeedMagnitude = normalizedSpeedMagnitude;
            this.pointOfInterestId = pointOfInterestId;
        }

        public PlayerAIMoveToBehavior()
        {
        }

        private PointOfInterestCutsceneController PointOfInterestCutsceneController;
        private bool isMoving;
        private bool destinationReached;
        private PlayableDirector PlayableDirector;

        public override void OnGraphStart(Playable playable)
        {
            base.OnGraphStart(playable);
            this.destinationReached = false;
            this.isMoving = false;
            var PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
            this.PointOfInterestCutsceneController = PointOfInterestManager.GetActivePointOfInterest(this.pointOfInterestId).PointOfInterestCutsceneController;

            this.PlayableDirector = playable.GetGraph().GetResolver() as PlayableDirector;
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);
            if (this.PointOfInterestCutsceneController != null)
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
            yield return this.PointOfInterestCutsceneController.SetAIDestination(this.destination, this.normalizedSpeedMagnitude);
            this.destinationReached = true;
            this.PlayableDirector.Resume();
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            base.OnBehaviourPause(playable, info);
            if (!this.destinationReached && this.isMoving)
            {
                this.PlayableDirector.Pause();
            }
        }

    }

}
