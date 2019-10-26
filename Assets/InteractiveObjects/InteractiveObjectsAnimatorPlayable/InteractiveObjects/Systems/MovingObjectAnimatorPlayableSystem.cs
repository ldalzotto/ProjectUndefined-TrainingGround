using AnimatorPlayable;
using InteractiveObjects_Interfaces;

namespace InteractiveObjectsAnimatorPlayable
{
    public class MovingObjectAnimatorPlayableSystem : AInteractiveObjectSystem
    {
        private float normalizedObjectSpeed;

        public MovingObjectAnimatorPlayableSystem(AnimatorPlayableObject AnimatorPlayableObject, A_AnimationPlayableDefinition LocomotionAnimationDefinition)
        {
            AnimatorPlayableObject.PlayAnimation(AnimationLayerStatic.AnimationLayers[AnimationLayerID.LocomotionLayer].ID, LocomotionAnimationDefinition.GetAnimationInput(),
                InputWeightProvider: () => this.normalizedObjectSpeed);
        }

        public void SetUnscaledObjectSpeed(float normalizedObjectSpeed)
        {
            this.normalizedObjectSpeed = normalizedObjectSpeed;
        }
    }
}