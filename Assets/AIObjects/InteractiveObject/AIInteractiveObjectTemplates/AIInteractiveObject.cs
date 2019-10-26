﻿using AnimatorPlayable;
using InteractiveObject_Animation;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using VisualFeedback;

namespace AIObjects
{
    public abstract class A_AIInteractiveObject<INIT> : CoreInteractiveObject where INIT : AbstractAIInteractiveObjectInitializerData
    {
        protected INIT AIInteractiveObjectInitializerData;
        protected AIMoveToDestinationSystem AIMoveToDestinationSystem;

        [VE_Nested] protected BaseObjectAnimatorPlayableSystem BaseObjectAnimatorPlayableSystem;
        protected LineVisualFeedbackSystem LineVisualFeedbackSystem;

        public A_AIInteractiveObject(IInteractiveGameObject interactiveGameObject, INIT AIInteractiveObjectInitializerData) : base(interactiveGameObject)
        {
            interactiveGameObject.CreateAgent(AIInteractiveObjectInitializerData.AIAgentDefinition);
            interactiveGameObject.CreateLogicCollider(AIInteractiveObjectInitializerData.InteractiveObjectLogicCollider);
            this.AIInteractiveObjectInitializerData = AIInteractiveObjectInitializerData;
            AIMoveToDestinationSystem = new AIMoveToDestinationSystem(this, AIInteractiveObjectInitializerData, OnAIDestinationReached);
            LineVisualFeedbackSystem = new LineVisualFeedbackSystem(InteractiveGameObject);
          
            if (InteractiveGameObject.Animator != null)
            {
                this.AnimatorPlayable = new AnimatorPlayableObject(InteractiveGameObject.InteractiveGameObjectParent.name, InteractiveGameObject.Animator);
            }

            this.AnimationController = new AnimationController(InteractiveGameObject.Agent, this.AnimatorPlayable, InteractiveGameObject.PhysicsRigidbody);
            InteractiveObjectEventsManager.OnInteractiveObjectCreated(this);
            
            this.BaseObjectAnimatorPlayableSystem = new BaseObjectAnimatorPlayableSystem(this.AnimatorPlayable, AIInteractiveObjectInitializerData.LocomotionAnimation);
        }

        public override void Tick(float d)
        {
            base.Tick(d);
            this.BaseObjectAnimatorPlayableSystem.Tick(d);
            LineVisualFeedbackSystem.Tick(d);
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

        public override void AfterTicks(float d)
        {
            AIMoveToDestinationSystem.AfterTicks();
            base.AfterTicks(d);
        }

        public override void Destroy()
        {
            LineVisualFeedbackSystem.OnDestroy();
            base.Destroy();
        }
    }
}