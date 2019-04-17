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

        public static IEnumerator Play(Animator animator, string animationName, int animationlayerIndex, float crossFadeDuration, Func<IEnumerator> animationEndCallback)
        {
            animator.CrossFade(animationName, crossFadeDuration);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfAnimation(animator, animationName, animationlayerIndex);

            if (animationEndCallback == null)
            {
                yield break;
            }
            else
            {
                yield return animationEndCallback.Invoke();
            }
        }

        public static IEnumerator Play(Animator animator, PlayerAnimatioNamesEnum playerAnimatioNnamesEnum, float crossFadeDuration, Func<IEnumerator> animationEndCallback)
        {
            var animationName = AnimationConstants.PlayerAnimationConstants[playerAnimatioNnamesEnum].AnimationName;
            var animationLayerIndex = AnimationConstants.PlayerAnimationConstants[playerAnimatioNnamesEnum].GetLayerIndex(animator);
            return Play(animator, animationName, animationLayerIndex, crossFadeDuration, animationEndCallback);
        }

        public static bool IsCurrentStateNameEquals(Animator animator ,PlayerAnimatioNamesEnum playerAnimatioNnamesEnum)
        {
            var animationName = AnimationConstants.PlayerAnimationConstants[playerAnimatioNnamesEnum].AnimationName;
            var animationLayerIndex = AnimationConstants.PlayerAnimationConstants[playerAnimatioNnamesEnum].GetLayerIndex(animator);
            return animator.GetCurrentAnimatorStateInfo(animationLayerIndex).IsName(animationName);
        }


    }

}
