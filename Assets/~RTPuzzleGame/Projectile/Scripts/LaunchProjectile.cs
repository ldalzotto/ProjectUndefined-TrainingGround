using UnityEngine;

namespace RTPuzzle
{
    public class LaunchProjectile : MonoBehaviour
    {
        private LaunchProjectileInherentData launchProjectileInherentData;

        public LaunchProjectileInherentData LaunchProjectileInherentData { get => launchProjectileInherentData; }

        private LaunchProjectileMovementManager LaunchProjectileMovementManager;
        private SphereCollisionManager SphereCollisionManager;

        public void Init(LaunchProjectileInherentData LaunchProjectileInherentData, BeziersControlPoints ProjectilePath)
        {
            this.launchProjectileInherentData = LaunchProjectileInherentData;
            var sphereCollider = GetComponent<SphereCollider>();
            //    sphereCollider.radius = 1f;// this.launchProjectileInherentData.EffectRange;
            this.transform.position = ProjectilePath.ResolvePoint(0.1f);
            var projectilePathDeepCopy = ProjectilePath.Clone();

            this.SphereCollisionManager = new SphereCollisionManager(sphereCollider, this.launchProjectileInherentData);
            this.LaunchProjectileMovementManager = new LaunchProjectileMovementManager(this.launchProjectileInherentData, transform, projectilePathDeepCopy);

        }

        public void Tick(float d, float timeAttenuationFactor)
        {
            LaunchProjectileMovementManager.Tick(d, timeAttenuationFactor);
        }

        public static LaunchProjectile GetFromCollisionType(CollisionType collisionType)
        {
            return collisionType.GetComponent<LaunchProjectile>();
        }

        #region External Events
        public void OnGroundTriggerEnter()
        {
            SphereCollisionManager.OnGroundTriggerEnter();
        }
        #endregion

    }

    #region Projectile movement manager
    class LaunchProjectileMovementManager
    {
        private LaunchProjectileInherentData LaunchProjectileInherentData;
        private Transform projectileTransform;
        private BeziersControlPoints ProjectilePath;

        public LaunchProjectileMovementManager(LaunchProjectileInherentData launchProjectileInherentData, Transform projectileTransform, BeziersControlPoints projectilePath)
        {
            LaunchProjectileInherentData = launchProjectileInherentData;
            this.projectileTransform = projectileTransform;
            ProjectilePath = projectilePath;
            currentProjectilePathBeziersPosition = 0.1f;
            computedProjectileSpeed = LaunchProjectileInherentData.TravelDistancePerSeconds / Vector3.Distance(projectilePath.P0, projectilePath.P3);
        }

        private float computedProjectileSpeed;
        private float currentProjectilePathBeziersPosition = 0f;

        public void Tick(float d, float timeAttenuationFactor)
        {
            currentProjectilePathBeziersPosition += computedProjectileSpeed * d * timeAttenuationFactor;
            projectileTransform.position = ProjectilePath.ResolvePoint(currentProjectilePathBeziersPosition);
        }

    }
    #endregion

    class SphereCollisionManager
    {
        private SphereCollider SphereCollider;
        private LaunchProjectileInherentData LaunchProjectileInherentData;

        public SphereCollisionManager(SphereCollider sphereCollider, LaunchProjectileInherentData LaunchProjectileInherentData)
        {
            SphereCollider = sphereCollider;
            this.LaunchProjectileInherentData = LaunchProjectileInherentData;
            SphereCollider.radius = 1f;
        }

        public void OnGroundTriggerEnter()
        {
            SphereCollider.radius = LaunchProjectileInherentData.EffectRange;
        }
    }
}
