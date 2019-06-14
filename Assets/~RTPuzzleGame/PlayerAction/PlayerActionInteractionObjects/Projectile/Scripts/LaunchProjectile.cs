using CoreGame;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class LaunchProjectile : MonoBehaviour
    {
        public LaunchProjectileId LaunchProjectileId;
        private ProjectileInherentData launchProjectileInherentData;
        public ProjectileInherentData LaunchProjectileInherentData { get => launchProjectileInherentData; }

        #region Internal Dependencies
        private LaunchProjectileGroundColliderTracker LaunchProjectileGroundColliderTracker;
        #endregion

        private LaunchProjectileMovementManager LaunchProjectileMovementManager;
        private SphereCollisionManager SphereCollisionManager;

        #region External Dependencies
        private LaunchProjectileEventManager LaunchProjectileEventManager;
        #endregion

        #region Data Retrieval
        public Collider GetGroundCollisionTrackingCollider()
        {
            return this.LaunchProjectileGroundColliderTracker.SphereCollider;
        }
        public Vector3 GetTargetPosition()
        {
            return this.LaunchProjectileMovementManager.GetTargetPosition();
        }
        #endregion

        public static LaunchProjectile Instantiate(ProjectileInherentData LaunchProjectileInherentData, BeziersControlPoints ProjectilePath, Transform parentTransform)
        {
            var launchProjectile = MonoBehaviour.Instantiate(LaunchProjectileInherentData.ProjectilePrefab, parentTransform);
            launchProjectile.Init(LaunchProjectileInherentData, ProjectilePath);
            return launchProjectile;
        }


        public void Init(ProjectileInherentData LaunchProjectileInherentData, BeziersControlPoints ProjectilePath)
        {
            #region External Dependencies
            var npcAiManagerContainer = GameObject.FindObjectOfType<NPCAIManagerContainer>();
            this.LaunchProjectileEventManager = GameObject.FindObjectOfType<LaunchProjectileEventManager>();
            var ObjectRepelContainer = GameObject.FindObjectOfType<ObjectRepelContainer>();
            var ObjectRepelContainerManager = GameObject.FindObjectOfType<ObjectRepelContainerManager>();
            var PuzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            #endregion

            #region Internal Dependencies
            this.LaunchProjectileGroundColliderTracker = this.GetComponentInChildren<LaunchProjectileGroundColliderTracker>();
            this.LaunchProjectileGroundColliderTracker.Init(this);
            #endregion

            this.launchProjectileInherentData = LaunchProjectileInherentData;
            //    sphereCollider.radius = 1f;// this.launchProjectileInherentData.EffectRange;
            this.transform.position = ProjectilePath.ResolvePoint(0.1f);
            var projectilePathDeepCopy = ProjectilePath.Clone();

            this.SphereCollisionManager = new SphereCollisionManager(this.launchProjectileInherentData, npcAiManagerContainer, ObjectRepelContainer, ObjectRepelContainerManager, PuzzleGameConfigurationManager, this);
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
            SphereCollisionManager.OnGroundTriggerEnter(this);
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

        public Vector3 GetTargetPosition()
        {
            return this.ProjectilePath.ResolvePoint(1f);
        }

    }
    #endregion

    class SphereCollisionManager
    {
        #region External Dependencies
        private NPCAIManagerContainer NPCAIManagerContainer;
        private ObjectRepelContainer ObjectRepelContainer;
        private ObjectRepelContainerManager ObjectRepelContainerManager;
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        #endregion

        private LaunchProjectile LaunchProjectileRef;
        private ProjectileInherentData LaunchProjectileInherentData;

        public SphereCollisionManager(ProjectileInherentData LaunchProjectileInherentData,
            NPCAIManagerContainer NPCAIManagerContainer, ObjectRepelContainer ObjectRepelContainer,
            ObjectRepelContainerManager ObjectRepelContainerManager,
            PuzzleGameConfigurationManager PuzzleGameConfigurationManager, LaunchProjectile LaunchProjectileRef)
        {
            #region External Dependencies
            this.NPCAIManagerContainer = NPCAIManagerContainer;
            this.ObjectRepelContainer = ObjectRepelContainer;
            this.ObjectRepelContainerManager = ObjectRepelContainerManager;
            this.PuzzleGameConfigurationManager = PuzzleGameConfigurationManager;
            #endregion

            this.LaunchProjectileInherentData = LaunchProjectileInherentData;
            this.LaunchProjectileRef = LaunchProjectileRef;
        }

        public void OnGroundTriggerEnter(LaunchProjectile launchProjectileRef)
        {
            var projectileTargetPosition = launchProjectileRef.GetTargetPosition();
            #region AI escape
            foreach (var npcAIManager in this.NPCAIManagerContainer.GetNPCAiManagers().Values)
            {
                if (Intersection.BoxIntersectsSphereV2(npcAIManager.GetCollider() as BoxCollider, projectileTargetPosition, LaunchProjectileInherentData.EffectRange))
                {
                    npcAIManager.OnProjectileTriggerEnter(launchProjectileRef);
                }
            }
            #endregion

            #region Repel objects
            foreach (var repelAbleObject in this.ObjectRepelContainer.ObjectsRepelable)
            {
                if (Intersection.BoxIntersectsSphereV2(repelAbleObject.ObjectRepelCollider as BoxCollider, projectileTargetPosition, LaunchProjectileInherentData.EffectRange))
                {
                    //float travelDistance = 13;
                    float travelDistance = this.PuzzleGameConfigurationManager.RepelableObjectsConfiguration()[repelAbleObject.RepelableObjectID].GetRepelableObjectDistance(this.LaunchProjectileRef.LaunchProjectileId);
                    var projectionDirection = Vector3.ProjectOnPlane((repelAbleObject.transform.position - projectileTargetPosition), repelAbleObject.transform.up).normalized;
                    NavMeshHit navmeshHit;
                    if (NavMesh.SamplePosition(repelAbleObject.transform.position + (projectionDirection * travelDistance), out navmeshHit, 0.5f, NavMesh.AllAreas))
                    {
                        this.ObjectRepelContainerManager.OnObjectRepelRepelled(repelAbleObject, navmeshHit.position);
                    }
                    else
                    {
                        if (NavMesh.Raycast(repelAbleObject.transform.position, repelAbleObject.transform.position + (projectionDirection * travelDistance), out navmeshHit, NavMesh.AllAreas))
                        {
                            this.ObjectRepelContainerManager.OnObjectRepelRepelled(repelAbleObject, navmeshHit.position);
                        }
                    }
                }
            }
            #endregion

            MonoBehaviour.Destroy(launchProjectileRef.gameObject);
        }
    }
}
