using GameConfigurationID;
using RTPuzzle;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Tests
{
    public class _1_Town_StartTutorial_AIBehaviorTest : AbstractPuzzleSceneTest
    {

        [UnityTest]
        public IEnumerator AI_MoveTowardPlayer_NominalTest()
        {
            yield return this.Before(SceneConstants._1_Level_StartTutorial_AIBehaviorTest, AiID._1_Town_StartTutorial_AITest);
            yield return new WaitForFixedUpdate();
            var aiManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(this.chosenId);
            var aiBehavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            var playerManagerDataRetriever = GameObject.FindObjectOfType<PlayerManagerDataRetriever>();

            Assert.IsFalse(aiBehavior.IsMovingTowardPlayer());
            Assert.IsTrue(aiBehavior.IsPatrolling());

            //The player move to ai sight
            yield return PuzzleSceneTestHelper.MoveTowardPlayerYield(TestPositionID.MOVE_TOWARDS_PLAYER_INSIGHT,
                       OnPlayerInSight: () =>
                       {
                           Assert.IsTrue(aiBehavior.IsMovingTowardPlayer());
                           Assert.IsFalse(aiBehavior.IsPatrolling());
                           Assert.AreEqual(aiBehavior.AIPlayerMoveTowardPlayerManager.GetCurrentTarget().collider, playerManagerDataRetriever.GetPlayerPuzzleLogicRootCollier());
                           return null;
                       }
                   );
        }

        [UnityTest]
        public IEnumerator AI_MoveTowardPlayer_KeepTargetingPlayerIfPlayerMoves()
        {
            yield return this.Before(SceneConstants._1_Level_StartTutorial_AIBehaviorTest, AiID._1_Town_StartTutorial_AITest);
            yield return new WaitForFixedUpdate();
            var aiManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(this.chosenId);
            var aiBehavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            var playerManagerDataRetriever = GameObject.FindObjectOfType<PlayerManagerDataRetriever>();

            //The player move to ai sight
            yield return PuzzleSceneTestHelper.MoveTowardPlayerYield(TestPositionID.MOVE_TOWARDS_PLAYER_INSIGHT,
                       OnPlayerInSight: () =>
                       {
                           NavMeshHit hit;
                           NavMesh.SamplePosition(playerManagerDataRetriever.GetPlayerRigidBody().position, out hit, 1f, NavMesh.AllAreas);
                           Assert.AreEqual(aiManager.GetAgent().destination, hit.position);
                           return PuzzleSceneTestHelper.MovePlayerAndWaitForFixed(playerManagerDataRetriever.GetPlayerRigidBody().position + playerManagerDataRetriever.GetPlayerRigidBody().transform.forward,
                                    OnPlayerMoved: () =>
                                    {
                                        NavMesh.SamplePosition(playerManagerDataRetriever.GetPlayerRigidBody().position, out hit, 1f, NavMesh.AllAreas);
                                        Assert.AreEqual(aiManager.GetAgent().destination, hit.position);
                                        return null;
                                    }
                               );
                       }
                   );
        }

        [UnityTest]
        public IEnumerator AI_MoveTowardPlayer_MoveToLastDestinationIfPlayerIsOutOfRange()
        {
            yield return this.Before(SceneConstants._1_Level_StartTutorial_AIBehaviorTest, AiID._1_Town_StartTutorial_AITest);
            yield return new WaitForFixedUpdate();
            var aiManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(this.chosenId);
            var aiBehavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            var playerManagerDataRetriever = GameObject.FindObjectOfType<PlayerManagerDataRetriever>();

            //The player move to ai sight
            yield return PuzzleSceneTestHelper.MoveTowardPlayerYield(TestPositionID.MOVE_TOWARDS_PLAYER_INSIGHT,
                       OnPlayerInSight: () =>
                       {
                           NavMeshHit hit;
                           NavMesh.SamplePosition(playerManagerDataRetriever.GetPlayerRigidBody().position, out hit, 1f, NavMesh.AllAreas);
                           return PuzzleSceneTestHelper.MovePlayerAndWaitForFixed(new Vector3(999f, 0f, 9999f),
                                    OnPlayerMoved: () =>
                                    {
                                        //Destination is keeped
                                        Assert.AreEqual(aiManager.GetAgent().destination, hit.position);
                                        return null;
                                    }
                               );
                       }
            );
        }

        [UnityTest]
        public IEnumerator AI_MoveTowardPlayer_NotInterruptedBy_ProjectileToAttractive()
        {
            yield return this.Before(SceneConstants._1_Level_StartTutorial_AIBehaviorTest, AiID._1_Town_StartTutorial_AITest);
            yield return new WaitForFixedUpdate();
            var aiManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(this.chosenId);
            var aiBehavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();

            Assert.IsFalse(aiBehavior.IsMovingTowardPlayer());
            Assert.IsFalse(aiBehavior.IsInfluencedByAttractiveObject());

            //The player move to ai sight
            yield return PuzzleSceneTestHelper.MoveTowardPlayerYield(TestPositionID.MOVE_TOWARDS_PLAYER_INSIGHT,
                         OnPlayerInSight: () =>
                         {
                             Assert.IsTrue(aiBehavior.IsMovingTowardPlayer());
                             Assert.IsFalse(aiBehavior.IsInfluencedByAttractiveObject());

                             return PuzzleSceneTestHelper.ProjectileToAttractiveYield(PuzzleSceneTestHelper.CreateProjectileInherentData(9999f, 9999f, LaunchProjectileId.TEST_PROJECTILE_TOATTRACTIVE, false, true), 9999f, 9999f, playerManager.transform.position,
                                         OnProjectileSpawn: null,
                                         OnProjectileTurnedIntoAttractive: (InteractiveObjectType projectile) =>
                                         {
                                             Assert.IsTrue(aiBehavior.IsMovingTowardPlayer());
                                             Assert.IsFalse(aiBehavior.IsInfluencedByAttractiveObject());
                                             return null;
                                         },
                                         OnDistanceReached: null
                             );
                         }
                     );
        }

        [UnityTest]
        public IEnumerator AI_ProjectileToAttractive_IsAttracted()
        {
            yield return this.Before(SceneConstants._1_Level_StartTutorial_AIBehaviorTest, AiID._1_Town_StartTutorial_AITest);
            yield return new WaitForFixedUpdate();
            var aiManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(this.chosenId);
            var aiBehavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();

            yield return PuzzleSceneTestHelper.ProjectileToAttractiveYield(PuzzleSceneTestHelper.CreateProjectileInherentData(9999f, 9999f, LaunchProjectileId.TEST_PROJECTILE_TOATTRACTIVE, false, true), 9999f, 9999f, TestPositionID.PROJECTILE_TOATTRACTIVE_NOMINAL,
                OnProjectileSpawn: (InteractiveObjectType projectile) =>
                {
                    Assert.IsFalse(aiBehavior.IsInfluencedByAttractiveObject());
                    Assert.IsFalse(aiBehavior.IsEscapingFromProjectileWithTargetZones());
                    Assert.IsFalse(aiBehavior.IsEscapingWithoutTarget());
                    return null;
                },
                OnProjectileTurnedIntoAttractive: (InteractiveObjectType projectile) =>
                {
                    Assert.IsTrue(aiBehavior.IsInfluencedByAttractiveObject());
                    Assert.IsFalse(aiBehavior.IsEscapingFromProjectileWithTargetZones());
                    Assert.IsFalse(aiBehavior.IsEscapingWithoutTarget());

                    Assert.IsTrue(projectile.GetModule<LaunchProjectileModule>() == null);

                    return null;
                },
                OnDistanceReached: null);
        }

        [UnityTest]
        public IEnumerator AI_ProjectileToAttractive_InterruptedBy_MoveTowardPlayer()
        {
            yield return this.Before(SceneConstants._1_Level_StartTutorial_AIBehaviorTest, AiID._1_Town_StartTutorial_AITest);
            yield return new WaitForFixedUpdate();
            var aiManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(this.chosenId);
            var aiBehavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();

            yield return PuzzleSceneTestHelper.ProjectileToAttractiveYield(PuzzleSceneTestHelper.CreateProjectileInherentData(9999f, 9999f, LaunchProjectileId.TEST_PROJECTILE_TOATTRACTIVE, false, true), 9999f, 9999f, TestPositionID.PROJECTILE_TOATTRACTIVE_NOMINAL,
                OnProjectileSpawn: null,
                OnProjectileTurnedIntoAttractive: (InteractiveObjectType projectile) =>
                {
                 //   Debug.Break();
                    Assert.IsTrue(aiBehavior.IsInfluencedByAttractiveObject());
                    Assert.IsFalse(aiBehavior.IsMovingTowardPlayer());

                    return PuzzleSceneTestHelper.MoveTowardPlayerYield(TestPositionID.PROJECTILE_TOATTRACTIVE_NOMINAL,
                            OnPlayerInSight: () =>
                            {
                                Debug.Log(MyLog.Format("END"));
                                Assert.IsFalse(aiBehavior.IsInfluencedByAttractiveObject());
                                Assert.IsTrue(aiBehavior.IsMovingTowardPlayer());
                                return null;
                            }
                        );
                },
                OnDistanceReached: null);
        }

    }
}
