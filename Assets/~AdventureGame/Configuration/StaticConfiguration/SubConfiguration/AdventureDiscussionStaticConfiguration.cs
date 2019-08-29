using OdinSerializer;
using UnityEngine;

namespace AdventureGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AdventureDiscussionStaticConfiguration", menuName = "Configuration/AdventureGame/AdventureStaticConfiguration/AdventureDiscussionStaticConfiguration", order = 1)]
    public class AdventureDiscussionStaticConfiguration : SerializedScriptableObject
    {
        public AnimationCurve ModelScaleAnimation;
        public float ModelScaleAnimationTotalTime;
    }
}
