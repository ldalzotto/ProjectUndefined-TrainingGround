using System;
using System.Collections;
using UnityEngine;

namespace CoreGame
{
    public class AnimationPlayerHelper
    {

        public static IEnumerator Play(Animator animator, string animationName, int animationlayerIndex, float crossFadeDuration)
        {
            animator.CrossFade(animationName, crossFadeDuration);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfAnimation(animator, animationName, animationlayerIndex);
        }

        public static IEnumerator Play(Animator animator, string animationName, int animationlayerIndex, float crossFadeDuration, Action animationEndCallback)
        {
            animator.CrossFade(animationName, crossFadeDuration);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfAnimation(animator, animationName, animationlayerIndex);
            animationEndCallback.Invoke();
        }

        public static IEnumerator Play(Animator animator, PlayerAnimatioNamesEnum playerAnimatioNnamesEnum, float crossFadeDuration, Action animationEndCallback)
        {
            var animationName = AnimationConstants.PlayerAnimationConstants[playerAnimatioNnamesEnum].AnimationName;
            var animationLayerIndex = AnimationConstants.PlayerAnimationConstants[playerAnimatioNnamesEnum].LayerIndex;
            return Play(animator, animationName, animationLayerIndex, crossFadeDuration, animationEndCallback);
        }


    }

}
