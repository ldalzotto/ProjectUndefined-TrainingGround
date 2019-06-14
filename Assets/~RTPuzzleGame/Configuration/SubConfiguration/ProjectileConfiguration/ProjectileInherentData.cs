using ConfigurationEditor;
using UnityEngine;

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
        private LaunchProjectile projectilePrefab;

        [SerializeField]
        private GameObject projectileModelPrefab;

        [Header("Animation")]

        [SearchableEnum]
        public PlayerAnimatioNamesEnum PreActionAnimation;
        [SearchableEnum]
        public PlayerAnimatioNamesEnum PostActionAnimation;

        [DictionaryEnumSearch]
        public float EffectRange { get => effectRange; }
        [DictionaryEnumSearch]
        public float TravelDistancePerSeconds { get => travelDistancePerSeconds; }
        public LaunchProjectile ProjectilePrefab { get => projectilePrefab; }
        public float ProjectileThrowRange { get => projectileThrowRange; }
        public GameObject ProjectileModelPrefab { get => projectileModelPrefab; }

#if UNITY_EDITOR
        public void SetProjectileModelPrefab(GameObject ProjectileModelPrefab)
        {
            this.projectileModelPrefab = ProjectileModelPrefab;
            EditorUtility.SetDirty(this);
        }
#endif

        public void Init(float effectRange, float projectileThrowRange, float travelDistancePerSeconds, LaunchProjectile projectilePrefab)
        {
            this.effectRange = effectRange;
            this.projectileThrowRange = projectileThrowRange;
            this.travelDistancePerSeconds = travelDistancePerSeconds;
            this.projectilePrefab = projectilePrefab;
        }

        #region Debug purposes
        public void SetTravelDistanceDebug(float value)
        {
            travelDistancePerSeconds = value;
        }
        #endregion
    }
}
