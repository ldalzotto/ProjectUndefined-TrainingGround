using GameConfigurationID;
using UnityEngine;
using UnityEngine.Serialization;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LaunchProjectileInherentData", menuName = "Configuration/PuzzleGame/LaunchProjectileConfiguration/LaunchProjectileInherentData", order = 1)]
    public class LaunchProjectileInherentData : ScriptableObject
    {

        [WireCircle(R = 1, G = 0, B = 1)]
        [SerializeField]
        public float ProjectileThrowRange;

        [SerializeField]
        public float TravelDistancePerSeconds;

        [SerializeField]
        public bool isExploding;

        [FormerlySerializedAs("EffectRange")]
        [WireCircle(R = 1, G = 0, B = 1)]
        [SerializeField]
        public float ExplodingEffectRange;

        [SerializeField]
        public bool isPersistingToAttractiveObject;


        [Header("Animation_Parameters")]
        [CustomEnum]
        public AnimationID PreActionAnimation;
        [CustomEnum]
        public AnimationID PostActionAnimation;

        [Header("Animation Graphs")]
        [CustomEnum(ConfigurationType = typeof(PuzzleCutsceneConfiguration))]
        public PuzzleCutsceneID PreActionAnimationV2;

        [CustomEnum(ConfigurationType = typeof(PuzzleCutsceneConfiguration))]
        public PuzzleCutsceneID PostActionAnimationV2;



    }

}
