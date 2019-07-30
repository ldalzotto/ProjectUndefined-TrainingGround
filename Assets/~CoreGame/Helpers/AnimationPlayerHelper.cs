using GameConfigurationID;
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
        
        public static IEnumerator PlaySlicesAnimation(Animator animator, AnimationConfiguration animationConfiguration, SliceAnimation[] sliceAnimations)
        {
            return PlaySliceAnimation(animator, animationConfiguration, sliceAnimations, 0);
        }

        private static IEnumerator PlaySliceAnimation(Animator animator, AnimationConfiguration animationConfiguration, SliceAnimation[] sliceAnimations, int currentIndex)
        {
            var sliceAnimation = sliceAnimations[currentIndex];
            yield return AnimationPlayerHelper.PlayAndWait(animator, animationConfiguration.ConfigurationInherentData[sliceAnimation.AnimationID], sliceAnimation.CrossFadeDuration, animationEndCallback: () =>
            {
                if (sliceAnimations.Length >= currentIndex + 2 && AnimationPlayerHelper.IsCurrentStateNameEquals(animator, animationConfiguration.ConfigurationInherentData[sliceAnimations[currentIndex + 1].AnimationID]))
                {
                    sliceAnimation.AnimationEndCallback.Invoke();
                    return PlaySliceAnimation(animator, animationConfiguration, sliceAnimations, currentIndex + 1);
                }
                else if (sliceAnimations.Length == currentIndex + 1)
                {
                    if (sliceAnimation.AnimationEndCallback != null)
                    {
                        sliceAnimation.AnimationEndCallback.Invoke();
                    }
                }
                return null;
            });
        }

        public struct SliceAnimation
        {
            public AnimationID AnimationID;
            public float CrossFadeDuration;
            public Action AnimationEndCallback;

            public SliceAnimation(AnimationID animationID, float crossFadeDuration, Action animationEndCallback)
            {
                AnimationID = animationID;
                CrossFadeDuration = crossFadeDuration;
                AnimationEndCallback = animationEndCallback;
            }
        }



    }

}
