using ConfigurationEditor;
using UnityEngine;


namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ProjectileInherentData", menuName = "Configuration/PuzzleGame/ProjectileConfiguration/ProjectileInherentData", order = 1)]
    public class ProjectileInherentData : ScriptableObject
    {
        [SerializeField]
        private float effectRange;

        [SerializeField]
        private float escapeSemiAngle;

        [SerializeField]
        private float travelDistancePerSeconds;

        [SerializeField]
        private LaunchProjectile projectilePrefab;

        [DictionaryEnumSearch]
        public float EffectRange { get => effectRange; }
        [DictionaryEnumSearch]
        public float EscapeSemiAngle { get => escapeSemiAngle; }
        [DictionaryEnumSearch]
        public float TravelDistancePerSeconds { get => travelDistancePerSeconds; }
        public LaunchProjectile ProjectilePrefab { get => projectilePrefab; }

        public void Init(float effectRange, float escapeSemiAngle, float travelDistancePerSeconds, LaunchProjectile projectilePrefab)
        {
            this.effectRange = effectRange;
            this.escapeSemiAngle = escapeSemiAngle;
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
