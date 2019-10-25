using System;
using AnimatorPlayable;
using InteractiveObjects_Interfaces;
using UnityEngine;

namespace InteractiveObjectsAnimatorPlayable
{
    public class MovingObjectAnimatorPlayableSystem : AInteractiveObjectSystem
    {
        [VE_Ignore] private AnimatorPlayableObject AnimatorPlayableObject;

        private float normalizedObjectSpeed;

        public MovingObjectAnimatorPlayableSystem(string graphName, Animator animator, A_AnimationPlayableDefinition LocomotionAnimationDefinition)
        {
            this.AnimatorPlayableObject = new AnimatorPlayableObject(graphName, animator);
            LocomotionAnimationDefinition.Play(AnimationLayerStatic.AnimationLayers[AnimationLayerID.LocomotionLayer].ID, this.AnimatorPlayableObject);
            this.AnimatorPlayableObject.AllAnimationLayersCurrentlyPlaying[AnimationLayerStatic.AnimationLayers[AnimationLayerID.LocomotionLayer].ID].RegisterInputWeightProvider(() => this.normalizedObjectSpeed);
        }

        public void SetUnscaledObjectSpeed(float normalizedObjectSpeed)
        {
            this.normalizedObjectSpeed = normalizedObjectSpeed;
        }

        public override void Tick(float d)
        {
            this.AnimatorPlayableObject.Tick(d);
        }

        public void PlayContextAction(SequencedAnimationInput ContextActionAnimation, Action OnAnimationFinished = null)
        {
            this.AnimatorPlayableObject.PlaySequencedAnimation(AnimationLayerStatic.AnimationLayers[AnimationLayerID.ContextActionLayer].ID, ContextActionAnimation);
            if (OnAnimationFinished != null)
            {
                this.AnimatorPlayableObject.AllAnimationLayersCurrentlyPlaying[AnimationLayerStatic.AnimationLayers[AnimationLayerID.ContextActionLayer].ID]
                    .ReigsterOnAnimationEnd(OnAnimationFinished);
            }
        }

        public override void OnDestroy()
        {
            this.AnimatorPlayableObject.Destroy();
        }
    }
}