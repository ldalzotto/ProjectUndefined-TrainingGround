using UnityEngine;
using System.Collections;

namespace RTPuzzle {
    
    public class PrefabContainer : MonoBehaviour
    {
        private static PrefabContainer instance;

        [Header("Projectile Prefabs")]
        [Space(20)]
        [Header("RT Puzzle Prefabs")]
        [Space(20)]
        public LaunchProjectile ProjectilePrefab;
        public ThrowProjectilePath ThrowProjectilePathPrefab;
        public ThrowProjectileCursorType ThrowProjectileCursorTypePrefab;

        [Header("Puzzle attracitve objects prefabs")]
        public AttractiveObjectType AttractiveObjectPrefab;

        [Header("NPC prefabs")]
        public NpcInteractionRingType NpcInteractionRingPrefab;

        [Header("Cooldown Feed Prefabs")]
        public CooldownFeedManager CooldownFeedManager;
        public CooldownFeedLineType CooldownFeedLineType;

        [Header("AI Mark Feedback Prefabs")]
        public AIFeedbackMarkType ExclamationMarkSimple;
        public AIFeedbackMarkType ExclamationMarkDouble;
        public AIFeedbackMarkType LoveCheese;

        [Header("Visual Feedback")]
        public LevelCompleteVisualFeedbackFX LevelCompletedParticleEffect;

        public static PrefabContainer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<PrefabContainer>();
                }
                return instance;
            }
        }
    }

}
