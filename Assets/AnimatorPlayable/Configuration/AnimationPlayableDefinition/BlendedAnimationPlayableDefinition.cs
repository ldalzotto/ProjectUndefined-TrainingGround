using System;

namespace AnimatorPlayable
{
    [Serializable]
    public class BlendedAnimationPlayableDefinition : A_AnimationPlayableDefinition
    {
        public BlendedAnimationInput BlendedAnimationInput;

        public override void Play(int layerID, AnimatorPlayableObject AnimatorPlayableObject)
        {
            AnimatorPlayableObject.PlayBlendedAnimation(layerID, this.BlendedAnimationInput);
        }
    }
}