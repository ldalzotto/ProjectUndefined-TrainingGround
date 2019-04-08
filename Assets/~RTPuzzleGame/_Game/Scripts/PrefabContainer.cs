﻿using UnityEngine;

namespace RTPuzzle
{

    public class PrefabContainer : MonoBehaviour
    {
        private static PrefabContainer instance;

        [Header("Projectile Prefabs")]
        [Space(20)]
        [Header("RT Puzzle Prefabs")]
        [Space(20)]
        public ThrowProjectilePath ThrowProjectilePathPrefab;
        public ThrowProjectileCursorType ThrowProjectileCursorTypePrefab;

        [Header("RT Puzzle Target Zone Prefab")]
        [Space(20)]
        public TargetZone TargetZonePrefab;


        [Header("Puzzle attracitve objects prefabs")]

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
