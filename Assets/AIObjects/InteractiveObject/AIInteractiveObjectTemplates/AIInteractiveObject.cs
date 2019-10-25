﻿using InteractiveObjects;
using InteractiveObjects_Interfaces;
using InteractiveObjectsAnimatorPlayable;
using VisualFeedback;

namespace AIObjects
{
    public abstract class A_AIInteractiveObject<INIT> : CoreInteractiveObject where INIT : AbstractAIInteractiveObjectInitializerData
    {
        protected INIT AIInteractiveObjectInitializerData;
        protected AIMoveToDestinationSystem AIMoveToDestinationSystem;

        [VE_Nested] protected MovingObjectAnimatorPlayableSystem MovingObjectAnimatorPlayableSystem;
        protected LineVisualFeedbackSystem LineVisualFeedbackSystem;

        public A_AIInteractiveObject(IInteractiveGameObject interactiveGameObject, INIT AIInteractiveObjectInitializerData) : base(interactiveGameObject)
        {
            interactiveGameObject.CreateAgent(AIInteractiveObjectInitializerData.AIAgentDefinition);
            interactiveGameObject.CreateLogicCollider(AIInteractiveObjectInitializerData.InteractiveObjectLogicCollider);
            this.AIInteractiveObjectInitializerData = AIInteractiveObjectInitializerData;
            this.MovingObjectAnimatorPlayableSystem = new MovingObjectAnimatorPlayableSystem(interactiveGameObject.InteractiveGameObjectParent.name, interactiveGameObject.Animator, AIInteractiveObjectInitializerData.LocomotionAnimation);
            AIMoveToDestinationSystem = new AIMoveToDestinationSystem(this, AIInteractiveObjectInitializerData, OnAIDestinationReached);
            LineVisualFeedbackSystem = new LineVisualFeedbackSystem(InteractiveGameObject);
        }

        public override void Tick(float d)
        {
            this.MovingObjectAnimatorPlayableSystem.Tick(d);
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
            this.MovingObjectAnimatorPlayableSystem.SetUnscaledObjectSpeed(unscaledSpeedMagnitude);
        }

        public override void AfterTicks()
        {
            AIMoveToDestinationSystem.AfterTicks();
        }

        public override void Destroy()
        {
            LineVisualFeedbackSystem.OnDestroy();
            base.Destroy();
        }
    }
}