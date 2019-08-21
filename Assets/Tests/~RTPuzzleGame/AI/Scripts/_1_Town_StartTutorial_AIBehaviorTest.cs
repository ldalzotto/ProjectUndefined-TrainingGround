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
                           Debug.Log(MyLog.Format(aiManager.GetAgent().destination.ToString("F4")));
                           return PuzzleSceneTestHelper.MovePlayerAndWaitForFixed(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.MOVE_TOWARDS_PLAYER_OUTOFSIGHT).position,
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

                             return PuzzleSceneTestHelper.ProjectileToAttractiveYield(ProjectileInteractiveObjectDefinitions.MutateToAttractiveProjectile(9999f, 9999f, 9999f, 9999f), playerManager.transform.position,
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
        public IEnumerator AI_MoveTowardPlayer_NotInterruptedBy_DisarmObject()
        {
            yield return this.Before(SceneConstants._1_Level_StartTutorial_AIBehaviorTest, AiID._1_Town_StartTutorial_AITest);
            yield return new WaitForFixedUpdate();
            var aiManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(this.chosenId);
            var aiBehavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();

            Assert.IsFalse(aiBehavior.IsMovingTowardPlayer());
            Assert.IsFalse(aiBehavior.IsDisarmingObject());

            //The player move to ai sight
            yield return PuzzleSceneTestHelper.MoveTowardPlayerYield(TestPositionID.MOVE_TOWARDS_PLAYER_INSIGHT,
                         OnPlayerInSight: () =>
                         {
                             Assert.IsTrue(aiBehavior.IsMovingTowardPlayer());
                             Assert.IsFalse(aiBehavior.IsDisarmingObject());

                             return PuzzleSceneTestHelper.DisarmObjectYield(DisarmInteractiveObjectDefinition.OnlyDisarmObject(InteractiveObjectTestID.TEST_1, 999f, 0.05f), aiManager.transform.position,
                                    OnDisarmObjectSpawn: (InteractiveObjectType disarmObject) =>
                                    {
                                        Assert.IsTrue(aiBehavior.IsMovingTowardPlayer());
                                        Assert.IsFalse(aiBehavior.IsDisarmingObject());
                                        return null;
                                    },
                                    OnDisarmTimerOver: null
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

            yield return PuzzleSceneTestHelper.ProjectileToAttractiveYield(ProjectileInteractiveObjectDefinitions.MutateToAttractiveProjectile(9999f, 9999f, 9999f, 9999f), TestPositionID.PROJECTILE_TOATTRACTIVE_NOMINAL,
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

            yield return PuzzleSceneTestHelper.ProjectileToAttractiveYield(ProjectileInteractiveObjectDefinitions.MutateToAttractiveProjectile(9999f, 9999f, 9999f, 9999f), TestPositionID.PROJECTILE_TOATTRACTIVE_NOMINAL,
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

        [UnityTest]
        public IEnumerator AI_DisarmObject_NominalTest()
        {
            yield return this.Before(SceneConstants._1_Level_StartTutorial_AIBehaviorTest, AiID._1_Town_StartTutorial_AITest);
            yield return new WaitForFixedUpdate();
            var aiManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(this.chosenId);
            var aiBehavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();

            Assert.IsFalse(aiBehavior.IsDisarmingObject());

            yield return PuzzleSceneTestHelper.DisarmObjectYield(DisarmInteractiveObjectDefinition.OnlyDisarmObject(InteractiveObjectTestID.TEST_1, 999f, 0.05f), aiManager.transform.position,
                    OnDisarmObjectSpawn: (InteractiveObjectType disarmObject) =>
                    {
                        Assert.IsTrue(aiBehavior.IsDisarmingObject());
                    //We ensure that the AI is not moving
                    Assert.IsFalse(aiManager.GetAgent().hasPath);
                        return null;
                    },
                    OnDisarmTimerOver: (InteractiveObjectType disarmObject) =>
                    {
                        Assert.IsFalse(aiBehavior.IsDisarmingObject());
                        return null;
                    }
                );
        }

        [UnityTest]
        public IEnumerator AI_DisarmObject_InterruptedBy_MoveTowardPlayer()
        {
            yield return this.Before(SceneConstants._1_Level_StartTutorial_AIBehaviorTest, AiID._1_Town_StartTutorial_AITest);
            yield return new WaitForFixedUpdate();
            var aiManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(this.chosenId);
            var aiBehavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();

            Assert.IsFalse(aiBehavior.IsDisarmingObject());
            Assert.IsFalse(aiBehavior.IsMovingTowardPlayer());

            yield return PuzzleSceneTestHelper.DisarmObjectYield(DisarmInteractiveObjectDefinition.OnlyDisarmObject(InteractiveObjectTestID.TEST_1, 999f, 0.05f), aiManager.transform.position,
                    OnDisarmObjectSpawn: (InteractiveObjectType disarmObject) =>
                    {
                        Assert.IsTrue(aiBehavior.IsDisarmingObject());
                        Assert.IsFalse(aiBehavior.IsMovingTowardPlayer());

                        return PuzzleSceneTestHelper.MoveTowardPlayerYield(TestPositionID.MOVE_TOWARDS_PLAYER_INSIGHT,
                            OnPlayerInSight: () =>
                            {
                                Assert.IsFalse(aiBehavior.IsDisarmingObject());
                                Assert.IsTrue(aiBehavior.IsMovingTowardPlayer());
                                return null;
                            }
                         );
                    },
                    OnDisarmTimerOver: null
                );
        }

        [UnityTest]
        public IEnumerator AI_DisarmObject_NotInterruptedBy_AttractiveObject()
        {
            yield return this.Before(SceneConstants._1_Level_StartTutorial_AIBehaviorTest, AiID._1_Town_StartTutorial_AITest);
            yield return new WaitForFixedUpdate();
            var aiManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(this.chosenId);
            var aiBehavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();

            Assert.IsFalse(aiBehavior.IsDisarmingObject());
            Assert.IsFalse(aiBehavior.IsInfluencedByAttractiveObject());

            yield return PuzzleSceneTestHelper.DisarmObjectYield(DisarmInteractiveObjectDefinition.OnlyDisarmObject(InteractiveObjectTestID.TEST_1, 999f, 0.05f), aiManager.transform.position,
                    OnDisarmObjectSpawn: (InteractiveObjectType disarmObject) =>
                    {
                        Assert.IsTrue(aiBehavior.IsDisarmingObject());
                        Assert.IsFalse(aiBehavior.IsInfluencedByAttractiveObject());

                        return PuzzleSceneTestHelper.AttractiveObjectYield(PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(999999f, 0.2f), aiManager.transform.position,
                           OnAttractiveObjectSpawn: (InteractiveObjectType attractiveObject) =>
                           {
                               Assert.IsTrue(aiBehavior.IsDisarmingObject());
                               Assert.IsFalse(aiBehavior.IsInfluencedByAttractiveObject());
                               return null;
                           },
                           OnAttractiveObjectDestroyed: null
                           );
                    },
                    OnDisarmTimerOver: null
                );
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_InterruptedBy_DisarmObject()
        {
            yield return this.Before(SceneConstants._1_Level_StartTutorial_AIBehaviorTest, AiID._1_Town_StartTutorial_AITest);
            yield return new WaitForFixedUpdate();
            yield return null;
            var aiManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(this.chosenId);
            var aiBehavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();

            Assert.IsFalse(aiBehavior.IsDisarmingObject());
            Assert.IsFalse(aiBehavior.IsInfluencedByAttractiveObject());

            yield return PuzzleSceneTestHelper.AttractiveObjectYield(PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(999999f, 0.2f), aiManager.transform.position,
               OnAttractiveObjectSpawn: (InteractiveObjectType attractiveObject) =>
               {
                   Assert.IsFalse(aiBehavior.IsDisarmingObject());
                   Assert.IsTrue(aiBehavior.IsInfluencedByAttractiveObject());
                   return PuzzleSceneTestHelper.DisarmObjectYield(DisarmInteractiveObjectDefinition.OnlyDisarmObject(InteractiveObjectTestID.TEST_1, 999f, 0.05f), aiManager.transform.position,
                    OnDisarmObjectSpawn: (InteractiveObjectType disarmObject) =>
                    {
                        Assert.IsTrue(aiBehavior.IsDisarmingObject());
                        Assert.IsFalse(aiBehavior.IsInfluencedByAttractiveObject());
                        return null;
                    },
                    OnDisarmTimerOver: null
                );
               },
               OnAttractiveObjectDestroyed: null
               );

        }


    }
}
