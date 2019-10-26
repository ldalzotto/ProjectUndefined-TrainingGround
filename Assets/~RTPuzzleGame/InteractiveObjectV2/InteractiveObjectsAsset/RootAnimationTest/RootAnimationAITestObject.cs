using AIObjects;
using AnimatorPlayable;
using InteractiveObject_Animation;
using InteractiveObjects_Interfaces;
using UnityEngine;

namespace InteractiveObjects
{
    public class RootAnimationAITestObject : CoreInteractiveObject
    {
        private RootAnimationAITestObjectInitializerData RootAnimationAITestObjectInitializerData;
        private LocalCutscenePlayerSystem LocalCutscenePlayerSystem;
        [VE_Nested] private AIMoveToDestinationSystem AIMoveToDestinationSystem;
        private BaseObjectAnimatorPlayableSystem BaseObjectAnimatorPlayableSystem;

        public RootAnimationAITestObject(IInteractiveGameObject interactiveGameObject, RootAnimationAITestObjectInitializerData AIInteractiveObjectInitializerData)
            : base(interactiveGameObject, AIInteractiveObjectInitializerData)
        {
            this.interactiveObjectTag = new InteractiveObjectTag() {IsAi = true};
            this.RootAnimationAITestObjectInitializerData = AIInteractiveObjectInitializerData;
            this.LocalCutscenePlayerSystem = new LocalCutscenePlayerSystem();

            interactiveGameObject.CreateAgent(AIInteractiveObjectInitializerData.AIAgentDefinition);
            interactiveGameObject.CreateLogicCollider(AIInteractiveObjectInitializerData.InteractiveObjectLogicCollider);

            this.AIMoveToDestinationSystem = new AIMoveToDestinationSystem(this, AIInteractiveObjectInitializerData, OnAIDestinationReached);

            if (InteractiveGameObject.Animator != null)
            {
                this.AnimatorPlayable = new AnimatorPlayableObject(InteractiveGameObject.InteractiveGameObjectParent.name, InteractiveGameObject.Animator);
            }

            this.AnimationController = new AnimationController(InteractiveGameObject.Agent, this.AnimatorPlayable, InteractiveGameObject.PhysicsRigidbody, this.OnRootMotionEnabled, this.OnRootMotionDisabled);
            InteractiveObjectEventsManager.OnInteractiveObjectCreated(this);

            this.BaseObjectAnimatorPlayableSystem = new BaseObjectAnimatorPlayableSystem(this.AnimatorPlayable, AIInteractiveObjectInitializerData.LocomotionAnimation);

            this.LocalCutscenePlayerSystem.PlayCutscene(this.RootAnimationAITestObjectInitializerData.RootAnimationCutsceneTemplate.GetSequencedActions(this));
        }

        public override void Tick(float d)
        {
            this.AIMoveToDestinationSystem.Tick(d);

            this.LocalCutscenePlayerSystem.Tick(d);
            this.BaseObjectAnimatorPlayableSystem.Tick(d);
        }

        public override void AfterTicks(float d)
        {
            this.AIMoveToDestinationSystem.AfterTicks();
            if (this.AnimatorPlayable != null)
            {
                this.AnimatorPlayable.Tick(d);
                this.AnimationController.Tick(d);
            }
        }

        public override void Destroy()
        {
            InteractiveObjectEventsManager.OnInteractiveObjectDestroyed(this);
            if (this.AnimatorPlayable != null)
            {
                this.AnimatorPlayable.Destroy();
            }

            Object.Destroy(InteractiveGameObject.InteractiveGameObjectParent);
        }

        private void OnRootMotionEnabled()
        {
            this.AIMoveToDestinationSystem.IsEnabled = false;
        }

        private void OnRootMotionDisabled()
        {
            this.AIMoveToDestinationSystem.IsEnabled = true;
        }

        public override void OnAIDestinationReached()
        {
            this.LocalCutscenePlayerSystem.OnAIDestinationReached();
        }

        public override void SetAIDestination(AIDestination AIDestination)
        {
            AIMoveToDestinationSystem.SetDestination(AIDestination);
        }

        public override void SetAISpeedAttenuationFactor(AIMovementSpeedDefinition AIMovementSpeedDefinition)
        {
            AIMoveToDestinationSystem.SetSpeedAttenuationFactor(AIMovementSpeedDefinition);
        }

        public override void OnAnimationObjectSetUnscaledSpeedMagnitude(float unscaledSpeedMagnitude)
        {
            this.BaseObjectAnimatorPlayableSystem.SetUnscaledObjectSpeed(unscaledSpeedMagnitude);
        }
    }
}