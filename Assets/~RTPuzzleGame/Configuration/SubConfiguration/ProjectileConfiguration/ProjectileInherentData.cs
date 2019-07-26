using ConfigurationEditor;
using UnityEngine;
using GameConfigurationID;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ProjectileInherentData", menuName = "Configuration/PuzzleGame/ProjectileConfiguration/ProjectileInherentData", order = 1)]
    public class ProjectileInherentData : ScriptableObject
    {
        [SerializeField]
        private float effectRange;

        [SerializeField]
        private float projectileThrowRange;

        [SerializeField]
        private float travelDistancePerSeconds;
        
        [SerializeField]
        private InteractiveObjectType projectilePrefabV2;

        [SerializeField]
        private GameObject projectileModelPrefab;

        [Header("Animation")]

        [CustomEnum]
        public AnimationID PreActionAnimation;
        [CustomEnum]
        public AnimationID PostActionAnimation;

        [DictionaryEnumSearch]
        public float EffectRange { get => effectRange; }
        [DictionaryEnumSearch]
        public float TravelDistancePerSeconds { get => travelDistancePerSeconds; }
        public float ProjectileThrowRange { get => projectileThrowRange; }
        public GameObject ProjectileModelPrefab { get => projectileModelPrefab; }
        public InteractiveObjectType ProjectilePrefabV2 { get => projectilePrefabV2; set => projectilePrefabV2 = value; }

#if UNITY_EDITOR
        public void SetProjectileModelPrefab(GameObject ProjectileModelPrefab)
        {
            this.projectileModelPrefab = ProjectileModelPrefab;
            EditorUtility.SetDirty(this);
        }
#endif

        public void Init(float effectRange, float projectileThrowRange, float travelDistancePerSeconds, InteractiveObjectType projectilePrefab)
        {
            this.effectRange = effectRange;
            this.projectileThrowRange = projectileThrowRange;
            this.travelDistancePerSeconds = travelDistancePerSeconds;
            this.projectilePrefabV2 = projectilePrefab;
        }

        #region Debug purposes
        public void SetTravelDistanceDebug(float value)
        {
            travelDistancePerSeconds = value;
        }
        #endregion
    }
}
