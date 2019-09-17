using CoreGame;

namespace RTPuzzle
{
    public class InteractiveObjectAnimationModule : InteractiveObjectModule
    {
        private ModelObjectModule ModelObjectModule;
        private InteractiveObjectSharedDataType InteractiveObjectSharedDataType;
        
        private PlayerAnimationDataManager NPCPAnimationDataManager;

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval,
                IInteractiveObjectTypeEvents IInteractiveObjectTypeEvents)
        {
            this.ModelObjectModule = IInteractiveObjectTypeDataRetrieval.GetModelObjectModule();
            this.InteractiveObjectSharedDataType = interactiveObjectInitializationObject.TransformMoveManagerComponent;

            this.NPCPAnimationDataManager = new PlayerAnimationDataManager(this.ModelObjectModule.Animator);
            //Initialize movement animation
            GenericAnimatorHelper.SetMovementLayer(this.ModelObjectModule.Animator, CoreGameSingletonInstances.CoreConfigurationManager.CoreConfiguration.AnimationConfiguration, LevelType.PUZZLE);
        }

        public void TickAlways(float d)
        {
            this.NPCPAnimationDataManager.Tick(this.ModelObjectModule.AssociatedAgent.velocity.magnitude / this.InteractiveObjectSharedDataType.InteractiveObjectSharedDataTypeInherentData.TransformMoveManagerComponent.SpeedMultiplicationFactor);
        }

    }
}
