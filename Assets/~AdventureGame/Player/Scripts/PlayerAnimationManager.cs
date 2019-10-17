using CoreGame;
using UnityEngine;

namespace AdventureGame
{
    public class PlayerAnimationManager : MonoBehaviour
    {
        [Header("Animation")] private AnimationDataManager _animationDataManager;

        #region FX Instanciation handler

        public AnimationDataManager AnimationDataManager => _animationDataManager;

        #endregion

        private void Start()
        {
            var AdventurePrefabConfiguration = AdventureGameSingletonInstances.AdventureStaticConfigurationContainer.AdventureStaticConfiguration.AdventurePrefabConfiguration;
            var AnimationConfiguration = CoreGameSingletonInstances.CoreConfigurationManager.AnimationConfiguration();
            var PlayerAnimator = GetComponentInChildren<Animator>();

            _animationDataManager = new AnimationDataManager(PlayerAnimator);

            GenericAnimatorHelper.SetMovementLayer(PlayerAnimator, AnimationConfiguration, LevelType.ADVENTURE);
        }

        public Animator GetPlayerAnimator()
        {
            return _animationDataManager.Animator;
        }
    }
}