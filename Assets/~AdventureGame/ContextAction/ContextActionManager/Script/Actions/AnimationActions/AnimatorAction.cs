using CoreGame;
using UnityEngine;

namespace AdventureGame
{
    public class AnimatorAction : AContextAction
    {
        private PlayerAnimatioNamesEnum playerAnimationEnum;
        private bool animationEnded;

        public AnimatorAction(PlayerAnimatioNamesEnum playerAnimationEnum, AContextAction nextContextAction) : base(nextContextAction)
        {
            this.playerAnimationEnum = playerAnimationEnum;
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return animationEnded;
        }

        public override void FirstExecutionAction(AContextActionInput ContextActionInput)
        {
            animationEnded = false;
            var animatorActionInput = (AnimatorActionInput)ContextActionInput;

            animatorActionInput.PlayerManager.StartCoroutine(AnimationPlayerHelper.Play(animatorActionInput.PlayerAnimator, playerAnimationEnum, 0f, () =>
            {
                animationEnded = true;
                return null;
            }));
        }

        public override void Tick(float d)
        {
        }
    }

    public class AnimatorActionInput : AContextActionInput
    {
        private Animator playerAnimator;
        private PlayerManager playerManager;

        public AnimatorActionInput(Animator playerAnimator, PlayerManager playerManager)
        {
            this.playerAnimator = playerAnimator;
            this.playerManager = playerManager;
        }

        public Animator PlayerAnimator { get => playerAnimator; }
        public PlayerManager PlayerManager { get => playerManager; }
    }
}