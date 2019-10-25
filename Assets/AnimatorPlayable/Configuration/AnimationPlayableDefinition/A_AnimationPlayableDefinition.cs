using System;
using OdinSerializer;

namespace AnimatorPlayable
{
    [Serializable]
    public abstract class A_AnimationPlayableDefinition : SerializedScriptableObject
    {
        public abstract void Play(int layerID, AnimatorPlayableObject AnimatorPlayableObject);
    }
}