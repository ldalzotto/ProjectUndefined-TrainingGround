using CoreGame;

namespace InteractiveObjects
{
    public class AnimationObjectSystem : AInteractiveObjectSystem
    {
        #region State
        private float UnscaledSpeedMagnitude;
        #endregion

        private PlayerAnimationDataManager animationDataManager;
        public AnimationObjectSystem(CoreInteractiveObject AssociatedInteractiveObject)
        {
            var objectAnimator = AssociatedInteractiveObject.InteractiveGameObject.Animator;
            if (objectAnimator != null)
            {
                GenericAnimatorHelper.SetMovementLayer(objectAnimator, CoreGameSingletonInstances.CoreConfigurationManager.CoreConfiguration.AnimationConfiguration, LevelType.PUZZLE);
            }

            this.animationDataManager = new PlayerAnimationDataManager(objectAnimator);
        }

        public override void TickAlways(float d)
        {
            this.animationDataManager.Tick(this.UnscaledSpeedMagnitude);
        }

        public void SetUnscaledSpeedMagnitude(AnimationObjectSetUnscaledSpeedMagnitudeEvent AnimationObjectSetUnscaledSpeedMagnitudeEvent) { this.UnscaledSpeedMagnitude = AnimationObjectSetUnscaledSpeedMagnitudeEvent.UnscaledSpeedMagnitude; }
    }
}
