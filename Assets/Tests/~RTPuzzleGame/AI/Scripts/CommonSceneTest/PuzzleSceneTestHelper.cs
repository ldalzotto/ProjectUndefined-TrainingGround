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
        public static InteractiveObjectType SpawnProjectile(InteractiveObjectInitialization InteractiveObjectInitialization, TestPositionID projectilePoistion)
        {
            var projectilePosition = GameObject.FindObjectsOfType<TestPosition>().ToList().Select(a => a).Where(pos => pos.aITestPositionID == projectilePoistion).First().transform.position;
            return SpawnProjectile(InteractiveObjectInitialization, projectilePosition);
        }

        public static InteractiveObjectType SpawnProjectile(InteractiveObjectInitialization InteractiveObjectInitialization, Vector3 projectilePosition)
        {
            var projectileBezierPath = new BeziersControlPoints();
            projectileBezierPath.P0 = projectilePosition;
            projectileBezierPath.P1 = projectilePosition;
            projectileBezierPath.P2 = projectilePosition;
            projectileBezierPath.P3 = projectilePosition;

            InteractiveObjectInitialization.InteractiveObjectInitializationObject.ProjectilePath = projectileBezierPath;

            var PuzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            return InteractiveObjectType.Instantiate(InteractiveObjectInitialization.InteractiveObjectTypeDefinitionInherentData, InteractiveObjectInitialization.InteractiveObjectInitializationObject,
                     GameObject.FindObjectOfType<PuzzleStaticConfigurationContainer>().PuzzleStaticConfiguration.PuzzlePrefabConfiguration, GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().PuzzleGameConfiguration);
        }

        public static Transform FindTestPosition(TestPositionID aITestPositionID)
        {
            return GameObject.FindObjectsOfType<TestPosition>().ToList().Select(a => a).Where(pos => pos.aITestPositionID == aITestPositionID).First().transform;
        }

        public static InteractiveObjectType SpawnAttractiveObject(InteractiveObjectInitialization InteractiveObjectInitialization, TestPositionID aITestPositionID)
        {
            var attractiveObjectSpawnPosition = FindTestPosition(aITestPositionID).position;
            return SpawnAttractiveObject(InteractiveObjectInitialization, attractiveObjectSpawnPosition);
        }

        public static InteractiveObjectType SpawnAttractiveObject(InteractiveObjectInitialization InteractiveObjectInitialization, Vector3 worldPosition)
        {
            return InteractiveObjectInitialization.InstanciateAndInit(worldPosition);
        }

        public static InteractiveObjectType SpawnAIDisarmObject(DisarmObjectInherentData disarmObjectInherentData, Vector3 worldPosition)
        {
            return DisarmObjectModule.Instanciate(worldPosition, disarmObjectInherentData);
        }

        public static TargetZoneModule FindTargetZone(TargetZoneID targetZoneID)
        {
            return GameObject.FindObjectsOfType<TargetZoneModule>().ToArray().Select(t => t).Where(targetZone => targetZone.TargetZoneID == targetZoneID).First();
        }

        public static AttractiveObjectInherentConfigurationData CreateAttractiveObjectInherentConfigurationData(float effectRange, float effectiveTime)
        {
            var attractiveObjectInherentConfigurationData = ScriptableObject.CreateInstance<AttractiveObjectInherentConfigurationData>();
            var randomAttractiveObjectInherentData = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().AttractiveObjectsConfiguration()[AttractiveObjectId._Sewers_1_CheeseAttractive];

            //TODO Attractive instance
            // attractiveObjectInherentConfigurationData.Init(effectRange, effectiveTime, randomAttractiveObjectInherentData.AssociatedInteractiveObjectType);
            return attractiveObjectInherentConfigurationData;
        }

        public static void SetPlayerEscapeComponentValues(GenericPuzzleAIComponents GenericPuzzleAIComponents, float escapeDistance, float escapeSemiAngle, float playerDetectionRadius)
        {
            GenericPuzzleAIComponents.AIPlayerEscapeComponent.EscapeDistance = escapeDistance;
            GenericPuzzleAIComponents.AIPlayerEscapeComponent.EscapeSemiAngle = escapeSemiAngle;
            GenericPuzzleAIComponents.AIPlayerEscapeComponent.PlayerDetectionRadius = playerDetectionRadius;
        }

        public static void SetAIEscapeSemiAngle(InteractiveObjectTestID InteractiveObjectTestID, AbstractAIComponents abstractAIComponents, float escapeSemiAngle)
        {
            if (abstractAIComponents.GetType() == typeof(GenericPuzzleAIComponents))
            {
                GenericPuzzleAIComponents genericPuzzleAIComponents = (GenericPuzzleAIComponents)abstractAIComponents;
                genericPuzzleAIComponents.AIProjectileEscapeWithCollisionComponent.EscapeSemiAngleV2.Values[InteractiveObjectTestIDTree.InteractiveObjectTestIDs[InteractiveObjectTestID].LaunchProjectileID] = escapeSemiAngle;
            }
        }

        public static void SetAIEscapeDistanceFromProjectile(InteractiveObjectTestID InteractiveObjectTestID, AbstractAIComponents abstractAIComponents, float escapeDistance)
        {
            if (abstractAIComponents.GetType() == typeof(GenericPuzzleAIComponents))
            {
                GenericPuzzleAIComponents genericPuzzleAIComponents = (GenericPuzzleAIComponents)abstractAIComponents;
                genericPuzzleAIComponents.AIProjectileEscapeWithCollisionComponent.EscapeDistanceV2.Values[InteractiveObjectTestIDTree.InteractiveObjectTestIDs[InteractiveObjectTestID].LaunchProjectileID] = escapeDistance;
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
        public static IEnumerator AttractiveObjectYield(InteractiveObjectInitialization InteractiveObjectInitialization, Vector3 worldPosition,
                    Func<InteractiveObjectType, IEnumerator> OnAttractiveObjectSpawn, Func<IEnumerator> OnAttractiveObjectDestroyed)
        {
            var attractiveObjectType = PuzzleSceneTestHelper.SpawnAttractiveObject(InteractiveObjectInitialization, worldPosition);
            yield return new WaitForFixedUpdate();
            if (OnAttractiveObjectSpawn != null)
            {
                yield return OnAttractiveObjectSpawn.Invoke(attractiveObjectType);
            }
            if (OnAttractiveObjectDestroyed != null)
            {
                yield return new WaitForSeconds(InteractiveObjectInitialization.InteractiveObjectInitializationObject.AttractiveObjectInherentConfigurationData.EffectiveTime);
                yield return OnAttractiveObjectDestroyed.Invoke();
            }
        }
        #endregion

        #region Projectile
        public static IEnumerator ProjectileYield(InteractiveObjectInitialization InteractiveObjectInitialization,
                            Vector3 projectilePoistion,
                Func<InteractiveObjectType, IEnumerator> OnProjectileSpawn, Func<IEnumerator> OnDistanceReached)
        {
            var projectile = SpawnProjectile(InteractiveObjectInitialization, projectilePoistion);
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

        public static IEnumerator ProjectileToAttractiveYield(InteractiveObjectInitialization InteractiveObjectInitialization, Vector3 projectilePosition,
               Func<InteractiveObjectType, IEnumerator> OnProjectileSpawn, Func<InteractiveObjectType, IEnumerator> OnProjectileTurnedIntoAttractive, Func<IEnumerator> OnDistanceReached)
        {
            var projectile = SpawnProjectile(InteractiveObjectInitialization, projectilePosition);

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

        public static IEnumerator ProjectileToAttractiveYield(InteractiveObjectInitialization InteractiveObjectInitialization, TestPositionID projectilePosition,
            Func<InteractiveObjectType, IEnumerator> OnProjectileSpawn, Func<InteractiveObjectType, IEnumerator> OnProjectileTurnedIntoAttractive, Func<IEnumerator> OnDistanceReached)
        {
            yield return ProjectileToAttractiveYield(InteractiveObjectInitialization, FindTestPosition(projectilePosition).position,
                 OnProjectileSpawn, OnProjectileTurnedIntoAttractive, OnDistanceReached);
        }
        #endregion

        #region TargetZone
        public static IEnumerator TargetZoneYield(TargetZoneInherentData targetZoneInherentData, Vector3 targetZonePosition,
            Func<InteractiveObjectType, IEnumerator> OnTargetZoneSpawn, Func<IEnumerator> OnDistanceReached)
        {
            var puzzlePrefabConfiguration = GameObject.FindObjectOfType<PuzzleStaticConfigurationContainer>().PuzzleStaticConfiguration.PuzzlePrefabConfiguration;
            var puzzleGameConfiguration = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().PuzzleGameConfiguration;
            var targetZone = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseInteractiveObjectType, targetZonePosition, Quaternion.identity);
            InteractiveObjectTypeDefinitionConfigurationInherentDataBuilder.TargetZone(
                    RangeTypeObjectDefinitionConfigurationInherentDataBuilder.BoxRangeNoObstacleListener(Vector3.zero, Vector3.zero, RangeTypeID.TARGET_ZONE)
                  ).DefineInteractiveObject(targetZone, puzzlePrefabConfiguration, puzzleGameConfiguration);
            targetZone.Init(new InteractiveObjectInitializationObject() { TargetZoneInherentData = targetZoneInherentData });

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
        public static IEnumerator ProjectileIngoreTargetYield(InteractiveObjectInitialization InteractiveObjectInitialization, Vector3 projectilePoistion,
            Func<IEnumerator> OnBeforeSecondProjectileSpawn,
            Func<InteractiveObjectType, IEnumerator> OnSecondProjectileSpawned,
            Func<IEnumerator> OnSecondProjectileDistanceReached)
        {
            SpawnProjectile(InteractiveObjectInitialization, projectilePoistion);
            yield return new WaitForFixedUpdate();
            if (OnBeforeSecondProjectileSpawn != null)
            {
                yield return OnBeforeSecondProjectileSpawn.Invoke();
            }
            var projectile = SpawnProjectile(InteractiveObjectInitialization, projectilePoistion);
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
        public static IEnumerator FearYield(Vector3 projectilePosition, InteractiveObjectTestID projectileID,
            Func<IEnumerator> OnFearTriggered, Func<IEnumerator> OnFearEnded, float fearTime, GenericPuzzleAIBehavior mouseAIBheavior)
        {
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(projectileID, mouseAIBheavior.AIComponents, 1f);
            yield return ProjectileYield(ProjectileInteractiveObjectDefinitions.ExplodingProjectile(projectileID, 1000, 1), projectilePosition,
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

        public static IEnumerator EscapeFromPlayerIgnoreTargetYield(PlayerManager playerManager, NPCAIManager nPCAIManager, InteractiveObjectInitialization InteractiveObjectInitialization, Vector3 projectilePosition,
            Func<IEnumerator> OnBeforeSettingPosition, Func<IEnumerator> OnSamePositionSetted, Func<IEnumerator> OnDestinationReached)
        {
            yield return PuzzleSceneTestHelper.ProjectileYield(InteractiveObjectInitialization, projectilePosition,
                OnProjectileSpawn: (InteractiveObjectType launchProjectile) =>
                {
                    return PuzzleSceneTestHelper.EscapeFromPlayerYield(playerManager, nPCAIManager, OnBeforeSettingPosition, OnSamePositionSetted, null);
                },
                OnDistanceReached: OnDestinationReached);
        }
        #endregion

        #region AI disarm object
        public static IEnumerator DisarmObjectYield(InteractiveObjectInitialization disarmObjectInitialization, Vector3 worldPosition, Func<InteractiveObjectType, IEnumerator> OnDisarmObjectSpawn,
                    Func<InteractiveObjectType, IEnumerator> OnDisarmTimerOver)
        {
            var disarmObject = disarmObjectInitialization.InstanciateAndInit(worldPosition);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
            if (OnDisarmObjectSpawn != null)
            {
                yield return OnDisarmObjectSpawn.Invoke(disarmObject);
            }
            if (OnDisarmTimerOver != null)
            {
                yield return new WaitForSeconds(disarmObjectInitialization.InteractiveObjectInitializationObject.DisarmObjectInherentData.DisarmTime);
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
                    genericPuzzleAIComponents.AIProjectileEscapeWithCollisionComponent.EscapeDistanceV2.Values.Clear();
                    foreach (LaunchProjectileID projectileId in Enum.GetValues(typeof(LaunchProjectileID)))
                    {
                        genericPuzzleAIComponents.AIProjectileEscapeWithCollisionComponent.EscapeDistanceV2.Values[projectileId] = 25f;
                    }
                }

                genericPuzzleAIComponents.AITargetZoneComponent.TargetZoneEscapeDistance = 50f;

                genericPuzzleAIComponents.AIFearStunComponent.FOVSumThreshold = 20f;
                genericPuzzleAIComponents.AIFearStunComponent.TimeWhileBeginFeared = 2f;

                genericPuzzleAIComponents.AIPlayerEscapeComponent.PlayerDetectionRadius = -1f;
            }
        }
    }
}
