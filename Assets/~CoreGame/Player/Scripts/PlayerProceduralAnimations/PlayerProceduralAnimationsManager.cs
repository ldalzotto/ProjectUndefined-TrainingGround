using UnityEngine;

namespace CoreGame
{
    public class PlayerProceduralAnimationsManager
    {
        #region Player Common component
        private PlayerCommonComponents PlayerCommonComponents;
        #endregion

        #region Internal Managers
        private PlayerHairStrandAnimationManager PlayerHairStrandAnimationManager;
        private PlayerHoodAnimationManager PlayerHoodAnimationManager;
        private PlayerJacketCordAnimationManager PlayerJacketCordAnimationManager;
        #endregion

        private AnimationPositionTrackerManager HairObjectAnimationTracker;
        private AnimationPositionTrackerManager ChestObjectAnimationTracker;

        public PlayerProceduralAnimationsManager(PlayerCommonComponents playerCommonComponents, Animator playerAnimator, Rigidbody playerRigidBody)
        {
            this.PlayerCommonComponents = playerCommonComponents;

            var hairObject = playerRigidBody.gameObject.FindChildObjectRecursively(AnimationConstants.PlayerAnimationConstantsData.HAIR_OBJECT_NAME);
            var chestObject = PlayerBoneRetriever.GetPlayerBone(PlayerBone.CHEST, playerAnimator);

            HairObjectAnimationTracker = new AnimationPositionTrackerManager(hairObject);
            ChestObjectAnimationTracker = new AnimationPositionTrackerManager(chestObject);

            PlayerHairStrandAnimationManager = new PlayerHairStrandAnimationManager(HairObjectAnimationTracker, hairObject, this.PlayerCommonComponents.PlayerHairStrandAnimationManagerComponent);
            PlayerHoodAnimationManager = new PlayerHoodAnimationManager(this.PlayerCommonComponents.PlayerHoodAnimationManagerComponent, PlayerBoneRetriever.GetPlayerBone(PlayerBone.HOOD, playerAnimator).transform, playerRigidBody, this.PlayerCommonComponents.PlayerInputMoveManagerComponent);
            PlayerJacketCordAnimationManager = new PlayerJacketCordAnimationManager(playerAnimator, ChestObjectAnimationTracker, this.PlayerCommonComponents.PlayerJacketCordAnimationManagerComponent, this.PlayerCommonComponents.PlayerInputMoveManagerComponent);
        }

        public void FickedTick(float d)
        {
            #region Trackers Update
            HairObjectAnimationTracker.FixedTick(d);
            ChestObjectAnimationTracker.FixedTick(d);
            #endregion
        }

        public void LateTick(float d)
        {
            PlayerHairStrandAnimationManager.LateTick(d);
            PlayerHoodAnimationManager.LateTick(d);
            PlayerJacketCordAnimationManager.LateTick(d);
        }
    }
}
