using CoreGame;
using GameConfigurationID;
using System;
using System.Collections;
using UnityEngine;

namespace AdventureGame
{
    public class PlayerAnimationPlayer
    {
        public static IEnumerator PlayIdleSmokeAnimation(Animator animator, AnimationConfiguration animationConfiguration, Action onTriggerSmokeEffect, Action onAnimationEnd)
        {
            return PlaySlicesAnimation(animator, animationConfiguration, new SliceAnimation[]
            {
                new SliceAnimation()
                {
                    AnimationID = AnimationID.IDLE_SMOKE_0,
                    CrossFadeDuration = 0f,
                    AnimationEndCallback = onTriggerSmokeEffect
                }, new SliceAnimation()
                {
                    AnimationID = AnimationID.IDLE_SMOKE_1,
                    CrossFadeDuration = 0f,
                    AnimationEndCallback = onAnimationEnd
                }
            });
        }

        public static IEnumerator PlayItemGivenAnimation(Animator animator, AnimationConfiguration animationConfiguration, Action onItemShow, Action onAnimationEnd)
        {
            return PlaySlicesAnimation(animator, animationConfiguration, new SliceAnimation[] {
                new SliceAnimation()
                {
                    AnimationID = AnimationID.ACTION_GIVE_OBJECT_0,
                    CrossFadeDuration = 0f,
                    AnimationEndCallback = onItemShow
                },
                new SliceAnimation()
                {
                    AnimationID = AnimationID.ACTION_GIVE_OBJECT_1,
                    CrossFadeDuration = 0f,
                    AnimationEndCallback = onAnimationEnd
                }
            });
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