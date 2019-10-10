using CoreGame;
using GameConfigurationID;
using InteractiveObjectTest;
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

        public static TargetZoneModule FindTargetZone(TargetZoneID targetZoneID)
        {
            return GameObject.FindObjectsOfType<TargetZoneModule>().ToArray().Select(t => t).Where(targetZone => targetZone.TargetZoneID == targetZoneID).First();
        }

        public static void SetPlayerEscapeComponentValues(AIObjectInitialization AIObjectInitialization, float escapeDistance, float escapeSemiAngle, float playerDetectionRadius)
        {
            AIObjectInitialization.AIObjectTypeDefinitionInherentData.GenericPuzzleAIComponents.GetDefinitionModule<AIPlayerEscapeComponent>().EscapeDistance = escapeDistance;
            AIObjectInitialization.AIObjectTypeDefinitionInherentData.GenericPuzzleAIComponents.GetDefinitionModule<AIPlayerEscapeComponent>().EscapeSemiAngle = escapeSemiAngle;
            AIObjectInitialization.AIObjectTypeDefinitionInherentData.GenericPuzzleAIComponents.GetDefinitionModule<AIPlayerEscapeComponent>().PlayerDetectionRadius = playerDetectionRadius;
        }

        public static void SetAIEscapeSemiAngle(InteractiveObjectTestID ProjectileTestID, AIObjectInitialization AIObjectInitialization, float escapeSemiAngle)
        {
            AIObjectInitialization.AIObjectTypeDefinitionInherentData.GenericPuzzleAIComponents.GetDefinitionModule<AIProjectileEscapeComponent>().EscapeSemiAngleV2.Values[InteractiveObjectTestIDTree.InteractiveObjectTestIDs[ProjectileTestID].LaunchProjectileID] = escapeSemiAngle;
        }

        public static void SetAIEscapeDistanceFromProjectile(InteractiveObjectTestID InteractiveObjectTestID, AIObjectInitialization AIObjectInitialization, float escapeDistance)
        {
            AIObjectInitialization.AIObjectTypeDefinitionInherentData.GenericPuzzleAIComponents.GetDefinitionModule<AIProjectileEscapeComponent>().EscapeDistanceV2.Values[InteractiveObjectTestIDTree.InteractiveObjectTestIDs[InteractiveObjectTestID].LaunchProjectileID] = escapeDistance;
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
            PlayerInteractiveObjectManager.Get().GetPlayerGameObject().Rigidbody.position = playerPosition;
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
                Func<InteractiveObjectType, IEnumerator> OnProjectileSpawn, Func<IEnumerator> OnDistanceReached, Func<AIObjectType> OnDistanceReaderAIObjectType)
        {
            var projectile = SpawnProjectile(InteractiveObjectInitialization, projectilePoistion);
            yield return new WaitForFixedUpdate();
            if (OnProjectileSpawn != null)
            {
                yield return OnProjectileSpawn.Invoke(projectile);
            }
            if (OnDistanceReached != null)
            {
                var agent = OnDistanceReaderAIObjectType.Invoke().GetAgent();
                TestHelperMethods.SetAgentDestinationPositionReached(agent);
                yield return null;
                yield return new WaitForFixedUpdate();
                yield return OnDistanceReached.Invoke();
            }
        }

        public static IEnumerator ProjectileToAttractiveYield(InteractiveObjectInitialization projectileObjectInitialization, AIObjectType aIObjectType, Vector3 projectilePosition,
               Func<InteractiveObjectType, IEnumerator> OnProjectileSpawn, Func<InteractiveObjectType, IEnumerator> OnProjectileTurnedIntoAttractive, Func<IEnumerator> OnDistanceReached)
        {
            var projectile = SpawnProjectile(projectileObjectInitialization, projectilePosition);

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
                var agent = aIObjectType.GetAgent();
                TestHelperMethods.SetAgentDestinationPositionReached(agent);
                yield return null;
                yield return new WaitForFixedUpdate();
                yield return OnDistanceReached.Invoke();
            }
        }

        public static IEnumerator ProjectileToAttractiveYield(InteractiveObjectInitialization InteractiveObjectInitialization, AIObjectType aIObjectType, TestPositionID projectilePosition, 
            Func<InteractiveObjectType, IEnumerator> OnProjectileSpawn, Func<InteractiveObjectType, IEnumerator> OnProjectileTurnedIntoAttractive, Func<IEnumerator> OnDistanceReached)
        {
            yield return ProjectileToAttractiveYield(InteractiveObjectInitialization, aIObjectType, FindTestPosition(projectilePosition).position, 
                 OnProjectileSpawn, OnProjectileTurnedIntoAttractive, OnDistanceReached);
        }
        #endregion

        #region TargetZone
        public static IEnumerator TargetZoneYield(InteractiveObjectTestID InteractiveObjectTestID, TargetZoneInherentData targetZoneInherentData, Vector3 targetZonePosition,
            Func<InteractiveObjectType, IEnumerator> OnTargetZoneSpawn, Func<IEnumerator> OnDistanceReached, Func<AIObjectType> OnDistanceReachedAIObjectType)
        {
            var puzzlePrefabConfiguration = GameObject.FindObjectOfType<PuzzleStaticConfigurationContainer>().PuzzleStaticConfiguration.PuzzlePrefabConfiguration;
            var puzzleGameConfiguration = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().PuzzleGameConfiguration;
            var targetZone = TargetZoneObjectDefinition.TargetZone(InteractiveObjectTestID, targetZoneInherentData).InstanciateAndInit(targetZonePosition);
            yield return new WaitForFixedUpdate();
            if (OnTargetZoneSpawn != null)
            {
                yield return OnTargetZoneSpawn.Invoke(targetZone);
            }
            if (OnDistanceReached != null)
            {
                var agent = OnDistanceReachedAIObjectType.Invoke().GetAgent();
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
            Func<IEnumerator> OnSecondProjectileDistanceReached,
            Func<AIObjectType> OnSecondProjectileDistanceReachedAIObjectType)
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
                var agent = OnSecondProjectileDistanceReachedAIObjectType.Invoke().GetAgent();
                TestHelperMethods.SetAgentDestinationPositionReached(agent);
                yield return null;
                yield return new WaitForFixedUpdate();
                yield return OnSecondProjectileDistanceReached.Invoke();
            }
        }
        #endregion

        #region Fear
        public static IEnumerator FearYield(Vector3 projectilePosition, InteractiveObjectTestID projectileID, AIObjectInitialization AIObjectInitialization,
            Func<IEnumerator> OnFearTriggered, Func<IEnumerator> OnFearEnded, float fearTime, GenericPuzzleAIBehavior mouseAIBheavior)
        {
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(projectileID, AIObjectInitialization, 1f);
            yield return ProjectileYield(ProjectileInteractiveObjectDefinitions.ExplodingProjectile(projectileID, 1000, 1), projectilePosition,
                OnProjectileSpawn: null,
                OnDistanceReached: null,
                OnDistanceReaderAIObjectType: null);
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
    }
}
