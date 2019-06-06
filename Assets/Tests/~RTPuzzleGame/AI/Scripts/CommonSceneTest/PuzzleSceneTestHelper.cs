using RTPuzzle;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Tests
{
    class PuzzleSceneTestHelper
    {
        public static LaunchProjectile SpawnProjectile(ProjectileInherentData projectileInherentData, AITestPositionID projectilePoistion, LaunchProjectileContainerManager launchProjectileContainerManager)
        {
            var projectilePosition = GameObject.FindObjectsOfType<AITestPosition>().ToList().Select(a => a).Where(pos => pos.aITestPositionID == projectilePoistion).First().transform.position;
            return SpawnProjectile(projectileInherentData, projectilePosition, launchProjectileContainerManager);
        }

        public static LaunchProjectile SpawnProjectile(ProjectileInherentData projectileInherentData, Vector3 projectilePoistion, LaunchProjectileContainerManager launchProjectileContainerManager)
        {
            var projectileBezierPath = new BeziersControlPoints();
            projectileBezierPath.P0 = projectilePoistion;
            projectileBezierPath.P1 = projectilePoistion;
            projectileBezierPath.P2 = projectilePoistion;
            projectileBezierPath.P3 = projectilePoistion;
            var launchProjectile = LaunchProjectile.Instantiate(projectileInherentData, projectileBezierPath, launchProjectileContainerManager.transform);
            launchProjectile.transform.position = projectilePoistion;
            return launchProjectile;
        }

        public static Transform FindAITestPosition(AITestPositionID aITestPositionID)
        {
            return GameObject.FindObjectsOfType<AITestPosition>().ToList().Select(a => a).Where(pos => pos.aITestPositionID == aITestPositionID).First().transform;
        }

        public static AttractiveObjectType SpawnAttractiveObject(AttractiveObjectInherentConfigurationData attractiveObjectInherentConfigurationData, AITestPositionID aITestPositionID)
        {
            var attractiveObjectSpawnPosition = FindAITestPosition(aITestPositionID).position;
            return SpawnAttractiveObject(attractiveObjectInherentConfigurationData, attractiveObjectSpawnPosition);
        }

        public static AttractiveObjectType SpawnAttractiveObject(AttractiveObjectInherentConfigurationData attractiveObjectInherentConfigurationData, Vector3 worldPosition)
        {
            return AttractiveObjectType.Instanciate(worldPosition, null, attractiveObjectInherentConfigurationData);
        }

        public static TargetZone FindTargetZone(TargetZoneID targetZoneID)
        {
            return GameObject.FindObjectsOfType<TargetZone>().ToArray().Select(t => t).Where(targetZone => targetZone.TargetZoneID == targetZoneID).First();
        }

        public static AttractiveObjectInherentConfigurationData CreateAttractiveObjectInherentConfigurationData(float effectRange, float effectiveTime)
        {
            var attractiveObjectInherentConfigurationData = ScriptableObject.CreateInstance<AttractiveObjectInherentConfigurationData>();
            var randomAttractiveObjectInherentData = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().AttractiveObjectsConfiguration()[AttractiveObjectId.CHEESE];
            attractiveObjectInherentConfigurationData.Init(effectRange, effectiveTime, randomAttractiveObjectInherentData.AttractiveObjectModelPrefab, randomAttractiveObjectInherentData.AttractiveObjectPrefab);
            return attractiveObjectInherentConfigurationData;
        }

        public static void SetPlayerEscapeComponentValues(GenericPuzzleAIComponents GenericPuzzleAIComponents, float escapeDistance, float escapeSemiAngle, float playerDetectionRadius)
        {
            GenericPuzzleAIComponents.AIPlayerEscapeComponent.EscapeDistance = escapeDistance;
            GenericPuzzleAIComponents.AIPlayerEscapeComponent.EscapeSemiAngle = escapeSemiAngle;
            GenericPuzzleAIComponents.AIPlayerEscapeComponent.PlayerDetectionRadius = playerDetectionRadius;
        }

        public static ProjectileInherentData CreateProjectileInherentData(float effectRange, float escapeSemiAngle, float travelDistancePerSeconds)
        {
            var projectileData = ScriptableObject.CreateInstance<ProjectileInherentData>();
            var randomProjectileInherentConfiguration = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().ProjectileConf()[LaunchProjectileId.STONE];
            projectileData.Init(effectRange, 0f, escapeSemiAngle, travelDistancePerSeconds, randomProjectileInherentConfiguration.ProjectilePrefab);
            return projectileData;
        }

        #region Attractive Object
        public static IEnumerator AttractiveObjectYield(AttractiveObjectInherentConfigurationData attractiveObjectInherentConfigurationData, Vector3 worldPosition,
                    Func<AttractiveObjectType, IEnumerator> OnAttractiveObjectSpawn, Func<IEnumerator> OnAttractiveObjectDestroyed)
        {
            var attractiveObjectType = PuzzleSceneTestHelper.SpawnAttractiveObject(attractiveObjectInherentConfigurationData,worldPosition);
            yield return new WaitForFixedUpdate();
            if (OnAttractiveObjectSpawn != null)
            {
                yield return OnAttractiveObjectSpawn.Invoke(attractiveObjectType);
            }
            if (OnAttractiveObjectDestroyed != null)
            {
                yield return new WaitForSeconds(attractiveObjectInherentConfigurationData.EffectiveTime);
                yield return OnAttractiveObjectDestroyed.Invoke();
            }
        }
        #endregion

        #region Projectile
        public static IEnumerator ProjectileYield(ProjectileInherentData projectileInherentData, Vector3 projectilePoistion,
                Func<LaunchProjectile, IEnumerator> OnProjectileSpawn, Func<IEnumerator> OnDistanceReached)
        {
            var projectile = SpawnProjectile(projectileInherentData, projectilePoistion, GameObject.FindObjectOfType<LaunchProjectileContainerManager>());
            yield return new WaitForFixedUpdate();
            if (OnProjectileSpawn != null)
            {
                yield return OnProjectileSpawn.Invoke(projectile);
            }
            if (OnDistanceReached != null)
            {
                var agent = GameObject.FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST).GetAgent();
                TestHelperMethods.SetAgentDestinationPositionReached(agent);
                yield return null;
                yield return new WaitForFixedUpdate();
                yield return OnDistanceReached.Invoke();
            }
        }
        #endregion

        #region TargetZone
        public static IEnumerator TargetZoneYield(TargetZoneInherentData targetZoneInherentData, Vector3 targetZonePosition,
            Func<TargetZone, IEnumerator> OnTargetZoneSpawn, Func<IEnumerator> OnDistanceReached)
        {
            var targetZone = TargetZone.Instanciate(targetZoneInherentData, targetZonePosition);
            yield return new WaitForFixedUpdate();
            if (OnTargetZoneSpawn != null)
            {
                yield return OnTargetZoneSpawn.Invoke(targetZone);
            }
            if (OnDistanceReached != null)
            {
                var agent = GameObject.FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST).GetAgent();
                TestHelperMethods.SetAgentDestinationPositionReached(agent);
                yield return null;
                yield return new WaitForFixedUpdate();
                yield return OnDistanceReached.Invoke();
            }
        }
        #endregion

        #region Projectile Ignore Target
        public static IEnumerator ProjectileIngoreTargetYield(ProjectileInherentData projectileInherentData, Vector3 projectilePoistion,
            Func<IEnumerator> OnBeforeSecondProjectileSpawn,
            Func<LaunchProjectile, IEnumerator> OnSecondProjectileSpawned,
            Func<IEnumerator> OnSecondProjectileDistanceReached)
        {
            SpawnProjectile(projectileInherentData, projectilePoistion, GameObject.FindObjectOfType<LaunchProjectileContainerManager>());
            yield return new WaitForFixedUpdate();
            if (OnBeforeSecondProjectileSpawn != null)
            {
                yield return OnBeforeSecondProjectileSpawn.Invoke();
            }
            var projectile = SpawnProjectile(projectileInherentData, projectilePoistion, GameObject.FindObjectOfType<LaunchProjectileContainerManager>());
            yield return new WaitForFixedUpdate();
            if (OnSecondProjectileSpawned != null)
            {
                yield return OnSecondProjectileSpawned.Invoke(projectile);
            }
            if (OnSecondProjectileDistanceReached != null)
            {
                var agent = GameObject.FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST).GetAgent();
                TestHelperMethods.SetAgentDestinationPositionReached(agent);
                yield return null;
                yield return new WaitForFixedUpdate();
                yield return OnSecondProjectileDistanceReached.Invoke();
            }
        }
        #endregion

        #region Fear
        public static IEnumerator FearYield(Vector3 projectilePosition,
            Func<IEnumerator> OnFearTriggered, Func<IEnumerator> OnFearEnded, float fearTime)
        {
            yield return ProjectileYield(PuzzleSceneTestHelper.CreateProjectileInherentData(1000, 1f, 1), projectilePosition,
                OnProjectileSpawn: null,
                OnDistanceReached: null);
            yield return new WaitForEndOfFrame();
            if (OnFearTriggered != null)
            {
                yield return OnFearTriggered.Invoke();
            }
            if (OnFearEnded != null)
            {
                yield return WaitForFearTimer(fearTime, OnFearEnded);
            }
        }

        public static IEnumerator WaitForFearTimer(float fearTime, Func<IEnumerator> OnFearEnded)
        {
            yield return new WaitForSeconds(fearTime);
            yield return new WaitForFixedUpdate();
            yield return OnFearEnded.Invoke();
        }
        #endregion

        #region Player escape
        public static IEnumerator EscapeFromPlayerYield(PlayerManager playerManager, NPCAIManager nPCAIManager, Func<IEnumerator> OnBeforeSettingPosition, Func<IEnumerator> OnSamePositionSetted, Func<IEnumerator> OnDestinationReached)
        {
            if (OnBeforeSettingPosition != null)
            {
                yield return OnBeforeSettingPosition.Invoke();
            }
            nPCAIManager.GetAgent().Warp(playerManager.transform.position);
            yield return null;
            if (OnSamePositionSetted != null)
            {
                yield return OnSamePositionSetted.Invoke();
            }
            if (OnDestinationReached != null)
            {
                var agent = GameObject.FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST).GetAgent();
                TestHelperMethods.SetAgentDestinationPositionReached(agent);
                yield return null;
                yield return new WaitForFixedUpdate();
                yield return OnDestinationReached.Invoke();
            }
        }

        public static IEnumerator EscapeFromPlayerIgnoreTargetYield(PlayerManager playerManager, NPCAIManager nPCAIManager, ProjectileInherentData projectileInherentData, Vector3 projectilePosition,
            Func<IEnumerator> OnBeforeSettingPosition, Func<IEnumerator> OnSamePositionSetted, Func<IEnumerator> OnDestinationReached)
        {
            yield return PuzzleSceneTestHelper.ProjectileYield(projectileInherentData, projectilePosition,
                OnProjectileSpawn: (LaunchProjectile launchProjectile) =>
                {
                    return PuzzleSceneTestHelper.EscapeFromPlayerYield(playerManager, nPCAIManager, OnBeforeSettingPosition, OnSamePositionSetted, null);
                },
                OnDistanceReached: OnDestinationReached);
        }
        #endregion

        public static void InitializeAIComponents(AbstractAIComponents abstractAIComponents)
        {
            if (abstractAIComponents.GetType() == typeof(GenericPuzzleAIComponents))
            {
                var genericPuzzleAIComponents = (GenericPuzzleAIComponents)abstractAIComponents;

                genericPuzzleAIComponents.AIRandomPatrolComponent.MaxDistance = 15f;

                genericPuzzleAIComponents.AIProjectileEscapeWithCollisionComponent.EscapeDistance = 25f;

                genericPuzzleAIComponents.AITargetZoneComponent.TargetZoneEscapeDistance = 50f;

                genericPuzzleAIComponents.AIFearStunComponent.FOVSumThreshold = 20f;
                genericPuzzleAIComponents.AIFearStunComponent.TimeWhileBeginFeared = 2f;

                genericPuzzleAIComponents.AIPlayerEscapeComponent.PlayerDetectionRadius = -1f;
            }
        }
    }
}
