using System;
using AnimatorPlayable;

namespace InteractiveObjectsAnimatorPlayable
{
    public interface IObjectAnimatorPlayableSystem
    {
        void Tick(float d, float normalizedObjectSpeed);
        void PlayContextAction(SequencedAnimationInput ContextActionAnimation, Action OnAnimationFinished = null);
    }
}