using System;
using System.Collections;
using UnityEngine;

namespace CoreGame
{
    public class AnimationPlayerHelper
    {

        public static IEnumerator PlayAndWait(Animator animator, string animationName, int animationlayerIndex, float crossFadeDuration, bool updateModelImmediately = false, bool framePerfectEndDetection = false)
        {
            animator.CrossFade(animationName, crossFadeDuration);
            if (updateModelImmediately)
            {
                animator.Update(0f);
            }
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfAnimation(animator, animationName, animationlayerIndex, framePerfectEndDetection);
        }

        public static IEnumerator PlayAndWait(Animator animator, string animationName, int animationlayerIndex, float crossFadeDuration, Func<IEnumerator> animationEndCallback, bool updateModelImmediately = false, bool framePerfectEndDetection = false)
        {
            yield return PlayAndWait(animator, animationName, animationlayerIndex, crossFadeDuration, updateModelImmediately, framePerfectEndDetection);
            if (animationEndCallback == null)
            {
                yield break;
            }
            else
            {
                yield return animationEndCallback.Invoke();
            }
        }

        public static IEnumerator PlayAndWait(Animator animator, AnimationConfigurationData animationConfigurationData, float crossFadeDuration, Func<IEnumerator> animationEndCallback, bool updateModelImmediately = false, bool framePerfectEndDetection = false)
        {
            var animationName = animationConfigurationData.AnimationName;
            var animationLayerIndex = animationConfigurationData.GetLayerIndex(animator);
            if (animationLayerIndex != -1)
            {
                return PlayAndWait(animator, animationName, animationLayerIndex, crossFadeDuration, animationEndCallback, updateModelImmediately, framePerfectEndDetection);
            }
            else
            {
                return null;
            }
        }



        public static void Play(Animator animator, string animationName, int animationlayerIndex, float crossFadeDuration, bool updateModelImmediately = false)
        {
            animator.CrossFade(animationName, crossFadeDuration);
            if (updateModelImmediately)
            {
                animator.Update(0f);
            }
        }

        public static void Play(Animator animator, AnimationConfigurationData animationConfigurationData, float crossFadeDuration, bool updateModelImmediately = false)
        {
            var animationName = animationConfigurationData.AnimationName;
            var animationLayerIndex = animationConfigurationData.GetLayerIndex(animator);
            if (animationLayerIndex != -1)
            {
                Play(animator, animationName, animationLayerIndex, crossFadeDuration, updateModelImmediately);
            }
        }


        public static bool IsCurrentStateNameEquals(Animator animator, AnimationConfigurationData animationConfigurationData)
        {
            var animationName = animationConfigurationData.AnimationName;
            var animationLayerIndex = animationConfigurationData.GetLayerIndex(animator);
            if (animationLayerIndex != -1)
            {
                return animator.GetCurrentAnimatorStateInfo(animationLayerIndex).IsName(animationName);
            }
            else
            {
                return false;
            }
        }



    }

}
