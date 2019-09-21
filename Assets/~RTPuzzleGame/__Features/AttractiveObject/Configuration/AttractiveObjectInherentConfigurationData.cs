using GameConfigurationID;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AttractiveObjectInherentConfigurationData", menuName = "Configuration/PuzzleGame/AttractiveObjectConfiguration/AttractiveObjectInherentConfigurationData", order = 1)]
    public class AttractiveObjectInherentConfigurationData : ScriptableObject
    {
        [WireArc(R = 1, G = 0, B = 1)]
        public float EffectRange;
        public float EffectiveTime;

        [Header("Animation_Graph")]
        [CustomEnum(ConfigurationType = typeof(PuzzleCutsceneConfiguration))]
        public PuzzleCutsceneID PreActionAnimationGraph;
        [CustomEnum(ConfigurationType = typeof(PuzzleCutsceneConfiguration))]
        public PuzzleCutsceneID PostActionAnimationGraph;

        [Header("Animation_Parameters")]
        [CustomEnum]
        public AnimationID PreActionAnimation;
        [CustomEnum]
        public AnimationID PostActionAnimation;

    }
}
