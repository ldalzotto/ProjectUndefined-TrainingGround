using CoreGame;

namespace RTPuzzle
{
    public class InteractiveObjectAnimationModule : InteractiveObjectModule, IInteractiveObjectAnimationModuleEvent
    {
        private ModelObjectModule ModelObjectModule;
        private InteractiveObjectSharedDataType InteractiveObjectSharedDataType;

        private IInteractiveObjectAnimationSpeedProvider IInteractiveObjectAnimationSpeedProvider;
        private PlayerAnimationDataManager animationDataManager;

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval,
                IInteractiveObjectTypeEvents IInteractiveObjectTypeEvents)
        {
            this.ModelObjectModule = IInteractiveObjectTypeDataRetrieval.GetModelObjectModule();
            this.InteractiveObjectSharedDataType = interactiveObjectInitializationObject.TransformMoveManagerComponent;

            this.animationDataManager = new PlayerAnimationDataManager(this.ModelObjectModule.Animator);
            //Initialize movement animation
            GenericAnimatorHelper.SetMovementLayer(this.ModelObjectModule.Animator, CoreGameSingletonInstances.CoreConfigurationManager.CoreConfiguration.AnimationConfiguration, LevelType.PUZZLE);
        }

        #region IInteractiveObjectAnimationModuleEvents
        public void SetIInteractiveObjectAnimationSpeedProvider(IInteractiveObjectAnimationSpeedProvider IInteractiveObjectAnimationSpeedProvider)
        {
            this.IInteractiveObjectAnimationSpeedProvider = IInteractiveObjectAnimationSpeedProvider;
        }
        #endregion

        public void TickAlways(float d)
        {
            if (this.IInteractiveObjectAnimationSpeedProvider != null)
            {
                this.animationDataManager.Tick(this.IInteractiveObjectAnimationSpeedProvider.GetNormalizedSpeed());
            }
            else
            {
                this.animationDataManager.Tick(this.ModelObjectModule.AssociatedAgent.velocity.magnitude / this.InteractiveObjectSharedDataType.InteractiveObjectSharedDataTypeInherentData.TransformMoveManagerComponent.SpeedMultiplicationFactor);
            }
        }
    }

    public interface IInteractiveObjectAnimationSpeedProvider
    {
        float GetNormalizedSpeed();
    }
}
