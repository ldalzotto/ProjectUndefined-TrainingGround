using GameConfigurationID;
using System;
using System.Collections;
using UnityEngine;

namespace CoreGame
{
    public class AnimationPlayerHelper
    {

        public static IEnumerator PlayAndWait(Animator animator, string animationName, int animationlayerIndex, float crossFadeDuration)
        {
            animator.CrossFade(animationName, crossFadeDuration);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfAnimation(animator, animationName, animationlayerIndex);
        }

        public static IEnumerator PlayAndWait(Animator animator, string animationName, int animationlayerIndex, float crossFadeDuration, Func<IEnumerator> animationEndCallback)
        {
            yield return PlayAndWait(animator, animationName, animationlayerIndex, crossFadeDuration);
            if (animationEndCallback == null)
            {
                yield break;
            }
            else
            {
                yield return animationEndCallback.Invoke();
            }
        }

        public static IEnumerator PlayAndWait(Animator animator, AnimationConfigurationData animationConfigurationData, float crossFadeDuration, Func<IEnumerator> animationEndCallback)
        {
            var animationName = animationConfigurationData.AnimationName;
            var animationLayerIndex = animationConfigurationData.GetLayerIndex(animator);
            return PlayAndWait(animator, animationName, animationLayerIndex, crossFadeDuration, animationEndCallback);
        }



        public static void Play(Animator animator, string animationName, int animationlayerIndex, float crossFadeDuration)
        {
            animator.CrossFade(animationName, crossFadeDuration);
        }

        public static void Play(Animator animator, AnimationConfigurationData animationConfigurationData, float crossFadeDuration)
        {
            var animationName = animationConfigurationData.AnimationName;
            var animationLayerIndex = animationConfigurationData.GetLayerIndex(animator);
            Play(animator, animationName, animationLayerIndex, crossFadeDuration);
        }


        public static bool IsCurrentStateNameEquals(Animator animator, AnimationConfigurationData animationConfigurationData)
        {
            var animationName = animationConfigurationData.AnimationName;
            var animationLayerIndex = animationConfigurationData.GetLayerIndex(animator);
            return animator.GetCurrentAnimatorStateInfo(animationLayerIndex).IsName(animationName);
        }


    }

}
