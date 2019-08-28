using CoreGame;
using GameConfigurationID;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace RTPuzzle
{
    public class LaunchProjectileModule : InteractiveObjectModule
    {
        [FormerlySerializedAs("LaunchProjectileId")]
        public LaunchProjectileID LaunchProjectileID;
        private LaunchProjectileInherentData launchProjectileInherentData;
        public LaunchProjectileInherentData LaunchProjectileInherentData { get => launchProjectileInherentData; }

        #region Internal Dependencies
        private LaunchProjectileGroundColliderTracker LaunchProjectileGroundColliderTracker;
        private InteractiveObjectType ParentInteractiveObjectTypeRef;
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
        
        public void Init(LaunchProjectileInherentData LaunchProjectileInherentData, BeziersControlPoints ProjectilePath, Transform baseObjectTransform)
        {
            #region External Dependencies
            var npcAiManagerContainer = GameObject.FindObjectOfType<AIManagerContainer>();
            this.LaunchProjectileEventManager = GameObject.FindObjectOfType<LaunchProjectileEventManager>();
            this.InteractiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();
            var InteractiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();
            var PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            var PuzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            #endregion

            #region Internal Dependencies
            this.ParentInteractiveObjectTypeRef = this.GetComponentInParent<InteractiveObjectType>();
            this.LaunchProjectileGroundColliderTracker = this.GetComponentInChildren<LaunchProjectileGroundColliderTracker>();
            #endregion

            this.LaunchProjectileGroundColliderTracker.Init(this);

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
                if (Intersection.BoxIntersectsOrEntirelyContainedInSphere(npcAIManager.GetCollider() as BoxCollider, launchProjectileRef.transform.position, LaunchProjectileInherentData.ExplodingEffectRange))
                {
                    npcAIManager.OnProjectileTriggerEnter(launchProjectileRef);
                }
            }
            #endregion

            #region Repel objects
            foreach (var repelAbleObject in this.InteractiveObjectContainer.ObjectsRepelable)
            {
                if (Intersection.BoxIntersectsOrEntirelyContainedInSphere(repelAbleObject.ObjectRepelCollider as BoxCollider, launchProjectileRef.transform.position, LaunchProjectileInherentData.ExplodingEffectRange))
                {
                    //float travelDistance = 13;
                    float travelDistance = this.PuzzleGameConfigurationManager.RepelableObjectsConfiguration()[repelAbleObject.ObjectRepelID].GetRepelableObjectDistance(this.LaunchProjectileRef.LaunchProjectileID);
                    var projectionDirection = Vector3.ProjectOnPlane((repelAbleObject.transform.position - launchProjectileRef.transform.position), repelAbleObject.transform.up).normalized;
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
