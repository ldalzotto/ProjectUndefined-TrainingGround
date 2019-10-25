using System;

namespace AnimatorPlayable
{
    [Serializable]
    public class SequencedAnimationPlayableDefinition : A_AnimationPlayableDefinition
    {
        public SequencedAnimationInput SequencedAnimationInput;

        public override void Play(int layerID, AnimatorPlayableObject AnimatorPlayableObject)
        {
            AnimatorPlayableObject.PlaySequencedAnimation(layerID, this.SequencedAnimationInput);
        }
    }
}