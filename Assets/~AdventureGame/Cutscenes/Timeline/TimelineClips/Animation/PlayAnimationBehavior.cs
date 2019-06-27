using GameConfigurationID;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

namespace AdventureGame
{
    [System.Serializable]
    public class PlayAnimationBehavior : PlayableBehaviour
    {
        private PointOfInterestId pointOfInterestId;
        private AnimationID animationId;
        private bool waitForEnd;

        public PlayAnimationBehavior(AnimationID animationId, PointOfInterestId pointOfInterestId, bool waitForEnd)
        {
            this.animationId = animationId;
            this.pointOfInterestId = pointOfInterestId;
            this.waitForEnd = waitForEnd;
        }

        public PlayAnimationBehavior()
        {
        }

        private bool hasEnded = false;
        private PointOfInterestCutsceneController PointOfInterestCutsceneController;
        private PlayableDirector PlayableDirector;

        public override void OnGraphStart(Playable playable)
        {
            base.OnGraphStart(playable);
            var PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
            this.PointOfInterestCutsceneController = PointOfInterestManager.GetActivePointOfInterest(this.pointOfInterestId).GetPointOfInterestCutsceneController();
            this.PlayableDirector = playable.GetGraph().GetResolver() as PlayableDirector;
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);
            if (this.PointOfInterestCutsceneController != null)
            {
                if (!this.waitForEnd)
                {
                    this.PointOfInterestCutsceneController.PlayAnimation(this.animationId, 0f);
                }
                else
                {
                    Coroutiner.Instance.StartCoroutine(this.PlayAndWait());
                }
            }
        }

        private IEnumerator PlayAndWait()
        {
            yield return this.PointOfInterestCutsceneController.PlayAnimationAndWait(this.animationId, 0f, animationEndCallback: () =>
            {
                this.hasEnded = true;
                this.PlayableDirector.Resume();
                return null;
            });
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            base.OnBehaviourPause(playable, info);
            if (this.waitForEnd && !this.hasEnded)
            {
                this.PlayableDirector.Pause();
            }
        }

    }

}
