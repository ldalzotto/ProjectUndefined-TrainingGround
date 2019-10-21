using UnityEngine;

namespace CoreGame
{
    public static class GenericAnimatorHelper
    {
        public static void SetMovementLayer(Animator animator, AnimationConfiguration animationConfiguration, LevelType levelType)
        {
            AnimationPlayerHelper.Play(animator, animationConfiguration.ConfigurationInherentData[GameConfigurationID.AnimationID.PUZZLE_MOVEMENT], 0f);
        }

        public static void ResetAllLayers(Animator animator, AnimationConfiguration animationConfiguration, LevelType levelType)
        {
            SetMovementLayer(animator, animationConfiguration, levelType);
            AnimationPlayerHelper.Play(animator, animationConfiguration.ConfigurationInherentData[GameConfigurationID.AnimationID.ACTION_LISTENING], 0f);
            AnimationPlayerHelper.Play(animator, animationConfiguration.ConfigurationInherentData[GameConfigurationID.AnimationID.ADDITIONAL_CONTEXT_ACTION_LISTENING], 0f);
            AnimationPlayerHelper.Play(animator, animationConfiguration.ConfigurationInherentData[GameConfigurationID.AnimationID.POSE_OVERRIVE_LISTENING], 0f);
        }
    }
}