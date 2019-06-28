using UnityEngine;

namespace CoreGame
{
    public static class GenericAnimatorHelper
    {

        public static void SetMovementLayer(Animator animator, AnimationConfiguration animationConfiguration, LevelType levelType)
        {
            if (levelType == LevelType.ADVENTURE)
            {
                AnimationPlayerHelper.Play(animator, animationConfiguration.ConfigurationInherentData[GameConfigurationID.AnimationID.ADVENTURE_MOVEMENT], 0f);
            }
            else
            {
                AnimationPlayerHelper.Play(animator, animationConfiguration.ConfigurationInherentData[GameConfigurationID.AnimationID.PUZZLE_MOVEMENT], 0f);
            }
        }

    }
}
