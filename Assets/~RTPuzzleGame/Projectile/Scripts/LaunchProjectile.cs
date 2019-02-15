using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class LaunchProjectile : MonoBehaviour
    {
        private LaunchProjectileInherentData LaunchProjectileInherentData;

        public void Init(LaunchProjectileInherentData LaunchProjectileInherentData)
        {
            this.LaunchProjectileInherentData = LaunchProjectileInherentData;
            var sphereCollider = GetComponent<SphereCollider>();
            sphereCollider.radius = this.LaunchProjectileInherentData.EffectRange;
        }

        public float GetEffectRange()
        {
            return LaunchProjectileInherentData.EffectRange;
        }

        public float GetEscapeSemiAngle()
        {
            return LaunchProjectileInherentData.EscapeSemiAngle;
        }

        public static LaunchProjectile GetFromCollisionType(CollisionType collisionType)
        {
            return collisionType.GetComponent<LaunchProjectile>();
        }

    }

    public class LaunchProjectileInherentData
    {
        private float effectRange;
        private float escapeSemiAngle;

        public LaunchProjectileInherentData(float effectRange, float escapeSemiAngle)
        {
            this.effectRange = effectRange;
            this.escapeSemiAngle = escapeSemiAngle;
        }

        public float EffectRange { get => effectRange; }
        public float EscapeSemiAngle { get => escapeSemiAngle; }
    }

    public enum LaunchProjectileId
    {
        STONE
    }

    public class LaunchProjectileInherentDataConfiguration
    {
        public static Dictionary<LaunchProjectileId, LaunchProjectileInherentData> conf = new Dictionary<LaunchProjectileId, LaunchProjectileInherentData>()
        {
            {LaunchProjectileId.STONE, new LaunchProjectileInherentData(8.230255f, 90f) }
        };
    }

}
