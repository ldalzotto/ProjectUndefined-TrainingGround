﻿using UnityEngine;

namespace RTPuzzle
{
    public class LaunchProjectile : MonoBehaviour
    {
        private ProjectileInherentData launchProjectileInherentData;

        public ProjectileInherentData LaunchProjectileInherentData { get => launchProjectileInherentData; }

        private LaunchProjectileMovementManager LaunchProjectileMovementManager;
        private SphereCollisionManager SphereCollisionManager;

        #region External Dependencies
        private LaunchProjectileEventManager LaunchProjectileEventManager;
        #endregion

        public static LaunchProjectile Instantiate(ProjectileInherentData LaunchProjectileInherentData, BeziersControlPoints ProjectilePath, Canvas parentCanvas)
        {
            var launchProjectile = MonoBehaviour.Instantiate(PrefabContainer.Instance.ProjectilePrefab, parentCanvas.transform);
            launchProjectile.Init(LaunchProjectileInherentData, ProjectilePath);
            return launchProjectile;
        }

        public void Init(ProjectileInherentData LaunchProjectileInherentData, BeziersControlPoints ProjectilePath)
        {
            #region External Dependencies
            var npcAiManagerContainer = GameObject.FindObjectOfType<NPCAIManagerContainer>();
            this.LaunchProjectileEventManager = GameObject.FindObjectOfType<LaunchProjectileEventManager>();
            #endregion

            this.launchProjectileInherentData = LaunchProjectileInherentData;
            var sphereCollider = GetComponent<SphereCollider>();
            //    sphereCollider.radius = 1f;// this.launchProjectileInherentData.EffectRange;
            this.transform.position = ProjectilePath.ResolvePoint(0.1f);
            var projectilePathDeepCopy = ProjectilePath.Clone();

            this.SphereCollisionManager = new SphereCollisionManager(sphereCollider, this.launchProjectileInherentData, npcAiManagerContainer);
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
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == LayerMask.NameToLayer(LayerConstants.PUZZLE_GROUND_LAYER))
            {
                this.LaunchProjectileEventManager.OnProjectileGroundTriggerEnter(this);
            }
            
        }

        public void OnGroundTriggerEnter()
        {
            SphereCollisionManager.OnGroundTriggerEnter();
        }
        #endregion

    }

    #region Projectile movement manager
    class LaunchProjectileMovementManager
    {
        private ProjectileInherentData LaunchProjectileInherentData;
        private Transform projectileTransform;
        private BeziersControlPoints ProjectilePath;

        public LaunchProjectileMovementManager(ProjectileInherentData launchProjectileInherentData, Transform projectileTransform, BeziersControlPoints projectilePath)
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
            currentProjectilePathBeziersPosition = Mathf.Clamp01(currentProjectilePathBeziersPosition);
            projectileTransform.position = ProjectilePath.ResolvePoint(currentProjectilePathBeziersPosition);
        }

    }
    #endregion

    class SphereCollisionManager
    {
        private NPCAIManagerContainer NPCAIManagerContainer;
        private SphereCollider SphereCollider;
        private ProjectileInherentData LaunchProjectileInherentData;

        public SphereCollisionManager(SphereCollider sphereCollider, ProjectileInherentData LaunchProjectileInherentData, NPCAIManagerContainer NPCAIManagerContainer)
        {
            SphereCollider = sphereCollider;
            this.LaunchProjectileInherentData = LaunchProjectileInherentData;
            this.NPCAIManagerContainer = NPCAIManagerContainer;
            SphereCollider.radius = 1f;
        }

        public void OnGroundTriggerEnter()
        {
            SphereCollider.radius = LaunchProjectileInherentData.EffectRange;
            foreach (var npcAiManagerWithId in NPCAIManagerContainer.GetNPCAiManagers())
            {
                if (npcAiManagerWithId.Value.GetCollider().bounds.Intersects(SphereCollider.bounds))
                {
                    npcAiManagerWithId.Value.OnProjectileTriggerEnter(this.SphereCollider);
                }
            }
            MonoBehaviour.Destroy(this.SphereCollider);
        }
    }
}
