using CoreGame;

namespace InteractiveObjects
{
    public class AnimationObjectSystem : AInteractiveObjectSystem
    {
        [VE_Nested] private AnimationDataManager animationDataManager;

        #region State

        private float UnscaledSpeedMagnitude;

        #endregion


        public AnimationObjectSystem(CoreInteractiveObject AssociatedInteractiveObject)
        {
            var LevelManager = CoreGameSingletonInstances.LevelManager;

            var objectAnimator = AssociatedInteractiveObject.InteractiveGameObject.Animator;
            if (objectAnimator != null)
                GenericAnimatorHelper.SetMovementLayer(objectAnimator, CoreGameSingletonInstances.CoreConfigurationManager.CoreConfiguration.AnimationConfiguration,
                    LevelManager.CurrentLevelType);

            animationDataManager = new AnimationDataManager(objectAnimator);
        }

        public override void Tick(float d)
        {
            animationDataManager.Tick(UnscaledSpeedMagnitude);
        }

        public void SetUnscaledSpeedMagnitude(float UnscaledSpeedMagnitude)
        {
            this.UnscaledSpeedMagnitude = UnscaledSpeedMagnitude;
        }
    }
}