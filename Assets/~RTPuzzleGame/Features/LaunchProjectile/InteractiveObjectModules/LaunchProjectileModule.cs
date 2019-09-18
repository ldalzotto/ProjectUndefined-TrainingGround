using CoreGame;
using GameConfigurationID;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace RTPuzzle
{
    public interface ILaunchProjectileModuleDataRetrieval
    {
        LaunchProjectileID GetLaunchProjectileID();
        LaunchProjectileInherentData GetLaunchProjectileInherentData();
        Collider GetGroundCollisionTrackingCollider();
    }

    public class LaunchProjectileModule : InteractiveObjectModule, ILaunchProjectileModuleDataRetrieval
    {
        [FormerlySerializedAs("LaunchProjectileId")]
        public LaunchProjectileID LaunchProjectileID;
        private LaunchProjectileInherentData launchProjectileInherentData;

        #region Internal Dependencies
        private InteractiveObjectType ParentInteractiveObjectTypeRef;
        private SphereCollider ProjectileGroundTrigger;
        #endregion

        private LaunchProjectileMovementManager LaunchProjectileMovementManager;
        private SphereCollisionManager SphereCollisionManager;

        #region External Dependencies
        private InteractiveObjectContainer InteractiveObjectContainer;
        #endregion

        #region Data Retrieval
        public Collider GetGroundCollisionTrackingCollider()
        {
            return this.ProjectileGroundTrigger;
        }
        public LaunchProjectileInherentData GetLaunchProjectileInherentData()
        {
            return this.launchProjectileInherentData;
        }
        public LaunchProjectileID GetLaunchProjectileID()
        {
            return this.LaunchProjectileID;
        }
        public Vector3 GetTargetPosition()
        {
            return this.LaunchProjectileMovementManager.GetTargetPosition();
        }
        #endregion

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval,
            IInteractiveObjectTypeEvents IInteractiveObjectTypeEvents)
        {
            LaunchProjectileInherentData LaunchProjectileInherentData = interactiveObjectInitializationObject.LaunchProjectileInherentData;
            if (LaunchProjectileInherentData == null)
            {
                LaunchProjectileInherentData = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager.ProjectileConf()[this.LaunchProjectileID];
            }

            #region External Dependencies
            var npcAiManagerContainer = PuzzleGameSingletonInstances.AIManagerContainer;
            this.InteractiveObjectContainer = PuzzleGameSingletonInstances.InteractiveObjectContainer;
            var InteractiveObjectContainer = PuzzleGameSingletonInstances.InteractiveObjectContainer;
            var PuzzleEventsManager = PuzzleGameSingletonInstances.PuzzleEventsManager;
            var PuzzleGameConfigurationManager = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager;
            #endregion

            #region Internal Dependencies
            this.ParentInteractiveObjectTypeRef = this.GetComponentInParent<InteractiveObjectType>();
            #endregion

            this.ProjectileGroundTrigger = GetComponent<SphereCollider>();

            this.launchProjectileInherentData = LaunchProjectileInherentData;
            IInteractiveObjectTypeDataRetrieval.GetTransform().position = interactiveObjectInitializationObject.ProjectilePath.ResolvePoint(0.1f);
            var projectilePathDeepCopy = interactiveObjectInitializationObject.ProjectilePath.Clone();

            this.SphereCollisionManager = new SphereCollisionManager(this.launchProjectileInherentData, npcAiManagerContainer, InteractiveObjectContainer, PuzzleGameConfigurationManager, PuzzleEventsManager, this);
            this.LaunchProjectileMovementManager = new LaunchProjectileMovementManager(this.launchProjectileInherentData, IInteractiveObjectTypeDataRetrieval.GetTransform(), projectilePathDeepCopy);
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
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer(LayerConstants.PUZZLE_GROUND_LAYER))
            {
                //We move the projectile to its final position
                this.ParentInteractiveObjectTypeRef.transform.position = this.GetTargetPosition();
                if (this.launchProjectileInherentData.isExploding)
                {
                    SphereCollisionManager.OnProjectileExplode(this);
                    this.InteractiveObjectContainer.OnInteractiveObjectDestroyed(this.ParentInteractiveObjectTypeRef);
                }
                else
                {
                    this.ParentInteractiveObjectTypeRef.EnableAllDisabledModules(new InteractiveObjectInitializationObject());
                    this.ParentInteractiveObjectTypeRef.DisableModule(this.GetType());
                }
            }
        }
        #endregion

        public static class LaunchProjectileModuleInstancer
        {
            internal static void PopuplateFromDefinition(LaunchProjectileModule launchProjectileModule, LaunchProjectileModuleDefinition launchProjectileModuleDefinition)
            {
                launchProjectileModule.LaunchProjectileID = launchProjectileModuleDefinition.LaunchProjectileID;
            }
        }
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
        private AIManagerContainer NPCAIManagerContainer;
        private InteractiveObjectContainer InteractiveObjectContainer;
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        private LaunchProjectileModule LaunchProjectileRef;
        private LaunchProjectileInherentData LaunchProjectileInherentData;

        public SphereCollisionManager(LaunchProjectileInherentData LaunchProjectileInherentData,
            AIManagerContainer NPCAIManagerContainer,
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

        public void OnProjectileExplode(LaunchProjectileModule launchProjectileRef)
        {
            #region AI escape
            foreach (var npcAIManager in this.NPCAIManagerContainer.GetNPCAiManagers().Values)
            {
                if (Intersection.BoxIntersectsOrEntirelyContainedInSphere(npcAIManager.GetLogicCollider() as BoxCollider, launchProjectileRef.transform.position, LaunchProjectileInherentData.ExplodingEffectRange))
                {
                    npcAIManager.GetAIBehavior().ReceiveEvent(new ProjectileTriggerEnterAIBehaviorEvent(launchProjectileRef));
                }
            }
            #endregion

            #region Repel objects
            foreach (var repelAbleObject in this.InteractiveObjectContainer.ObjectsRepelable)
            {
                if (Intersection.BoxIntersectsOrEntirelyContainedInSphere(repelAbleObject.GetObjectRepelCollider() as BoxCollider, launchProjectileRef.transform.position, LaunchProjectileInherentData.ExplodingEffectRange))
                {
                    //float travelDistance = 13;
                    float travelDistance = this.PuzzleGameConfigurationManager.RepelableObjectsConfiguration()[repelAbleObject.GetObjectRepelID()].GetRepelableObjectDistance(this.LaunchProjectileRef.LaunchProjectileID);
                    var projectionDirection = Vector3.ProjectOnPlane((repelAbleObject.GetTransform().position - launchProjectileRef.transform.position), repelAbleObject.GetTransform().up).normalized;
                    NavMeshHit navmeshHit;
                    if (NavMesh.SamplePosition(repelAbleObject.GetTransform().position + (projectionDirection * travelDistance), out navmeshHit, 0.5f, NavMesh.AllAreas))
                    {
                        repelAbleObject.GetIObjectRepelModuleEvent().OnObjectRepelRepelled(navmeshHit.position);
                    }
                    else
                    {
                        if (NavMesh.Raycast(repelAbleObject.GetTransform().position, repelAbleObject.GetTransform().position + (projectionDirection * travelDistance), out navmeshHit, NavMesh.AllAreas))
                        {
                            repelAbleObject.GetIObjectRepelModuleEvent().OnObjectRepelRepelled(navmeshHit.position);
                        }
                    }
                }
            }
            #endregion
        }
    }
}
