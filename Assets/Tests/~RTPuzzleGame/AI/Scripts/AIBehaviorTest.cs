﻿using RTPuzzle;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Tests
{
    public class AIBehaviorTest : AbstractPuzzleSceneTest
    {

        #region Random Patrol
        [UnityTest]
        public IEnumerator AI_RandomPatrol_Nominal_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            // (1) - Initialization process
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var oldPosition = mouseTestAIManager.transform.position;
            // (2) - Let a frame pass to let the AI travel distance
            yield return null;
            var newPosition = mouseTestAIManager.transform.position;
            Assert.AreNotEqual(newPosition, oldPosition);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            // (3) - The AI is in a patrolling state
            Assert.IsTrue(mouseAIBheavior.IsPatrolling());
            // (4) - The AI FOV is not modified for patrolling
            Assert.AreEqual(new FOVSlice(0, 360), mouseAIBheavior.GetFOV().FovSlices[0]);
            var beforeReachDestination = mouseTestAIManager.GetAgent().destination;
            // (5) - The AI has reached the position
            TestHelperMethods.SetAgentDestinationPositionReached(mouseTestAIManager.GetAgent());
            yield return null;
            this.MockPuzzleEventsManagerTest.ClearCalls();
            // (6) - The AI is still in a patrolling state because nothing has happened
            Assert.IsTrue(mouseAIBheavior.IsPatrolling());
            var afterReachDestination = mouseTestAIManager.GetAgent().destination;
            Assert.AreNotEqual(beforeReachDestination, afterReachDestination);
        }

        [UnityTest]
        public IEnumerator AI_RandomPatrol_InterruptedBy_AttractiveObject()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            Assert.IsTrue(mouseAIBheavior.IsPatrolling());
            Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
            yield return PuzzleSceneTestHelper.AttractiveObjectYield(PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(1000, 0.02f), mouseTestAIManager.transform.position,
                OnAttractiveObjectSpawn: (AttractiveObjectType attractiveObjectType) =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsPatrolling());
                    Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject());
                    return null;
                },
                OnAttractiveObjectDestroyed: () =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsPatrolling());
                    Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                    return null;
                });
        }

        [UnityTest]
        public IEnumerator AI_RandomPatrol_InterruptedBy_Projectile()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            Assert.IsTrue(mouseAIBheavior.IsPatrolling());
            Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectile());
            yield return PuzzleSceneTestHelper.ProjectileYield(PuzzleSceneTestHelper.CreateProjectileInherentData(1000, 175f, 0.1f), mouseTestAIManager.transform.position,
                OnProjectileSpawn: (LaunchProjectile projectile) =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsPatrolling());
                    Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectile());
                    return null;
                },
                OnDistanceReached: () =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsPatrolling());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectile());
                    return null;
                });
        }

        [UnityTest]
        public IEnumerator AI_RandomPatrol_InterruptedBy_TargetZone()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return PuzzleSceneTestHelper.TargetZoneYield(new TargetZoneInherentData(1, 10, 170), mouseTestAIManager.transform.position,
                OnTargetZoneSpawn: (TargetZone targetZone) =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsPatrolling());
                    Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone());
                    return null;
                },
                OnDistanceReached: () =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsPatrolling());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone());
                    return null;
                }
                );
        }

        [UnityTest]
        public IEnumerator AI_RandomPatrol_InterruptedBy_ProjectileIgnoreTarget()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return PuzzleSceneTestHelper.ProjectileIngoreTargetYield(PuzzleSceneTestHelper.CreateProjectileInherentData(3, 170, 1), mouseTestAIManager.transform.position,
                OnBeforeSecondProjectileSpawn: null,
                OnSecondProjectileSpawned: (LaunchProjectile projectile) =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsPatrolling());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectile());
                    Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectileIngnoringTargetZones());
                    return null;
                },
                OnSecondProjectileDistanceReached: () =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsPatrolling());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectile());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileIngnoringTargetZones());
                    return null;
                });
        }

        [UnityTest]
        public IEnumerator AI_RandomPatrol_InterruptedBy_Fear()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            var fearTime = 0.05f;
            ((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents).AIFearStunComponent.TimeWhileBeginFeared = fearTime;
            yield return PuzzleSceneTestHelper.FearYield(mouseTestAIManager.transform.position,
                OnFearTriggered: () =>
                {
                    Debug.Log(mouseAIBheavior.ToString());
                    Assert.IsFalse(mouseAIBheavior.IsPatrolling());
                    Assert.IsTrue(mouseAIBheavior.IsFeared());
                    return null;
                },
                OnFearEnded: () =>
                {
                    Debug.Log(mouseAIBheavior.ToString());
                    Assert.IsTrue(mouseAIBheavior.IsPatrolling());
                    Assert.IsFalse(mouseAIBheavior.IsFeared());
                    return null;
                },
                fearTime: fearTime
            );
        }

        #endregion

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_Nominal_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            var launchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            // (1) - Initialization process
            yield return null;
            Assert.IsTrue(mouseAIBheavior.IsPatrolling(), "The AI has no interaction -> Patrolling.");
            var projectileData = PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 90f, 30f);
            var lpTest = PuzzleSceneTestHelper.SpawnProjectile(projectileData, mouseTestAIManager.transform.position, launchProjectileContainerManager);
            // (2) - Wait for projectile to be processed by physics engine. The AI must be in escape from projectile state.
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(mouseAIBheavior.IsPatrolling(), "The AI has been hit, no more patrolling.");
            Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectile(), "The AI has been hit, escaping from projectile while taking into account target zones.");
            Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileIngnoringTargetZones(), "The AI should not ignore target zones while escaping from projectile for the first time.");
            Assert.AreEqual(this.MockPuzzleEventsManagerTest.AiHittedByProjectileCallCount, 1, "The 'hitted by projectile event' must be triggered only once.");
            // (3) - After processing, the projecile must be destroyed
            yield return null;
            Assert.IsNull(lpTest, "The projectile must be detroyed after hit.");
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_WhenDestinationReached_NoMoreEscape_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            var launchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return null;
            Assert.IsTrue(mouseAIBheavior.IsPatrolling(), "The AI has no interaction -> Patrolling.");
            var projectileData = PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 90f, 30f);
            var lpTest = PuzzleSceneTestHelper.SpawnProjectile(projectileData, mouseTestAIManager.transform.position, launchProjectileContainerManager);
            yield return new WaitForFixedUpdate();
            //projectile taken into account
            yield return new WaitForEndOfFrame();
            //clear agent destinations
            var agent = mouseTestAIManager.GetAgent();
            TestHelperMethods.SetAgentDestinationPositionReached(agent);
            // wait for set destination
            yield return null;
            Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectile(), "The AI should no more escape from projectile when destination is reached.");
            Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileIngnoringTargetZones(), "The AI should no more escape from projectile when destination is reached.");
        }

        private IEnumerator AI_ProjectileReceived_FirstTimeNotIntoTargetZone(string sceneName, AiID aiID)
        {
            yield return this.Before(sceneName, aiID);
            var launchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(aiID);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return null;
            var projectileData = PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 90f, 30f);
            var firstProj = PuzzleSceneTestHelper.SpawnProjectile(projectileData, AITestPositionID.PROJECTILE_TARGET_1, launchProjectileContainerManager);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame(); //wait for destination position to update
            var aiDestination = mouseTestAIManager.GetAgent().destination;
            var targetZoneCollider = PuzzleSceneTestHelper.FindTargetZone(TargetZoneID.TEST_TARGET_ZONE).TargetZoneTriggerType.TargetZoneTriggerCollider;
            Assert.IsFalse(targetZoneCollider.bounds.Contains(aiDestination), "AI Destination should not be in the target zone the first hit.");
        }

        private IEnumerator AI_ProjectileReceived_SecondTimeInTargetZone(string sceneName, float? projectileEscapeDistance)
        {
            yield return this.AI_ProjectileReceived_FirstTimeNotIntoTargetZone(sceneName, AiID.MOUSE_TEST);
            var launchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            if (projectileEscapeDistance != null)
            {
                ((GenericPuzzleAIComponents)mouseTestAIManager.GetAIBehavior().AIComponents).AIProjectileEscapeWithCollisionComponent.EscapeDistance = projectileEscapeDistance.Value;
            }
            var projectileData = PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 90f, 30f);
            var secondProj = PuzzleSceneTestHelper.SpawnProjectile(projectileData, AITestPositionID.PROJECTILE_TARGET_2, launchProjectileContainerManager);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame(); //wait for destination position to update
            var aiDestination = mouseTestAIManager.GetAgent().destination;
            var targetZoneCollider = PuzzleSceneTestHelper.FindTargetZone(TargetZoneID.TEST_TARGET_ZONE).TargetZoneTriggerType.TargetZoneTriggerCollider;
            Assert.IsTrue(targetZoneCollider.bounds.Contains(aiDestination), "AI Destination should contains the target zone the second hit.");
            Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectileIngnoringTargetZones(), "The AI should ignore target zones while escaping from projectile for the second time.");
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_FirstTimeNotIntoTargetZone()
        {
            yield return this.AI_ProjectileReceived_FirstTimeNotIntoTargetZone(SceneConstants.OneAIForcedTargetZone, AiID.MOUSE_TEST);
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_FirstTimeIntoTargetZoneDistanceCheck()
        {
            yield return this.Before(SceneConstants.OneAIForcedTargetZone);
            var launchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return null;
            var projectileData = PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 90f, 30f);
            var firstProj = PuzzleSceneTestHelper.SpawnProjectile(projectileData, AITestPositionID.PROJECTILE_TARGET_1, launchProjectileContainerManager);
            firstProj.transform.position = mouseTestAIManager.transform.position + new Vector3(-0.1f, 0, 0);
            yield return new WaitForFixedUpdate(); //projectile processing
            TestHelperMethods.SetAgentDestinationPositionReached(mouseTestAIManager.GetAgent());
            Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone(), "AI should not escape from target zone when a projectile is triggered.");
            Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectile(), "AI should escape form projectile when a projectile is triggered.");
            Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileIngnoringTargetZones(), "AI should escape while ignoring target zones when a projectile is triggered for the first time.");
            yield return new WaitForFixedUpdate();
            Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone(), "AI should escape from target zone when target zone distance check is triggered.");
            Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectile(), "AI should not escape form projectile because target zone is cancelling it.");
            Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileIngnoringTargetZones());
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_SecondTimeInTargetZone()
        {
            yield return this.AI_ProjectileReceived_SecondTimeInTargetZone(SceneConstants.OneAIForcedTargetZone, null);
        }


        [UnityTest]
        public IEnumerator AI_ProjectileReceived_InterruptedBy_TargetZone_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return PuzzleSceneTestHelper.ProjectileYield(PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 170f, 30f), mouseTestAIManager.transform.position,
                 OnProjectileSpawn: (LaunchProjectile lauinchProjectile) =>
                 {
                     Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectile());
                     Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone());
                     return PuzzleSceneTestHelper.TargetZoneYield(new TargetZoneInherentData(0.1f, 100, 170), mouseTestAIManager.transform.position,
                         OnTargetZoneSpawn: (TargetZone targetZone) =>
                         {
                             Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectile());
                             Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone());
                             return null;
                         },
                         OnDistanceReached: () =>
                         {
                             Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectile());
                             Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone());
                             return null;
                         }
                     );
                 },
                 OnDistanceReached: null
                 );
        }

        [UnityTest]
        public IEnumerator AI_ProjectileRecevied_NotInterruptedBy_Attractive_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return PuzzleSceneTestHelper.ProjectileYield(PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 170f, 30f), mouseTestAIManager.transform.position,
                OnProjectileSpawn: (LaunchProjectile lauinchProjectile) =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectile());
                    Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                    return PuzzleSceneTestHelper.AttractiveObjectYield(PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(999999f, 0.2f), mouseTestAIManager.transform.position,
                        OnAttractiveObjectSpawn: (AttractiveObjectType attractiveObjectType) =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectile());
                            Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                            return null;
                        },
                        OnAttractiveObjectDestroyed: () =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectile());
                            Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                            return null;
                        });
                },
                OnDistanceReached: () =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectile());
                    Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                    return null;
                });
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_Nominal_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            float attractiveObjectEffectiveTime = .1f;
            var attractiveObjectInherentConfigurationData = PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(999999f, attractiveObjectEffectiveTime);
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return null;
            var attractiveObjectType = PuzzleSceneTestHelper.SpawnAttractiveObject(attractiveObjectInherentConfigurationData, AITestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(mouseAIBheavior.IsPatrolling(), "The AI has been attracted, no more patrolling.");
            Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject(), "The AI is being attracted.");
            yield return new WaitForSeconds(attractiveObjectEffectiveTime);
            Assert.IsNull(attractiveObjectType);
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_InterruptedBy_Projectile_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return PuzzleSceneTestHelper.AttractiveObjectYield(PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(999999f, 0.2f), mouseTestAIManager.transform.position,
                OnAttractiveObjectSpawn: (AttractiveObjectType attractiveObject) =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectile());
                    return PuzzleSceneTestHelper.ProjectileYield(PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 170f, 30f), mouseTestAIManager.transform.position,
                        OnProjectileSpawn: (LaunchProjectile projectile) =>
                        {
                            Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                            Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectile());
                            return null;
                        },
                        OnDistanceReached: () =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject());
                            Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectile());
                            return null;
                        }
                   );
                },
                OnAttractiveObjectDestroyed: () =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectile());
                    return null;
                }
                );
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_InterruptedBy_TargetZone_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return PuzzleSceneTestHelper.AttractiveObjectYield(PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(999999f, 0.2f), mouseTestAIManager.transform.position,
               OnAttractiveObjectSpawn: (AttractiveObjectType attractiveObject) =>
               {
                   Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject());
                   Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone());
                   return PuzzleSceneTestHelper.TargetZoneYield(new TargetZoneInherentData(0.1f, 1, 170), mouseTestAIManager.transform.position,
                       OnTargetZoneSpawn: (TargetZone targetZone) =>
                       {
                           Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                           Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone());
                           return null;
                       },
                       OnDistanceReached: () =>
                       {
                           Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject());
                           Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone());
                           return null;
                       }
                       );
               },
               OnAttractiveObjectDestroyed: () =>
               {
                   Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                   Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone());
                   return null;
               }
               );
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_InterruptedBy_Fear_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            var fearTime = 0.05f;
            ((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents).AIFearStunComponent.TimeWhileBeginFeared = fearTime;
            yield return PuzzleSceneTestHelper.AttractiveObjectYield(PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(999999f, 0.2f), mouseTestAIManager.transform.position,
             OnAttractiveObjectSpawn: (AttractiveObjectType attractiveObject) =>
             {
                 Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject());
                 Assert.IsFalse(mouseAIBheavior.IsFeared());

                 return PuzzleSceneTestHelper.FearYield(mouseTestAIManager.transform.position,
                     OnFearTriggered: () =>
                     {
                         Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                         Assert.IsTrue(mouseAIBheavior.IsFeared());
                         return null;
                     },
                     OnFearEnded: () =>
                     {
                         Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject());
                         Assert.IsFalse(mouseAIBheavior.IsFeared());
                         return null;
                     },
                     fearTime
                 );
             },
             OnAttractiveObjectDestroyed: null
             );
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_WithSmallRange_NoInfluence_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            var attractiveObjectInherentConfigurationData = PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(0.001f, 99f);
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return null;
            var attractiveObjectType = PuzzleSceneTestHelper.SpawnAttractiveObject(attractiveObjectInherentConfigurationData, AITestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject(), "The AI is too far from attractive object zone.");
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_WhenEnterInRange_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            var attractiveObjectInherentConfigurationData = PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(0.1f, 99f);
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return null;
            var attractiveObjectType = PuzzleSceneTestHelper.SpawnAttractiveObject(attractiveObjectInherentConfigurationData, AITestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject(), "The AI is too far from attractive object zone.");
            attractiveObjectType.transform.position = mouseTestAIManager.transform.position;
            yield return new WaitForFixedUpdate();
            Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject(), "The AI has entered the attractive object zone.");
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_WhenDestinationReached_AndObjectStillUp_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            var attractiveObjectInherentConfigurationData = PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(999999f, 99f);
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return null;
            var attractiveObjectType = PuzzleSceneTestHelper.SpawnAttractiveObject(attractiveObjectInherentConfigurationData, AITestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
            mouseTestAIManager.GetAgent().Warp(mouseTestAIManager.GetAgent().destination);
            yield return null;
            yield return new WaitForEndOfFrame();
            Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject(), "The AI must still be attracted by object at the end of frame.");
            Assert.AreEqual(attractiveObjectType.transform.position.x, mouseTestAIManager.GetAgent().destination.x, "The AI must have the target position to the attractive object.");
            Assert.AreEqual(attractiveObjectType.transform.position.z, mouseTestAIManager.GetAgent().destination.z, "The AI must have the target position to the attractive object.");
        }

        #region AI_ProjectileReceived_WhenDestinationReached_WhenThereIsStillEscapeDistanceToTravel

        private IEnumerator SetupProjectileReceived_ThenDestinationReached_WhenThereIsStillEscapeDistanceToTravel(string sceneId)
        {
            yield return this.Before(sceneId);
            var launchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            //high projectile escape distance
            ((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents).AIProjectileEscapeWithCollisionComponent.EscapeDistance = 9999f;
            yield return null;
            Assert.IsTrue(mouseAIBheavior.IsPatrolling(), "The AI has no interaction -> Patrolling.");
            //wide angle to avoid navmesh ray cast to hit the same point.
            var projectileData = PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 180f, 30f);
            var lpTest = PuzzleSceneTestHelper.SpawnProjectile(projectileData, mouseTestAIManager.transform.position, launchProjectileContainerManager);
            yield return new WaitForFixedUpdate();
            //projectile taken into account
            yield return new WaitForEndOfFrame();
            //move agent to a near position
            var agent = mouseTestAIManager.GetAgent();
            TestHelperMethods.SetAgentDestinationPositionReached(agent);
            Debug.Log("Agent position manually updated.");
            yield return null; //trigger SetDestination();
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_WhenDestinationReached_WhenThereIsStillEscapeDistanceToTravel_StillEscape_Test()
        {
            yield return this.SetupProjectileReceived_ThenDestinationReached_WhenThereIsStillEscapeDistanceToTravel(SceneConstants.OneAINoTargetZone);
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectile(), "The AI should still escape from projectile when destination has been reached one time but there is still distance to travel.");
        }
        [UnityTest]
        public IEnumerator AI_ProjectileReceived_WhenDestinationReached_WhenThereIsStillEscapeDistanceToTravel_NoInteruptionByAttractive_Test()
        {
            yield return this.SetupProjectileReceived_ThenDestinationReached_WhenThereIsStillEscapeDistanceToTravel(SceneConstants.OneAINoTargetZone);
            var attractiveObjectInherentConfigurationData = PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(999999f, 99f);
            var attractiveObjectType = PuzzleSceneTestHelper.SpawnAttractiveObject(attractiveObjectInherentConfigurationData, AITestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
            yield return new WaitForFixedUpdate();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectile(), "The AI should still escape from projectile when destination has been reached one time but there is still distance to travel. And an attractive object has spawn in range.");
            Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject(), "");
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_SecondTimeInTargetZone_WhenDestinationReached_WhenThereIsStillEscapeDistanceToTravel_StillEscape_Test()
        {
            yield return this.AI_ProjectileReceived_SecondTimeInTargetZone(SceneConstants.OneAIForcedTargetZone, 9999f);
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            var launchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            TestHelperMethods.SetAgentDestinationPositionReached(mouseTestAIManager.GetAgent());
            Debug.Log("Destination before AI update : " + mouseTestAIManager.GetAgent().destination);
            yield return null;
            Debug.Log("Destination after AI update : " + mouseTestAIManager.GetAgent().destination);
            Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectileIngnoringTargetZones(), "The AI should ignore target zones while escaping from projectile for the second time.");
            //second projectile hit when destination has been reached one time but there is still distance
            var projectileData = PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 180f, 30f);
            PuzzleSceneTestHelper.SpawnProjectile(projectileData, AITestPositionID.PROJECTILE_TARGET_2, launchProjectileContainerManager);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame(); //wait for destination position to update
            var targetZoneCollider = PuzzleSceneTestHelper.FindTargetZone(TargetZoneID.TEST_TARGET_ZONE).TargetZoneTriggerType.TargetZoneTriggerCollider;
            Assert.IsTrue(targetZoneCollider.bounds.Contains(mouseTestAIManager.GetAgent().destination), "AI Destination should contains the target zone the second (or more) hit.");
            Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectileIngnoringTargetZones(), "The AI should ignore target zones while escaping from projectile for the second (or more) time.");
        }
        #endregion

        [UnityTest]
        public IEnumerator AI_TargetZone_WhenAIIsNearTargetZone_AiFOVShoudlBeReducedBasedOnTargetZoneBoundCenter_Test()
        {
            yield return this.Before(SceneConstants.OneAIForcedTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            var targetZoneCollider = PuzzleSceneTestHelper.FindTargetZone(TargetZoneID.TEST_TARGET_ZONE).TargetZoneTriggerType.TargetZoneTriggerCollider;
            var gameConfiguration = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var targetEscapeSemiAngle = gameConfiguration.TargetZonesConfiguration()[TargetZoneID.TEST_TARGET_ZONE].EscapeFOVSemiAngle;
            var aiPosition = targetZoneCollider.transform.position + new Vector3(0, 0, 0.1f);
            TestHelperMethods.SetAgentDestinationPositionReached(mouseTestAIManager.GetAgent(), aiPosition);
            yield return null;
            var aiFov = mouseAIBheavior.GetFOV();
            Assert.AreEqual(new FOVSlice(360f - targetEscapeSemiAngle, 360f), aiFov.FovSlices[0]);
            Assert.AreEqual(new FOVSlice(0f, targetEscapeSemiAngle), aiFov.FovSlices[1]);
        }

        [UnityTest]
        public IEnumerator AI_TargetZone_WhenAIExitTargetZone_WhenThereIsStillEscapeDistanceToTravel_StillEscape()
        {
            yield return this.Before(SceneConstants.OneAIForcedHighDistanceTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            var targetZoneCollider = PuzzleSceneTestHelper.FindTargetZone(TargetZoneID.TEST_TARGET_ZONE).TargetZoneTriggerType.TargetZoneTriggerCollider;
            var gameConfiguration = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var targetEscapeSemiAngle = gameConfiguration.TargetZonesConfiguration()[TargetZoneID.TEST_TARGET_ZONE].EscapeFOVSemiAngle;
            var aiPosition = targetZoneCollider.transform.position + new Vector3(-0.1f, 0f, 0f);
            TestHelperMethods.SetAgentDestinationPositionReached(mouseTestAIManager.GetAgent(), aiPosition);
            yield return null; //AI is inside the target zone
            TestHelperMethods.SetAgentDestinationPositionReached(mouseTestAIManager.GetAgent(), PuzzleSceneTestHelper.FindAITestPosition(AITestPositionID.FAR_AWAY_POSITION_1).position);
            yield return null;
            Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone(), "AI should still escape as there are distance to cross.");
            Assert.AreNotEqual(new FOVSlice(0f, 360f), mouseAIBheavior.GetFOV().FovSlices[0], "AI fov must not be resetted after destination reached and still distance to cross.");
            TestHelperMethods.SetAgentDestinationPositionReached(mouseTestAIManager.GetAgent());
            yield return null;
            Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone(), "AI should stop escape when destination is reached.");
            Assert.AreEqual(new FOVSlice(0f, 360f), mouseAIBheavior.GetFOV().FovSlices[0]);
        }

        [UnityTest]
        public IEnumerator AI_TargetZone_InterruptedBy_Projectile()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return PuzzleSceneTestHelper.TargetZoneYield(new TargetZoneInherentData(9999, 2, 170), mouseTestAIManager.transform.position,
                OnTargetZoneSpawn: (TargetZone targetZone) =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectile());
                    return PuzzleSceneTestHelper.ProjectileYield(PuzzleSceneTestHelper.CreateProjectileInherentData(1000f, 170, 1), mouseTestAIManager.transform.position,
                        OnProjectileSpawn: (LaunchProjectile projectile) =>
                        {
                            Assert.AreEqual(1, this.MockPuzzleEventsManagerTest.AiHittedByProjectileCallCount);
                            Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone());
                            Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectile());
                            return null;
                        },
                        OnDistanceReached: () =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone());
                            Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectile());
                            return null;
                        });
                },
                OnDistanceReached: null);
        }

        // TODO -> this test is a failure because external events of AI behavior are not synched. This means that 
        // Trigger enter / stay / exit events must not be processed at the moment when they are occuring.
        // We have to store them, then consume at "yield new WaitForEndOfFrame()" at the right order.
        [UnityTest]
        public IEnumerator AI_TargetZone_InterruptedBy_ProjectileIgnoreTarget()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();

            yield return PuzzleSceneTestHelper.TargetZoneYield(new TargetZoneInherentData(9999, 100, 170), mouseTestAIManager.transform.position,
                OnTargetZoneSpawn: (TargetZone targetZone) =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileIngnoringTargetZones());
                    return PuzzleSceneTestHelper.ProjectileIngoreTargetYield(PuzzleSceneTestHelper.CreateProjectileInherentData(1000, 170, 1), mouseTestAIManager.transform.position,
                        OnBeforeSecondProjectileSpawn: () =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone());
                            Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileIngnoringTargetZones());
                            return null;
                        },
                        OnSecondProjectileSpawned: (LaunchProjectile projectile) =>
                        {
                            Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone());
                            Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectileIngnoringTargetZones());
                            return null;
                        },
                        OnSecondProjectileDistanceReached: null);
                },
                OnDistanceReached: null
            );
        }

        [UnityTest]
        public IEnumerator AI_TargetZone_NotInterruptedBy_Attractive()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return PuzzleSceneTestHelper.TargetZoneYield(new TargetZoneInherentData(1, 2, 170), mouseTestAIManager.transform.position,
                OnTargetZoneSpawn: (TargetZone targetZone) =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone());
                    Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                    return PuzzleSceneTestHelper.AttractiveObjectYield(PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(300, 0.15f), mouseTestAIManager.transform.position,
                        OnAttractiveObjectSpawn: (AttractiveObjectType attractiveObject) =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone());
                            Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                            return null;
                        },
                        OnAttractiveObjectDestroyed: null);
                },
                OnDistanceReached: () =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone());
                    Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject());
                    return null;
                });
        }

        [UnityTest]
        public IEnumerator AI_FearStun_WhenReceivingAProjectile_IsFeared()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);

            var launchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            var fearTime = 0.05f;
            ((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents).AIFearStunComponent.TimeWhileBeginFeared = fearTime;

            var projectileData = PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 0.5f, 30f);
            var projectileSpanwed = PuzzleSceneTestHelper.SpawnProjectile(projectileData, mouseTestAIManager.transform.position, launchProjectileContainerManager);

            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
            Assert.IsTrue(mouseAIBheavior.IsFeared());
            yield return new WaitForSeconds(fearTime);
            Assert.IsFalse(mouseAIBheavior.IsFeared());
        }
    }


}
