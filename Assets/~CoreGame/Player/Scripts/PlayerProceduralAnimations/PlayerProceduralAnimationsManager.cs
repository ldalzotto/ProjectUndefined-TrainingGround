using UnityEngine;
using static AnimationConstants;

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

        public PlayerProceduralAnimationsManager(PlayerCommonComponents playerCommonComponents, TransformMoveManagerComponentV2 playerInputMoveManagerComponentV2, Animator playerAnimator, Rigidbody playerRigidBody, CoreConfigurationManager CoreConfigurationManager)
        {
            this.PlayerCommonComponents = playerCommonComponents;

            var hairObject = playerRigidBody.gameObject.FindChildObjectRecursively(AnimationConstants.HAIR_OBJECT_NAME);
            var chestObject = BipedBoneRetriever.GetPlayerBone(BipedBone.CHEST, playerAnimator);

            HairObjectAnimationTracker = new AnimationPositionTrackerManager(hairObject);
            ChestObjectAnimationTracker = new AnimationPositionTrackerManager(chestObject);

            PlayerHairStrandAnimationManager = new PlayerHairStrandAnimationManager(HairObjectAnimationTracker, hairObject, this.PlayerCommonComponents.PlayerHairStrandAnimationManagerComponent);
            PlayerHoodAnimationManager = new PlayerHoodAnimationManager(this.PlayerCommonComponents.PlayerHoodAnimationManagerComponent, BipedBoneRetriever.GetPlayerBone(BipedBone.HOOD, playerAnimator).transform, playerRigidBody, playerInputMoveManagerComponentV2);
            PlayerJacketCordAnimationManager = new PlayerJacketCordAnimationManager(playerAnimator, ChestObjectAnimationTracker, this.PlayerCommonComponents.PlayerJacketCordAnimationManagerComponent, playerInputMoveManagerComponentV2, CoreConfigurationManager);
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
