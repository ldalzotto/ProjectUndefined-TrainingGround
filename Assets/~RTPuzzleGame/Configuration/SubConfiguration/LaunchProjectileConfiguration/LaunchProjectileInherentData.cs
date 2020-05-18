﻿using UnityEngine;
using GameConfigurationID;
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

        [FormerlySerializedAs("ProjectilePrefabV2")]
        [SerializeField]
        public InteractiveObjectType AssociatedInteractiveObjectType;

        [SerializeField]
        public bool isExploding;

        [FormerlySerializedAs("EffectRange")]
        [SerializeField]
        public float ExplodingEffectRange;

        [SerializeField]
        public bool isPersistingToAttractiveObject;

        
        [Header("Animation")]

        [CustomEnum]
        public AnimationID PreActionAnimation;
        [CustomEnum]
        public AnimationID PostActionAnimation;
        
    }
    
}
