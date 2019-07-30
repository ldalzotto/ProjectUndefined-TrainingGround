using CoreGame;
using GameConfigurationID;
using System;
using System.Collections;
using UnityEngine;
using static CoreGame.AnimationPlayerHelper;

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

    }

}