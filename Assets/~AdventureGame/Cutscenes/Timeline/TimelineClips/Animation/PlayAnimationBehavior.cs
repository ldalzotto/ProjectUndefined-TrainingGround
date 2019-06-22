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
        private PlayerAnimatioNamesEnum animationId;

        public PlayAnimationBehavior(PlayerAnimatioNamesEnum animationId, PointOfInterestId pointOfInterestId)
        {
            this.animationId = animationId;
            this.pointOfInterestId = pointOfInterestId;
        }

        public PlayAnimationBehavior()
        {
        }

        private PointOfInterestCutsceneController PointOfInterestCutsceneController;
        private PlayableDirector PlayableDirector;

        public override void OnGraphStart(Playable playable)
        {
            base.OnGraphStart(playable);
            var PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
            this.PointOfInterestCutsceneController = PointOfInterestManager.GetActivePointOfInterest(this.pointOfInterestId).PointOfInterestCutsceneController;
            this.PlayableDirector = playable.GetGraph().GetResolver() as PlayableDirector;
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);
            if (this.PointOfInterestCutsceneController != null)
            {
                this.PointOfInterestCutsceneController.PlayAnimation(this.animationId, 0f);
            }
        }

    }

}
