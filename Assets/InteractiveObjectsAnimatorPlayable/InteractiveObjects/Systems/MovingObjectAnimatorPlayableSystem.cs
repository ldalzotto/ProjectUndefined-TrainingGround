using System;
using AnimatorPlayable;
using InteractiveObjects_Interfaces;

namespace InteractiveObjectsAnimatorPlayable
{
    public class MovingObjectAnimatorPlayableSystem : AInteractiveObjectSystem
    {
        private float normalizedObjectSpeed;

        public MovingObjectAnimatorPlayableSystem(AnimatorPlayableObject AnimatorPlayableObject, A_AnimationPlayableDefinition LocomotionAnimationDefinition)
        {
            LocomotionAnimationDefinition.Play(AnimationLayerStatic.AnimationLayers[AnimationLayerID.LocomotionLayer].ID, AnimatorPlayableObject);
            AnimatorPlayableObject.AllAnimationLayersCurrentlyPlaying[AnimationLayerStatic.AnimationLayers[AnimationLayerID.LocomotionLayer].ID].RegisterInputWeightProvider(() => this.normalizedObjectSpeed);
        }

        public void SetUnscaledObjectSpeed(float normalizedObjectSpeed)
        {
            this.normalizedObjectSpeed = normalizedObjectSpeed;
        }

        public void PlayContextAction(AnimatorPlayableObject AnimatorPlayableObject, SequencedAnimationInput ContextActionAnimation, Action OnAnimationFinished = null)
        {
            AnimatorPlayableObject.PlaySequencedAnimation(AnimationLayerStatic.AnimationLayers[AnimationLayerID.ContextActionLayer].ID, ContextActionAnimation);
            if (OnAnimationFinished != null)
            {
                AnimatorPlayableObject.AllAnimationLayersCurrentlyPlaying[AnimationLayerStatic.AnimationLayers[AnimationLayerID.ContextActionLayer].ID]
                    .ReigsterOnSequencedAnimationEnd(OnAnimationFinished);
            }
        }
    }
}