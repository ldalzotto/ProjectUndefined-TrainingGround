using CoreGame;
using UnityEngine;

namespace AdventureGame
{
    public class PlayerAnimationManager : MonoBehaviour
    {
        [Header("Animation")]
        private PlayerAnimationDataManager playerAnimationDataManager;

        #region FX Instanciation handler
        public PlayerAnimationDataManager PlayerAnimationDataManager { get => playerAnimationDataManager; }
        #endregion

        private void Start()
        {
            var AdventurePrefabConfiguration = AdventureGameSingletonInstances.AdventureStaticConfigurationContainer.AdventureStaticConfiguration.AdventurePrefabConfiguration;
            var AnimationConfiguration = CoreGameSingletonInstances.CoreConfigurationManager.AnimationConfiguration();
            var PlayerAnimator = GetComponentInChildren<Animator>();

            this.playerAnimationDataManager = new PlayerAnimationDataManager(PlayerAnimator);

            GenericAnimatorHelper.SetMovementLayer(PlayerAnimator, AnimationConfiguration, LevelType.ADVENTURE);
        }

        public Animator GetPlayerAnimator()
        {
            return playerAnimationDataManager.Animator;
        }

    }
}
