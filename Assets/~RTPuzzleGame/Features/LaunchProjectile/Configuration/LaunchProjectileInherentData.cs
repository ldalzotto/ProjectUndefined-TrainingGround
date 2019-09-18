using GameConfigurationID;
using UnityEngine;
using UnityEngine.Serialization;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LaunchProjectileInherentData", menuName = "Configuration/PuzzleGame/LaunchProjectileConfiguration/LaunchProjectileInherentData", order = 1)]
    public class LaunchProjectileInherentData : ScriptableObject
    {

        [SerializeField]
        public float ProjectileThrowRange;

        [SerializeField]
        public float TravelDistancePerSeconds;

        [SerializeField]
        public bool isExploding;

        [FormerlySerializedAs("EffectRange")]
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
