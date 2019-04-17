using CoreGame;
using System;
using System.Collections;
using UnityEngine;

namespace AdventureGame
{
    public class PlayerAnimationPlayer
    {

        public static IEnumerator PlayIdleSmokeAnimation(Animator animator, PlayerGlobalAnimationEventHandler PlayerGlobalAnimationEventHandler)
        {
            return AnimationPlayerHelper.Play(animator, PlayerAnimatioNamesEnum.PLAYER_IDLE_SMOKE_0, 0f,
                animationEndCallback: () =>
                {
                    if (AnimationPlayerHelper.IsCurrentStateNameEquals(animator, PlayerAnimatioNamesEnum.PLAYER_IDLE_SMOKE_1))
                    {
                        PlayerGlobalAnimationEventHandler.IdleOverideTriggerSmokeEffect();
                        return AnimationPlayerHelper.Play(animator, PlayerAnimatioNamesEnum.PLAYER_IDLE_SMOKE_1, 0f, null);
                    }
                    return null;
                });
        }

        public static IEnumerator PlayItemGivenAnimation(Animator animator, PlayerGlobalAnimationEventHandler PlayerGlobalAnimationEventHandler, Func<IEnumerator> onAnimationEnd)
        {
            return AnimationPlayerHelper.Play(animator, PlayerAnimatioNamesEnum.PLAYER_ACTION_GIVE_OBJECT_0, 0f,
                animationEndCallback: () =>
                {
                    if (AnimationPlayerHelper.IsCurrentStateNameEquals(animator, PlayerAnimatioNamesEnum.PLAYER_ACTION_GIVE_OBJECT_1))
                    {
                        PlayerGlobalAnimationEventHandler.ShowGivenItem();
                        return AnimationPlayerHelper.Play(animator, PlayerAnimatioNamesEnum.PLAYER_ACTION_GIVE_OBJECT_1, 0f, animationEndCallback: () =>
                        {
                            PlayerGlobalAnimationEventHandler.HideGivenItem();
                            if (onAnimationEnd != null)
                            {
                                return onAnimationEnd.Invoke();
                            }
                            return null;
                        });
                    }

                    return null;
                });
        }

    }

}