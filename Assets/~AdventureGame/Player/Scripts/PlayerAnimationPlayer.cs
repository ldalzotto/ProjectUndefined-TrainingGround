using CoreGame;
using System;
using System.Collections;
using UnityEngine;

namespace AdventureGame
{
    public class PlayerAnimationPlayer
    {
        public static IEnumerator PlayIdleSmokeAnimation(Animator animator, Action onTriggerSmokeEffect, Action onAnimationEnd)
        {
            return PlaySlicesAnimation(animator, new SliceAnimation[]
            {
                new SliceAnimation()
                {
                    PlayerAnimatioNamesEnum = PlayerAnimatioNamesEnum.PLAYER_IDLE_SMOKE_0,
                    CrossFadeDuration = 0f,
                    AnimationEndCallback = onTriggerSmokeEffect
                }, new SliceAnimation()
                {
                    PlayerAnimatioNamesEnum = PlayerAnimatioNamesEnum.PLAYER_IDLE_SMOKE_1,
                    CrossFadeDuration = 0f,
                    AnimationEndCallback = onAnimationEnd
                }
            });
        }

        public static IEnumerator PlayItemGivenAnimation(Animator animator, Action onItemShow, Action onAnimationEnd)
        {
            return PlaySlicesAnimation(animator, new SliceAnimation[] {
                new SliceAnimation()
                {
                    PlayerAnimatioNamesEnum = PlayerAnimatioNamesEnum.PLAYER_ACTION_GIVE_OBJECT_0,
                    CrossFadeDuration = 0f,
                    AnimationEndCallback = onItemShow
                },
                new SliceAnimation()
                {
                    PlayerAnimatioNamesEnum = PlayerAnimatioNamesEnum.PLAYER_ACTION_GIVE_OBJECT_1,
                    CrossFadeDuration = 0f,
                    AnimationEndCallback = onAnimationEnd
                }
            });
        }

        public static IEnumerator PlaySlicesAnimation(Animator animator, SliceAnimation[] sliceAnimations)
        {
            return PlaySliceAnimation(animator, sliceAnimations, 0);
        }

        private static IEnumerator PlaySliceAnimation(Animator animator, SliceAnimation[] sliceAnimations, int currentIndex)
        {
            var sliceAnimation = sliceAnimations[currentIndex];
            yield return AnimationPlayerHelper.PlayAndWait(animator, sliceAnimation.PlayerAnimatioNamesEnum, sliceAnimation.CrossFadeDuration, animationEndCallback: () =>
            {
                if (sliceAnimations.Length >= currentIndex + 2 && AnimationPlayerHelper.IsCurrentStateNameEquals(animator, sliceAnimations[currentIndex + 1].PlayerAnimatioNamesEnum))
                {
                    sliceAnimation.AnimationEndCallback.Invoke();
                    return PlaySliceAnimation(animator, sliceAnimations, currentIndex + 1);
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
            public PlayerAnimatioNamesEnum PlayerAnimatioNamesEnum;
            public float CrossFadeDuration;
            public Action AnimationEndCallback;

            public SliceAnimation(PlayerAnimatioNamesEnum playerAnimatioNamesEnum, float crossFadeDuration, Action animationEndCallback)
            {
                PlayerAnimatioNamesEnum = playerAnimatioNamesEnum;
                CrossFadeDuration = crossFadeDuration;
                AnimationEndCallback = animationEndCallback;
            }
        }

    }

}