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

    }

    public class LaunchProjectileInherentData
    {
        private float effectRange;

        public LaunchProjectileInherentData(float effectRange)
        {
            this.effectRange = effectRange;
        }

        public float EffectRange { get => effectRange; }
    }

    public enum LaunchProjectileId
    {
        STONE
    }

    public class LaunchProjectileInherentDataConfiguration
    {
        public static Dictionary<LaunchProjectileId, LaunchProjectileInherentData> conf = new Dictionary<LaunchProjectileId, LaunchProjectileInherentData>()
        {
            {LaunchProjectileId.STONE, new LaunchProjectileInherentData(8.230255f) }
        };
    }

}
