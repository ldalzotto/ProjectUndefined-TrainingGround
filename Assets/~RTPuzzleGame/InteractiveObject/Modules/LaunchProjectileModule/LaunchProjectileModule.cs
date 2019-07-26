using CoreGame;
using GameConfigurationID;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class LaunchProjectileModule : InteractiveObjectModule
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
        private InteractiveObjectContainer InteractiveObjectContainer;
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

        public static InteractiveObjectType InstanciateV2(ProjectileInherentData LaunchProjectileInherentData, BeziersControlPoints ProjectilePath, Transform parentTransform)
        {
            var launchProjectileInteractiveObjet = MonoBehaviour.Instantiate(LaunchProjectileInherentData.ProjectilePrefabV2, parentTransform);
            launchProjectileInteractiveObjet.Init(new InteractiveObjectInitializationObject(ProjectileInherentData: LaunchProjectileInherentData, ProjectilePath: ProjectilePath));
            return launchProjectileInteractiveObjet;
        }

        public void Init(ProjectileInherentData LaunchProjectileInherentData, BeziersControlPoints ProjectilePath, Transform baseObjectTransform)
        {
            #region External Dependencies
            var npcAiManagerContainer = GameObject.FindObjectOfType<NPCAIManagerContainer>();
            this.LaunchProjectileEventManager = GameObject.FindObjectOfType<LaunchProjectileEventManager>();
            this.InteractiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();
            var InteractiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();
            var PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            var PuzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            #endregion

            #region Internal Dependencies
            this.LaunchProjectileGroundColliderTracker = this.GetComponentInChildren<LaunchProjectileGroundColliderTracker>();
            this.LaunchProjectileGroundColliderTracker.Init(this);
            #endregion

            this.launchProjectileInherentData = LaunchProjectileInherentData;
            baseObjectTransform.position = ProjectilePath.ResolvePoint(0.1f);
            var projectilePathDeepCopy = ProjectilePath.Clone();

            this.SphereCollisionManager = new SphereCollisionManager(this.launchProjectileInherentData, npcAiManagerContainer, InteractiveObjectContainer, PuzzleGameConfigurationManager, PuzzleEventsManager, this);
            this.LaunchProjectileMovementManager = new LaunchProjectileMovementManager(this.launchProjectileInherentData, baseObjectTransform, projectilePathDeepCopy);

        }

        public void Tick(float d, float timeAttenuationFactor)
        {
            LaunchProjectileMovementManager.Tick(d, timeAttenuationFactor);
        }

        public static LaunchProjectileModule GetFromCollisionType(CollisionType collisionType)
        {
            return collisionType.GetComponent<LaunchProjectileModule>();
        }

        #region External Events
        public void OnGroundTriggerEnter()
        {
            SphereCollisionManager.OnGroundTriggerEnter(this);
            this.InteractiveObjectContainer.OnInteractiveObjectDestroyed(this.GetComponentInParent<InteractiveObjectType>());
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
        private InteractiveObjectContainer InteractiveObjectContainer;
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        private LaunchProjectileModule LaunchProjectileRef;
        private ProjectileInherentData LaunchProjectileInherentData;

        public SphereCollisionManager(ProjectileInherentData LaunchProjectileInherentData,
            NPCAIManagerContainer NPCAIManagerContainer,
            InteractiveObjectContainer InteractiveObjectContainer,
            PuzzleGameConfigurationManager PuzzleGameConfigurationManager,
            PuzzleEventsManager PuzzleEventsManager,
            LaunchProjectileModule LaunchProjectileRef)
        {
            #region External Dependencies
            this.NPCAIManagerContainer = NPCAIManagerContainer;
            this.InteractiveObjectContainer = InteractiveObjectContainer;
            this.PuzzleGameConfigurationManager = PuzzleGameConfigurationManager;
            this.PuzzleEventsManager = PuzzleEventsManager;
            #endregion

            this.LaunchProjectileInherentData = LaunchProjectileInherentData;
            this.LaunchProjectileRef = LaunchProjectileRef;
        }

        public void OnGroundTriggerEnter(LaunchProjectileModule launchProjectileRef)
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
            foreach (var repelAbleObject in this.InteractiveObjectContainer.ObjectsRepelable)
            {
                if (Intersection.BoxIntersectsSphereV2(repelAbleObject.ObjectRepelCollider as BoxCollider, projectileTargetPosition, LaunchProjectileInherentData.EffectRange))
                {
                    //float travelDistance = 13;
                    float travelDistance = this.PuzzleGameConfigurationManager.RepelableObjectsConfiguration()[repelAbleObject.RepelableObjectID].GetRepelableObjectDistance(this.LaunchProjectileRef.LaunchProjectileId);
                    var projectionDirection = Vector3.ProjectOnPlane((repelAbleObject.transform.position - projectileTargetPosition), repelAbleObject.transform.up).normalized;
                    NavMeshHit navmeshHit;
                    if (NavMesh.SamplePosition(repelAbleObject.transform.position + (projectionDirection * travelDistance), out navmeshHit, 0.5f, NavMesh.AllAreas))
                    {
                        this.PuzzleEventsManager.PZ_EVT_RepelableObject_OnObjectRepelled(repelAbleObject, navmeshHit.position);
                    }
                    else
                    {
                        if (NavMesh.Raycast(repelAbleObject.transform.position, repelAbleObject.transform.position + (projectionDirection * travelDistance), out navmeshHit, NavMesh.AllAreas))
                        {
                            this.PuzzleEventsManager.PZ_EVT_RepelableObject_OnObjectRepelled(repelAbleObject, navmeshHit.position);
                        }
                    }
                }
            }
            #endregion
        }
    }
}
