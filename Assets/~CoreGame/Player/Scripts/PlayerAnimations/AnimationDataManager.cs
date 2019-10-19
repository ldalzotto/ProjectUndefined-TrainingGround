using UnityEngine;

namespace CoreGame
{
    public class AnimationDataManager
    {
        [VE_Ignore] private Animator animator;

        private bool updateSpeed = false;

        public AnimationDataManager(Animator animator)
        {
            this.animator = animator;
            if (this.animator != null)
                foreach (var animatorParameter in this.animator.parameters)
                    if (animatorParameter.name == AnimationConstants.PlayerAnimatorParametersName.Speed)
                        updateSpeed = true;
        }

        public Animator Animator => animator;

        public void Tick(float unscaledSpeedMagnitude)
        {
            if (updateSpeed) animator.SetFloat(AnimationConstants.PlayerAnimatorParametersName.Speed, unscaledSpeedMagnitude);
        }
    }
}