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
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants._1_Level_StartTutorial_AIBehaviorTest, () =>
            {
                aiManager = AIObjectDefinition.TownAIV2(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_1).Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return new WaitForFixedUpdate();
            var aiBehavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            var playerManagerDataRetriever = GameObject.FindObjectOfType<PlayerManagerDataRetriever>();

            Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIMoveTowardPlayerManager>());
            Assert.IsTrue(aiBehavior.IsManagerEnabled<AbstractAIPatrolComponentManager>());
            //The player move to ai sight
            yield return PuzzleSceneTestHelper.MoveTowardPlayerYield(TestPositionID.MOVE_TOWARDS_PLAYER_INSIGHT,
                       OnPlayerInSight: () =>
                       {
                           Assert.IsTrue(aiBehavior.IsManagerEnabled<AbstractAIMoveTowardPlayerManager>());
                           Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIPatrolComponentManager>());
                           Assert.AreEqual(aiBehavior.GetAIManager<AbstractAIMoveTowardPlayerManager>().GetCurrentTarget().collider, playerManagerDataRetriever.GetPlayerPuzzleLogicRootCollier());
                           return null;
                       }
                   );
        }

        [UnityTest]
        public IEnumerator AI_MoveTowardPlayer_KeepTargetingPlayerIfPlayerMoves()
        {
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants._1_Level_StartTutorial_AIBehaviorTest, () =>
            {
                aiManager = AIObjectDefinition.TownAIV2(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_1).Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return new WaitForFixedUpdate();
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
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants._1_Level_StartTutorial_AIBehaviorTest, () =>
            {
                aiManager = AIObjectDefinition.TownAIV2(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_1).Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return new WaitForFixedUpdate();
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
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants._1_Level_StartTutorial_AIBehaviorTest, () =>
            {
                aiManager = AIObjectDefinition.TownAIV2(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_1).Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return new WaitForFixedUpdate();
            var aiBehavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();

            Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIMoveTowardPlayerManager>());
            Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
            //The player move to ai sight
            yield return PuzzleSceneTestHelper.MoveTowardPlayerYield(TestPositionID.MOVE_TOWARDS_PLAYER_INSIGHT,
                         OnPlayerInSight: () =>
                         {
                             Assert.IsTrue(aiBehavior.IsManagerEnabled<AbstractAIMoveTowardPlayerManager>());
                             Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());

                             return PuzzleSceneTestHelper.ProjectileToAttractiveYield(ProjectileInteractiveObjectDefinitions.MutateToAttractiveProjectile(InteractiveObjectTestID.TEST_1, 9999f, 9999f, 9999f, 9999f), aiManager, playerManager.transform.position,
                                         OnProjectileSpawn: null,
                                         OnProjectileTurnedIntoAttractive: (InteractiveObjectType projectile) =>
                                         {
                                             Assert.IsTrue(aiBehavior.IsManagerEnabled<AbstractAIMoveTowardPlayerManager>());
                                             Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
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
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants._1_Level_StartTutorial_AIBehaviorTest, () =>
            {
                aiManager = AIObjectDefinition.TownAIV2(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_1).Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return new WaitForFixedUpdate();
            var aiBehavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();

            Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIMoveTowardPlayerManager>());
            Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIDisarmObjectManager>());
            //The player move to ai sight
            yield return PuzzleSceneTestHelper.MoveTowardPlayerYield(TestPositionID.MOVE_TOWARDS_PLAYER_INSIGHT,
                         OnPlayerInSight: () =>
                         {
                             Assert.IsTrue(aiBehavior.IsManagerEnabled<AbstractAIMoveTowardPlayerManager>());
                             Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIDisarmObjectManager>());

                             return PuzzleSceneTestHelper.DisarmObjectYield(DisarmInteractiveObjectDefinition.OnlyDisarmObject(InteractiveObjectTestID.TEST_1, 999f, 0.05f), aiManager.transform.position,
                                    OnDisarmObjectSpawn: (InteractiveObjectType disarmObject) =>
                                    {
                                        Assert.IsTrue(aiBehavior.IsManagerEnabled<AbstractAIMoveTowardPlayerManager>());
                                        Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIDisarmObjectManager>());
                                        return null;
                                    },
                                    OnDisarmTimerOver: null
                                );
                         }
                     );
        }

        [UnityTest]
        public IEnumerator AI_MoveTowardPlayer_WhenAIMoves_WhenPlayerIsMotionless()
        {
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants._1_Level_StartTutorial_AIBehaviorTest, () =>
            {
                aiManager = AIObjectDefinition.TownAIV2(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_1, sphereSightInfinite: true).Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });

            var aiBehavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIMoveTowardPlayerManager>());
            yield return new WaitForFixedUpdate();
            Assert.IsTrue(aiBehavior.IsManagerEnabled<AbstractAIMoveTowardPlayerManager>());
        }

        [UnityTest]
        public IEnumerator AI_ProjectileToAttractive_IsAttracted()
        {
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants._1_Level_StartTutorial_AIBehaviorTest, () =>
            {
                aiManager = AIObjectDefinition.TownAIV2(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_1).Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return new WaitForFixedUpdate();
            var aiBehavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();

            yield return PuzzleSceneTestHelper.ProjectileToAttractiveYield(ProjectileInteractiveObjectDefinitions.MutateToAttractiveProjectile(InteractiveObjectTestID.TEST_1, 9999f, 9999f, 9999f, 9999f), aiManager, TestPositionID.PROJECTILE_TOATTRACTIVE_NOMINAL,
                OnProjectileSpawn: (InteractiveObjectType projectile) =>
                {
                    Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                    Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                    Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
                    return null;
                },
                OnProjectileTurnedIntoAttractive: (InteractiveObjectType projectile) =>
                {
                    Assert.IsTrue(aiBehavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                    Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                    Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());

                    Assert.IsTrue(projectile.GetModule<LaunchProjectileModule>() == null);

                    return null;
                },
                OnDistanceReached: null);
        }

        [UnityTest]
        public IEnumerator AI_ProjectileToAttractive_InterruptedBy_MoveTowardPlayer()
        {
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants._1_Level_StartTutorial_AIBehaviorTest, () =>
            {
                aiManager = AIObjectDefinition.TownAIV2(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_1).Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return new WaitForFixedUpdate();
            var aiBehavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();

            yield return PuzzleSceneTestHelper.ProjectileToAttractiveYield(ProjectileInteractiveObjectDefinitions.MutateToAttractiveProjectile(InteractiveObjectTestID.TEST_1, 9999f, 9999f, 9999f, 9999f), aiManager, TestPositionID.PROJECTILE_TOATTRACTIVE_NOMINAL,
                OnProjectileSpawn: null,
                OnProjectileTurnedIntoAttractive: (InteractiveObjectType projectile) =>
                {
                    //   Debug.Break();
                    Assert.IsTrue(aiBehavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                    Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIMoveTowardPlayerManager>());

                    return PuzzleSceneTestHelper.MoveTowardPlayerYield(TestPositionID.PROJECTILE_TOATTRACTIVE_NOMINAL,
                            OnPlayerInSight: () =>
                            {
                                Debug.Log(MyLog.Format("END"));
                                Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                                Assert.IsTrue(aiBehavior.IsManagerEnabled<AbstractAIMoveTowardPlayerManager>());
                                return null;
                            }
                        );
                },
                OnDistanceReached: null);
        }

        [UnityTest]
        public IEnumerator AI_DisarmObject_NominalTest()
        {
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants._1_Level_StartTutorial_AIBehaviorTest, () =>
            {
                aiManager = AIObjectDefinition.TownAIV2(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_1).Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return new WaitForFixedUpdate();
            var aiBehavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();

            Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIDisarmObjectManager>());

            yield return PuzzleSceneTestHelper.DisarmObjectYield(DisarmInteractiveObjectDefinition.OnlyDisarmObject(InteractiveObjectTestID.TEST_1, 999f, 0.05f), aiManager.transform.position,
                    OnDisarmObjectSpawn: (InteractiveObjectType disarmObject) =>
                    {
                        Assert.IsTrue(aiBehavior.IsManagerEnabled<AbstractAIDisarmObjectManager>());
                        //We ensure that the AI is not moving
                        Assert.IsFalse(aiManager.GetAgent().hasPath);
                        return null;
                    },
                    OnDisarmTimerOver: (InteractiveObjectType disarmObject) =>
                    {
                        Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIDisarmObjectManager>());
                        return null;
                    }
                );
        }

        [UnityTest]
        public IEnumerator AI_DisarmObject_InterruptedBy_MoveTowardPlayer()
        {
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants._1_Level_StartTutorial_AIBehaviorTest, () =>
            {
                aiManager = AIObjectDefinition.TownAIV2(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_1).Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return new WaitForFixedUpdate();
            var aiBehavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();

            Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIDisarmObjectManager>());
            Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIMoveTowardPlayerManager>());

            yield return PuzzleSceneTestHelper.DisarmObjectYield(DisarmInteractiveObjectDefinition.OnlyDisarmObject(InteractiveObjectTestID.TEST_1, 999f, 0.05f), aiManager.transform.position,
                    OnDisarmObjectSpawn: (InteractiveObjectType disarmObject) =>
                    {
                        Assert.IsTrue(aiBehavior.IsManagerEnabled<AbstractAIDisarmObjectManager>());
                        Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIMoveTowardPlayerManager>());

                        return PuzzleSceneTestHelper.MoveTowardPlayerYield(TestPositionID.MOVE_TOWARDS_PLAYER_INSIGHT,
                            OnPlayerInSight: () =>
                            {
                                Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIDisarmObjectManager>());
                                Assert.IsTrue(aiBehavior.IsManagerEnabled<AbstractAIMoveTowardPlayerManager>());
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
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants._1_Level_StartTutorial_AIBehaviorTest, () =>
            {
                aiManager = AIObjectDefinition.TownAIV2(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_1).Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return new WaitForFixedUpdate();
            var aiBehavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();

            Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIDisarmObjectManager>());
            Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());

            yield return PuzzleSceneTestHelper.DisarmObjectYield(DisarmInteractiveObjectDefinition.OnlyDisarmObject(InteractiveObjectTestID.TEST_1, 999f, 0.05f), aiManager.transform.position,
                    OnDisarmObjectSpawn: (InteractiveObjectType disarmObject) =>
                    {
                        Assert.IsTrue(aiBehavior.IsManagerEnabled<AbstractAIDisarmObjectManager>());
                        Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());

                        return PuzzleSceneTestHelper.AttractiveObjectYield(AttractiveObjectDefinition.AttractiveObjectOnly(InteractiveObjectTestID.TEST_2, 999999f, 0.2f), aiManager.transform.position,
                           OnAttractiveObjectSpawn: (InteractiveObjectType attractiveObject) =>
                           {
                               Assert.IsTrue(aiBehavior.IsManagerEnabled<AbstractAIDisarmObjectManager>());
                               Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
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
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants._1_Level_StartTutorial_AIBehaviorTest, () =>
            {
                aiManager = AIObjectDefinition.TownAIV2(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_1).Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return new WaitForFixedUpdate();
            yield return null;
            var aiBehavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();

            Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIDisarmObjectManager>());
            Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());

            yield return PuzzleSceneTestHelper.AttractiveObjectYield(AttractiveObjectDefinition.AttractiveObjectOnly(InteractiveObjectTestID.TEST_1, 999999f, 0.2f), aiManager.transform.position,
               OnAttractiveObjectSpawn: (InteractiveObjectType attractiveObject) =>
               {
                   Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIDisarmObjectManager>());
                   Assert.IsTrue(aiBehavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                   return PuzzleSceneTestHelper.DisarmObjectYield(DisarmInteractiveObjectDefinition.OnlyDisarmObject(InteractiveObjectTestID.TEST_2, 999f, 0.05f), aiManager.transform.position,
                    OnDisarmObjectSpawn: (InteractiveObjectType disarmObject) =>
                    {
                        Assert.IsTrue(aiBehavior.IsManagerEnabled<AbstractAIDisarmObjectManager>());
                        Assert.IsFalse(aiBehavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
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
