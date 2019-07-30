using UnityEngine;

namespace CoreGame
{
    public class PlayerAnimationDataManager
    {

        private Animator animator;

        private bool updateSpeed = false;

        public PlayerAnimationDataManager(Animator animator)
        {
            this.animator = animator;
            if (this.animator != null)
            {
                foreach (var animatorParameter in this.animator.parameters)
                {
                    if (animatorParameter.name == AnimationConstants.PlayerAnimatorParametersName.Speed)
                    {
                        this.updateSpeed = true;
                    }
                }
            }
        }

        public Animator Animator { get => animator; }

        public void Tick(float unscaledSpeedMagnitude)
        {
            if (this.updateSpeed)
            {
                animator.SetFloat(AnimationConstants.PlayerAnimatorParametersName.Speed, unscaledSpeedMagnitude);
            }
        }

    }
}
