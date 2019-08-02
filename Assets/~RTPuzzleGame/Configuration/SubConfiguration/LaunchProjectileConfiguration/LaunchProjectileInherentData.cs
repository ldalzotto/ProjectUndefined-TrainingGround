using UnityEngine;
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

        [SerializeField]
        public bool isPersistingToAttractiveObject;

        [SerializeField]
        public float EffectRange;
        
        [Header("Animation")]

        [CustomEnum]
        public AnimationID PreActionAnimation;
        [CustomEnum]
        public AnimationID PostActionAnimation;

        public void Init(float effectRange, float projectileThrowRange, float travelDistancePerSeconds, InteractiveObjectType projectilePrefab, bool isExploding, bool isPersistingToAttractiveObject)
        {
            this.EffectRange = effectRange;
            this.ProjectileThrowRange = projectileThrowRange;
            this.TravelDistancePerSeconds = travelDistancePerSeconds;
            this.AssociatedInteractiveObjectType = projectilePrefab;
            this.isExploding = isExploding;
            this.isPersistingToAttractiveObject = isPersistingToAttractiveObject;
        }

        #region Debug purposes
        public void SetTravelDistanceDebug(float value)
        {
            TravelDistancePerSeconds = value;
        }
        #endregion
    }
    
}
