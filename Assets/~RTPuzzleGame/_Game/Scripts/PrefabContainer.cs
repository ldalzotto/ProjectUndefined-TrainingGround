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

        [Header("NPC prefabs")]
        public RTPuzzle.NpcInteractionRingType NpcInteractionRingPrefab;

        [Header("Cooldown Feed Prefabs")]
        public CooldownFeedManager CooldownFeedManager;
        public CooldownFeedLineType CooldownFeedLineType;

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
