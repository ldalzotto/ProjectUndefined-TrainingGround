using GameConfigurationID;
using UnityEngine;

namespace CoreGame
{
    public static class GenericAnimatorHelper
    {
        public static void SetMovementLayer(Animator animator, AnimationConfiguration animationConfiguration)
        {
            AnimationPlayerHelper.Play(animator, animationConfiguration.ConfigurationInherentData[AnimationID.PUZZLE_MOVEMENT], 0f);
        }

        public static void ResetAllLayers(Animator animator, AnimationConfiguration animationConfiguration)
        {
            SetMovementLayer(animator, animationConfiguration);
            AnimationPlayerHelper.Play(animator, animationConfiguration.ConfigurationInherentData[AnimationID.ACTION_LISTENING], 0f);
            AnimationPlayerHelper.Play(animator, animationConfiguration.ConfigurationInherentData[AnimationID.ADDITIONAL_CONTEXT_ACTION_LISTENING], 0f);
            AnimationPlayerHelper.Play(animator, animationConfiguration.ConfigurationInherentData[AnimationID.POSE_OVERRIVE_LISTENING], 0f);
        }
    }
}