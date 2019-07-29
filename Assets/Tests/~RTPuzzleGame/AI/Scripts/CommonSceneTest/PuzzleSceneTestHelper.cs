using CoreGame;
using GameConfigurationID;
using RTPuzzle;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Tests
{
    class PuzzleSceneTestHelper
    {
        public static InteractiveObjectType SpawnProjectile(ProjectileInherentData projectileInherentData, TestPositionID projectilePoistion)
        {
            var projectilePosition = GameObject.FindObjectsOfType<TestPosition>().ToList().Select(a => a).Where(pos => pos.aITestPositionID == projectilePoistion).First().transform.position;
            return SpawnProjectile(projectileInherentData, projectilePosition);
        }

        public static InteractiveObjectType SpawnProjectile(ProjectileInherentData projectileInherentData, Vector3 projectilePosition)
        {
            var PuzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var createdProjectile = ProjectileActionInstanciationHelper.CreateProjectileAtStart(projectileInherentData, GameObject.FindObjectOfType<InteractiveObjectContainer>());

            var projectileBezierPath = new BeziersControlPoints();
            projectileBezierPath.P0 = projectilePosition;
            projectileBezierPath.P1 = projectilePosition;
            projectileBezierPath.P2 = projectilePosition;
            projectileBezierPath.P3 = projectilePosition;

            ProjectileActionInstanciationHelper.OnProjectileSpawn(ref createdProjectile, projectileBezierPath, projectileInherentData);
            return createdProjectile;
        }

        public static Transform FindTestPosition(TestPositionID aITestPositionID)
        {
            return GameObject.FindObjectsOfType<TestPosition>().ToList().Select(a => a).Where(pos => pos.aITestPositionID == aITestPositionID).First().transform;
        }

        public static InteractiveObjectType SpawnAttractiveObject(AttractiveObjectInherentConfigurationData attractiveObjectInherentConfigurationData, TestPositionID aITestPositionID)
        {
            var attractiveObjectSpawnPosition = FindTestPosition(aITestPositionID).position;
            return SpawnAttractiveObject(attractiveObjectInherentConfigurationData, attractiveObjectSpawnPosition);
        }

        public static InteractiveObjectType SpawnAttractiveObject(AttractiveObjectInherentConfigurationData attractiveObjectInherentConfigurationData, Vector3 worldPosition)
        {
            return AttractiveObjectTypeModule.Instanciate(worldPosition, null, attractiveObjectInherentConfigurationData, null);
        }

        public static InteractiveObjectType SpawnAIDisarmObject(DisarmObjectInherentData disarmObjectInherentData, Vector3 worldPosition)
        {
            return DisarmObjectModule.Instanciate(worldPosition, disarmObjectInherentData);
        }

        public static void SetAttractiveObjectConfigurationData(AttractiveObjectId attractiveObjectId, float effectRange, float effectiveTime)
        {
            var attractiveObjectConfiguration = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().AttractiveObjectsConfiguration()[attractiveObjectId];
            attractiveObjectConfiguration.EffectRange = effectRange;
            attractiveObjectConfiguration.EffectiveTime = effectiveTime;
        }

        public static TargetZoneObjectModule FindTargetZone(TargetZoneID targetZoneID)
        {
            return GameObject.FindObjectsOfType<TargetZoneObjectModule>().ToArray().Select(t => t).Where(targetZone => targetZone.TargetZoneID == targetZoneID).First();
        }

        public static AttractiveObjectInherentConfigurationData CreateAttractiveObjectInherentConfigurationData(float effectRange, float effectiveTime)
        {
            var attractiveObjectInherentConfigurationData = ScriptableObject.CreateInstance<AttractiveObjectInherentConfigurationData>();
            var randomAttractiveObjectInherentData = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().AttractiveObjectsConfiguration()[AttractiveObjectId.CHEESE];
            attractiveObjectInherentConfigurationData.Init(effectRange, effectiveTime, randomAttractiveObjectInherentData.AttractiveInteractiveObjectPrefab);
            return attractiveObjectInherentConfigurationData;
        }

        public static void SetPlayerEscapeComponentValues(GenericPuzzleAIComponents GenericPuzzleAIComponents, float escapeDistance, float escapeSemiAngle, float playerDetectionRadius)
        {
            GenericPuzzleAIComponents.AIPlayerEscapeComponent.EscapeDistance = escapeDistance;
            GenericPuzzleAIComponents.AIPlayerEscapeComponent.EscapeSemiAngle = escapeSemiAngle;
            GenericPuzzleAIComponents.AIPlayerEscapeComponent.PlayerDetectionRadius = playerDetectionRadius;
        }

        public static ProjectileInherentData CreateProjectileInherentData(float effectRange, float travelDistancePerSeconds, LaunchProjectileId baseProjectileIDForPrefab, bool isExploding = true, bool isPersistingToAttractiveObject = false)
        {
            var projectileData = ScriptableObject.CreateInstance<ProjectileInherentData>();
            var randomProjectileInherentConfiguration = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().ProjectileConf()[baseProjectileIDForPrefab];
            projectileData.Init(effectRange, 0f, travelDistancePerSeconds, randomProjectileInherentConfiguration.ProjectilePrefabV2, isExploding, isPersistingToAttractiveObject);
            return projectileData;
        }

        public static DisarmObjectInherentData CreateDisarmObjectInherentData(float DisarmTime, float DisarmInteractionRange, DisarmObjectID disarmObjectID)
        {
            var disarmObjectData = ScriptableObject.CreateInstance<DisarmObjectInherentData>();
            var foundDisarmObjectConfiguration = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().DisarmObjectsConfiguration()[disarmObjectID];
            disarmObjectData.Init(DisarmTime, DisarmInteractionRange, foundDisarmObjectConfiguration.DisarmObjectPrefab);
            return disarmObjectData;
        }

        public static void SetAIEscapeSemiAngle(AbstractAIComponents abstractAIComponents, float escapeSemiAngle)
        {
            if (abstractAIComponents.GetType() == typeof(GenericPuzzleAIComponents))
            {
                GenericPuzzleAIComponents genericPuzzleAIComponents = (GenericPuzzleAIComponents)abstractAIComponents;
                genericPuzzleAIComponents.AIProjectileEscapeWithCollisionComponent.EscapeSemiAngleV2.Values[LaunchProjectileId.TEST_PROJECTILE_EXPLODE] = escapeSemiAngle;
            }
        }

        public static void SetAIEscapeDistanceFromProjectile(AbstractAIComponents abstractAIComponents, float escapeDistance)
        {
            if (abstractAIComponents.GetType() == typeof(GenericPuzzleAIComponents))
            {
                GenericPuzzleAIComponents genericPuzzleAIComponents = (GenericPuzzleAIComponents)abstractAIComponents;
                genericPuzzleAIComponents.AIProjectileEscapeWithCollisionComponent.EscapeDistanceV2.Values[LaunchProjectileId.TEST_PROJECTILE_EXPLODE] = escapeDistance;
            }
        }

        #region Move Toward Player
        public static IEnumerator MoveTowardPlayerYield(TestPositionID playerPosition, Func<IEnumerator> OnPlayerInSight)
        {
            yield return MovePlayerAndWaitForFixed(playerPosition,
                    OnPlayerMoved: () =>
                    {
                        if (OnPlayerInSight != null)
                        {
                            return OnPlayerInSight.Invoke();
                        }
                        return null;
                    }
                );
        }

        public static IEnumerator MovePlayerAndWaitForFixed(TestPositionID playerPosition, Func<IEnumerator> OnPlayerMoved)
        {
            yield return MovePlayerAndWaitForFixed(FindTestPosition(playerPosition).position, OnPlayerMoved);
        }

        public static IEnumerator MovePlayerAndWaitForFixed(Vector3 playerPosition, Func<IEnumerator> OnPlayerMoved)
        {
            var playerManagerDataRetriever = GameObject.FindObjectOfType<PlayerManagerDataRetriever>();
            playerManagerDataRetriever.GetPlayerRigidBody().position = playerPosition;
            if (OnPlayerMoved != null)
            {
                yield return new WaitForFixedUpdate();
                yield return new WaitForEndOfFrame();
                yield return OnPlayerMoved.Invoke();
            }
        }
        #endregion

        #region Attractive Object
        public static IEnumerator AttractiveObjectYield(AttractiveObjectInherentConfigurationData attractiveObjectInherentConfigurationData, Vector3 worldPosition,
                    Func<InteractiveObjectType, IEnumerator> OnAttractiveObjectSpawn, Func<IEnumerator> OnAttractiveObjectDestroyed)
        {
            var attractiveObjectType = PuzzleSceneTestHelper.SpawnAttractiveObject(attractiveObjectInherentConfigurationData, worldPosition);
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
                Func<InteractiveObjectType, IEnumerator> OnProjectileSpawn, Func<IEnumerator> OnDistanceReached)
        {
            var projectile = SpawnProjectile(projectileInherentData, projectilePoistion);
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

        public static IEnumerator ProjectileToAttractiveYield(ProjectileInherentData projectileInherentData, float attractiveObjectEffectRange, float attractiveObjectEffectiveTime, Vector3 projectilePosition,
               Func<InteractiveObjectType, IEnumerator> OnProjectileSpawn, Func<InteractiveObjectType, IEnumerator> OnProjectileTurnedIntoAttractive, Func<IEnumerator> OnDistanceReached)
        {
            var projectile = SpawnProjectile(projectileInherentData, projectilePosition);

            PuzzleSceneTestHelper.SetAttractiveObjectConfigurationData(projectile.GetDisabledModule<AttractiveObjectTypeModule>().AttractiveObjectId, attractiveObjectEffectRange, attractiveObjectEffectiveTime);

            yield return new WaitForFixedUpdate();
            if (OnProjectileSpawn != null)
            {
                yield return OnProjectileSpawn.Invoke(projectile);
            }
            if (OnProjectileTurnedIntoAttractive != null)
            {
                //Wait for attractive object to be taken into account
                yield return new WaitForFixedUpdate();
                yield return new WaitForEndOfFrame();
                yield return OnProjectileTurnedIntoAttractive.Invoke(projectile);
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

        public static IEnumerator ProjectileToAttractiveYield(ProjectileInherentData projectileInherentData, float attractiveObjectEffectRange, float attractiveObjectEffectiveTime, TestPositionID projectilePosition,
            Func<InteractiveObjectType, IEnumerator> OnProjectileSpawn, Func<InteractiveObjectType, IEnumerator> OnProjectileTurnedIntoAttractive, Func<IEnumerator> OnDistanceReached)
        {
            yield return ProjectileToAttractiveYield(projectileInherentData, attractiveObjectEffectRange, attractiveObjectEffectiveTime, FindTestPosition(projectilePosition).position,
                 OnProjectileSpawn, OnProjectileTurnedIntoAttractive, OnDistanceReached);
        }
        #endregion

        #region TargetZone
        public static IEnumerator TargetZoneYield(TargetZoneInherentData targetZoneInherentData, Vector3 targetZonePosition,
            Func<InteractiveObjectType, IEnumerator> OnTargetZoneSpawn, Func<IEnumerator> OnDistanceReached)
        {
            var targetZone = TargetZoneObjectModule.Instanciate(targetZoneInherentData, targetZonePosition);
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
            Func<InteractiveObjectType, IEnumerator> OnSecondProjectileSpawned,
            Func<IEnumerator> OnSecondProjectileDistanceReached)
        {
            SpawnProjectile(projectileInherentData, projectilePoistion);
            yield return new WaitForFixedUpdate();
            if (OnBeforeSecondProjectileSpawn != null)
            {
                yield return OnBeforeSecondProjectileSpawn.Invoke();
            }
            var projectile = SpawnProjectile(projectileInherentData, projectilePoistion);
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
            Func<IEnumerator> OnFearTriggered, Func<IEnumerator> OnFearEnded, float fearTime, GenericPuzzleAIBehavior mouseAIBheavior)
        {
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseAIBheavior.AIComponents, 1f);
            yield return ProjectileYield(PuzzleSceneTestHelper.CreateProjectileInherentData(1000, 1, LaunchProjectileId.TEST_PROJECTILE_EXPLODE), projectilePosition,
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
                OnProjectileSpawn: (InteractiveObjectType launchProjectile) =>
                {
                    return PuzzleSceneTestHelper.EscapeFromPlayerYield(playerManager, nPCAIManager, OnBeforeSettingPosition, OnSamePositionSetted, null);
                },
                OnDistanceReached: OnDestinationReached);
        }
        #endregion

        #region AI disarm object
        public static IEnumerator DisarmObjectYield(DisarmObjectInherentData disarmObjectInherentData, Vector3 worldPosition, Func<InteractiveObjectType, IEnumerator> OnDisarmObjectSpawn, 
                    Func<InteractiveObjectType, IEnumerator> OnDisarmTimerOver)
        {
            var disarmObject = SpawnAIDisarmObject(disarmObjectInherentData, worldPosition);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
            if (OnDisarmObjectSpawn != null)
            {
                yield return OnDisarmObjectSpawn.Invoke(disarmObject);
            }
            if (OnDisarmTimerOver != null)
            {
                yield return new WaitForSeconds(disarmObjectInherentData.DisarmTime);
                yield return OnDisarmTimerOver.Invoke(disarmObject);
            }
        }
        #endregion

        public static void InitializeAIComponents(AbstractAIComponents abstractAIComponents)
        {
            if (abstractAIComponents.GetType() == typeof(GenericPuzzleAIComponents))
            {
                var genericPuzzleAIComponents = (GenericPuzzleAIComponents)abstractAIComponents;

                genericPuzzleAIComponents.AIRandomPatrolComponent.MaxDistance = 15f;

                if (genericPuzzleAIComponents.AIProjectileEscapeWithCollisionComponent.EscapeDistanceV2 != null)
                {
                    PuzzleSceneTestHelper.SetAIEscapeDistanceFromProjectile(genericPuzzleAIComponents, 25f);
                }

                genericPuzzleAIComponents.AITargetZoneComponent.TargetZoneEscapeDistance = 50f;

                genericPuzzleAIComponents.AIFearStunComponent.FOVSumThreshold = 20f;
                genericPuzzleAIComponents.AIFearStunComponent.TimeWhileBeginFeared = 2f;

                genericPuzzleAIComponents.AIPlayerEscapeComponent.PlayerDetectionRadius = -1f;
            }
        }
    }
}
