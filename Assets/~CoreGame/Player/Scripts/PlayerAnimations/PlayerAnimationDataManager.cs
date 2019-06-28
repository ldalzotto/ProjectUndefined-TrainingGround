using UnityEngine;

namespace CoreGame
{
    public class PlayerAnimationDataManager
    {

        private Animator animator;

        public PlayerAnimationDataManager(Animator animator)
        {
            this.animator = animator;
        }

        public Animator Animator { get => animator; }

        public void Tick(float unscaledSpeedMagnitude)
        {
            animator.SetFloat(AnimationConstants.PlayerAnimatorParametersName.Speed, unscaledSpeedMagnitude);
        }

    }
}
