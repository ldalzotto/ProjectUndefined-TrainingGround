using GameConfigurationID;
using RTPuzzle;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Tests
{
    public class AIBehaviorTest : AbstractPuzzleSceneTest
    {

        private const int PITFALL_AI_TO_FEAR_FRAME_PRECISION = 20;

        private EscapeWhileIgnoringTargetZoneTracker GetEscapeWhileIgnoringTargetZoneTracker(GenericPuzzleAIBehavior genericPuzzleAIBehavior)
        {
            return genericPuzzleAIBehavior.PuzzleAIBehaviorExternalEventManager.GetBehaviorStateTrackerContainer().GetBehavior<EscapeWhileIgnoringTargetZoneTracker>();
        }

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
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseAIBheavior.AIComponents, 175f);

            Assert.IsTrue(mouseAIBheavior.IsPatrolling());
            Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
            yield return PuzzleSceneTestHelper.ProjectileYield(PuzzleSceneTestHelper.CreateProjectileInherentData(1000, 0.1f), mouseTestAIManager.transform.position,
                OnProjectileSpawn: (LaunchProjectile projectile) =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsPatrolling());
                    Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                    Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);
                    return null;
                },
                OnDistanceReached: () =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsPatrolling());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                    Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
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
            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
            yield return PuzzleSceneTestHelper.TargetZoneYield(new TargetZoneInherentData(1, 170), mouseTestAIManager.transform.position,
                OnTargetZoneSpawn: (TargetZone targetZone) =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsPatrolling());
                    Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone());
                    Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);
                    return null;
                },
                OnDistanceReached: () =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsPatrolling());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone());
                    Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
                    return null;
                }
                );
        }

        [UnityTest]
        public IEnumerator AI_RandomPatrol_InterruptedBy_EscapeIgnoreTarget()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseAIBheavior.AIComponents, 170f);

            var currentFOVSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
            Assert.IsTrue(currentFOVSum == 360f);
            yield return PuzzleSceneTestHelper.ProjectileIngoreTargetYield(PuzzleSceneTestHelper.CreateProjectileInherentData(3, 1), mouseTestAIManager.transform.position,
                OnBeforeSecondProjectileSpawn: () =>
                {
                    currentFOVSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
                    Assert.IsTrue(currentFOVSum < 360f);
                    return null;
                },
                OnSecondProjectileSpawned: (LaunchProjectile projectile) =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsPatrolling());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                    Assert.IsTrue(mouseAIBheavior.IsEscapingWithoutTarget());
                    Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < currentFOVSum);
                    currentFOVSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
                    return null;
                },
                OnSecondProjectileDistanceReached: () =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsPatrolling());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingWithoutTarget());
                    Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
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
                fearTime: fearTime,
                mouseAIBheavior
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
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseAIBheavior.AIComponents, 90f);
            // (1) - Initialization process
            yield return null;
            Assert.IsTrue(mouseAIBheavior.IsPatrolling(), "The AI has no interaction -> Patrolling.");
            Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
            var projectileData = PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 30f);
            var lpTest = PuzzleSceneTestHelper.SpawnProjectile(projectileData, mouseTestAIManager.transform.position, launchProjectileContainerManager);
            // (2) - Wait for projectile to be processed by physics engine. The AI must be in escape from projectile state.
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(mouseAIBheavior.IsPatrolling(), "The AI has been hit, no more patrolling.");
            Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones(), "The AI has been hit, escaping from projectile while taking into account target zones.");
            Assert.IsFalse(mouseAIBheavior.IsEscapingWithoutTarget(), "The AI should not ignore target zones while escaping from projectile for the first time.");
            Assert.AreEqual(this.MockPuzzleEventsManagerTest.AiHittedByProjectileCallCount, 1, "The 'hitted by projectile event' must be triggered only once.");
            Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);
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
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseAIBheavior.AIComponents, 90f);
            yield return null;
            Assert.IsTrue(mouseAIBheavior.IsPatrolling(), "The AI has no interaction -> Patrolling.");
            var projectileData = PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 30f);
            var lpTest = PuzzleSceneTestHelper.SpawnProjectile(projectileData, mouseTestAIManager.transform.position, launchProjectileContainerManager);
            yield return new WaitForFixedUpdate();
            //projectile taken into account
            yield return new WaitForEndOfFrame();
            //clear agent destinations
            var agent = mouseTestAIManager.GetAgent();
            TestHelperMethods.SetAgentDestinationPositionReached(agent);
            // wait for set destination
            yield return null;
            Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones(), "The AI should no more escape from projectile when destination is reached.");
            Assert.IsFalse(mouseAIBheavior.IsEscapingWithoutTarget(), "The AI should no more escape from projectile when destination is reached.");
            Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
        }

        private IEnumerator AI_ProjectileReceived_FirstTimeNotIntoTargetZone(string sceneName, AiID aiID)
        {
            yield return this.Before(sceneName, aiID);
            var launchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(aiID);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseAIBheavior.AIComponents, 90f);
            yield return null;
            var projectileData = PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 30f);
            Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
            var firstProj = PuzzleSceneTestHelper.SpawnProjectile(projectileData, TestPositionID.PROJECTILE_TARGET_1, launchProjectileContainerManager);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame(); //wait for destination position to update
            var aiDestination = mouseTestAIManager.GetAgent().destination;
            var targetZoneCollider = PuzzleSceneTestHelper.FindTargetZone(TargetZoneID.TEST_TARGET_ZONE).TargetZoneTriggerType.GetTargetZoneTriggerCollider();
            Assert.IsFalse(targetZoneCollider.bounds.Contains(aiDestination), "AI Destination should not be in the target zone the first hit.");
            Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
        }

        private IEnumerator AI_ProjectileReceived_SecondTime_EscapeWithoutTarget_DestinationInTargetZone(string sceneName, float? projectileEscapeDistance)
        {
            yield return this.AI_ProjectileReceived_FirstTimeNotIntoTargetZone(sceneName, AiID.MOUSE_TEST);
            var launchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseAIBheavior.AIComponents, 90f);
            if (projectileEscapeDistance != null)
            {
                ((GenericPuzzleAIComponents)mouseTestAIManager.GetAIBehavior().AIComponents).AIProjectileEscapeWithCollisionComponent.EscapeDistanceV2.Values[LaunchProjectileId.TEST] = projectileEscapeDistance.Value;
            }
            var projectileData = PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 30f);
            var secondProj = PuzzleSceneTestHelper.SpawnProjectile(projectileData, TestPositionID.PROJECTILE_TARGET_2, launchProjectileContainerManager);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame(); //wait for destination position to update
            var aiDestination = mouseTestAIManager.GetAgent().destination;
            var targetZoneCollider = PuzzleSceneTestHelper.FindTargetZone(TargetZoneID.TEST_TARGET_ZONE).TargetZoneTriggerType.GetTargetZoneTriggerCollider();
            Assert.IsTrue(targetZoneCollider.bounds.Contains(aiDestination), "AI Destination should contains the target zone the second hit.");
            Assert.IsTrue(mouseAIBheavior.IsEscapingWithoutTarget(), "The AI should ignore target zones while escaping from projectile for the second time.");
            Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones(), "The AI should not escape with target zone consideration.");
            Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
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
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseAIBheavior.AIComponents, 110f);
            yield return null;
            var projectileData = PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 30f);
            var firstProj = PuzzleSceneTestHelper.SpawnProjectile(projectileData, TestPositionID.PROJECTILE_TARGET_1, launchProjectileContainerManager);
            var currentFOVSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
            firstProj.transform.position = mouseTestAIManager.transform.position + new Vector3(-0.1f, 0, 0);
            Assert.IsTrue(currentFOVSum == 360f);
            yield return new WaitForFixedUpdate(); //projectile processing
            currentFOVSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
            TestHelperMethods.SetAgentDestinationPositionReached(mouseTestAIManager.GetAgent());
            Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone(), "AI should not escape from target zone when a projectile is triggered.");
            Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones(), "AI should escape form projectile when a projectile is triggered.");
            Assert.IsFalse(mouseAIBheavior.IsEscapingWithoutTarget(), "AI should escape while ignoring target zones when a projectile is triggered for the first time.");
            Assert.IsTrue(currentFOVSum < 360f);
            yield return new WaitForFixedUpdate();
            Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone(), "AI should escape from target zone when target zone distance check is triggered.");
            Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones(), "AI should not escape form projectile because target zone is cancelling it.");
            Assert.IsFalse(mouseAIBheavior.IsEscapingWithoutTarget());
            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < currentFOVSum);
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_WhenThereIsNoAvailableDestination_BeFeared()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            mouseTestAIManager.GetAgent().Warp(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.PITFAL_Z_POSITION_1).transform.position);
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseAIBheavior.AIComponents, 20f);
            yield return null;
            Assert.IsFalse(mouseAIBheavior.IsFeared());
            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
            yield return PuzzleSceneTestHelper.ProjectileYield(PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 30f), mouseTestAIManager.transform.position - Vector3.forward,
                OnProjectileSpawn: (LaunchProjectile lauinchProjectile) =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsFeared());
                    Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);
                    return null;
                },
                OnDistanceReached: null
                );
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_InterruptedBy_TargetZone_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseAIBheavior.AIComponents, 170f);
            var currentFOVSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
            Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
            yield return PuzzleSceneTestHelper.ProjectileYield(PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 30f), mouseTestAIManager.transform.position,
                 OnProjectileSpawn: (LaunchProjectile lauinchProjectile) =>
                 {
                     Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                     Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone());
                     Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                     Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < currentFOVSum);
                     currentFOVSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
                     return PuzzleSceneTestHelper.TargetZoneYield(new TargetZoneInherentData(0.1f, 170), mouseTestAIManager.transform.position,
                         OnTargetZoneSpawn: (TargetZone targetZone) =>
                         {
                             Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                             Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone());
                             Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                             Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < currentFOVSum);
                             currentFOVSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
                             return null;
                         },
                         OnDistanceReached: () =>
                         {
                             Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                             Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone());
                             Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                             Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360);
                             return null;
                         }
                     );
                 },
                 OnDistanceReached: null
                 );
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_InterruptedBy_EscapeWithoutTarget()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseAIBheavior.AIComponents, 170f);
            var currentFOVSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();

            Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
            yield return PuzzleSceneTestHelper.ProjectileYield(PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 30f), mouseTestAIManager.transform.position,
                 OnProjectileSpawn: (LaunchProjectile lauinchProjectile) =>
                 {
                     Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                     Assert.IsFalse(mouseAIBheavior.IsEscapingWithoutTarget());
                     Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < currentFOVSum);
                     currentFOVSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
                     return PuzzleSceneTestHelper.ProjectileYield(PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 30f), mouseTestAIManager.transform.position,
                         OnProjectileSpawn: (LaunchProjectile launchProjectile) =>
                         {
                             Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                             Assert.IsTrue(mouseAIBheavior.IsEscapingWithoutTarget());
                             Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < currentFOVSum);
                             return null;
                         },
                         OnDistanceReached: () =>
                         {
                             Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                             Assert.IsFalse(mouseAIBheavior.IsEscapingWithoutTarget());
                             Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360);
                             return null;
                         });
                 },
                 OnDistanceReached: null
                 );
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_InterruptedBy_PlayerEscape()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseAIBheavior.AIComponents, 170f);
            var currentFOVSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();

            Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
            yield return PuzzleSceneTestHelper.ProjectileYield(PuzzleSceneTestHelper.CreateProjectileInherentData(9999f, 30f), mouseTestAIManager.transform.position,
                OnProjectileSpawn: (LaunchProjectile launchProjectile) =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromPlayer());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingWithoutTarget());
                    Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                    Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);

                    currentFOVSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
                    PuzzleSceneTestHelper.SetPlayerEscapeComponentValues(((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents), 10f, 90f, 1f);
                    return PuzzleSceneTestHelper.EscapeFromPlayerYield(playerManager, mouseTestAIManager,
                        OnBeforeSettingPosition: null,
                        OnSamePositionSetted: () =>
                        {
                            Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                            Assert.IsFalse(mouseAIBheavior.IsEscapingFromPlayer());
                            Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                            Assert.IsTrue(mouseAIBheavior.IsEscapingWithoutTarget());
                            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < currentFOVSum);
                            return null;
                        },
                        OnDestinationReached: () =>
                        {
                            Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                            Assert.IsFalse(mouseAIBheavior.IsEscapingFromPlayer());
                            Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                            Assert.IsFalse(mouseAIBheavior.IsEscapingWithoutTarget());
                            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360);
                            return null;
                        });
                },
                OnDistanceReached: null);
        }

        [UnityTest]
        public IEnumerator AI_ProjectileRecevied_NotInterruptedBy_Attractive_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseAIBheavior.AIComponents, 170f);

            Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);

            yield return PuzzleSceneTestHelper.ProjectileYield(PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 30f), mouseTestAIManager.transform.position,
                OnProjectileSpawn: (LaunchProjectile lauinchProjectile) =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                    Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                    Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                    Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);
                    return PuzzleSceneTestHelper.AttractiveObjectYield(PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(999999f, 0.2f), mouseTestAIManager.transform.position,
                        OnAttractiveObjectSpawn: (AttractiveObjectType attractiveObjectType) =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                            Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                            Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);
                            return null;
                        },
                        OnAttractiveObjectDestroyed: () =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                            Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                            Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);
                            return null;
                        });
                },
                OnDistanceReached: () =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                    Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                    Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                    Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
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
            var attractiveObjectType = PuzzleSceneTestHelper.SpawnAttractiveObject(attractiveObjectInherentConfigurationData, TestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(mouseAIBheavior.IsPatrolling(), "The AI has been attracted, no more patrolling.");
            Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject(), "The AI is being attracted.");
            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
            yield return new WaitForSeconds(attractiveObjectEffectiveTime);
            Assert.IsNull(attractiveObjectType);
        }

        //Ai is following attractive object position
        [UnityTest]
        public IEnumerator AI_AttractiveObject_OnStay_AIFollowPosition()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            var attractiveObjectType = PuzzleSceneTestHelper.SpawnAttractiveObject(PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(999999f, 1f), mouseTestAIManager.transform.position);
            yield return new WaitForFixedUpdate();
            yield return null; //Wait for AI to set the destination
            Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject());
            Assert.AreEqual(attractiveObjectType.transform.position, mouseTestAIManager.GetAgent().destination);
            attractiveObjectType.transform.position = attractiveObjectType.transform.position + Vector3.forward;
            yield return new WaitForFixedUpdate();
            yield return null; //Wait for AI to set the destination
            Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject());
            Assert.AreEqual(attractiveObjectType.transform.position, mouseTestAIManager.GetAgent().destination);
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_InterruptedBy_Projectile_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseAIBheavior.AIComponents, 170f);
            yield return PuzzleSceneTestHelper.AttractiveObjectYield(PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(999999f, 0.2f), mouseTestAIManager.transform.position,
                OnAttractiveObjectSpawn: (AttractiveObjectType attractiveObject) =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingWithoutTarget());
                    Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
                    return PuzzleSceneTestHelper.ProjectileYield(PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 30f), mouseTestAIManager.transform.position,
                        OnProjectileSpawn: (LaunchProjectile projectile) =>
                        {
                            Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                            Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                            Assert.IsFalse(mouseAIBheavior.IsEscapingWithoutTarget());
                            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);
                            return null;
                        },
                        OnDistanceReached: () =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject(), "The attractive object is stille living and in range. AttractiveObject_TriggerStay -> attracted = true.");
                            Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                            Assert.IsFalse(mouseAIBheavior.IsEscapingWithoutTarget());
                            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
                            return null;
                        }
                   );
                },
                OnAttractiveObjectDestroyed: () =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingWithoutTarget());
                    Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
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
                   Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
                   return PuzzleSceneTestHelper.TargetZoneYield(new TargetZoneInherentData(0.1f, 170), mouseTestAIManager.transform.position,
                       OnTargetZoneSpawn: (TargetZone targetZone) =>
                       {
                           Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                           Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone());
                           Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);
                           return null;
                       },
                       OnDistanceReached: () =>
                       {
                           Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject());
                           Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone());
                           Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
                           return null;
                       }
                       );
               },
               OnAttractiveObjectDestroyed: () =>
               {
                   Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                   Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone());
                   Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
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
                 Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);

                 return PuzzleSceneTestHelper.FearYield(mouseTestAIManager.transform.position,
                     OnFearTriggered: () =>
                     {
                         Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                         Assert.IsTrue(mouseAIBheavior.IsFeared());
                         Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);
                         return null;
                     },
                     OnFearEnded: () =>
                     {
                         Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject());
                         Assert.IsFalse(mouseAIBheavior.IsFeared());
                         Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
                         return null;
                     },
                     fearTime,
                     mouseAIBheavior
                 );
             },
             OnAttractiveObjectDestroyed: null
             );
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_InterruptedBy_PlayerEscape()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();

            Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
            yield return PuzzleSceneTestHelper.AttractiveObjectYield(PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(999999f, 1f), mouseTestAIManager.transform.position,
            OnAttractiveObjectSpawn: (AttractiveObjectType attractiveObject) =>
            {
                Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject());
                Assert.IsFalse(mouseAIBheavior.IsEscapingFromPlayer());
                Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);

                PuzzleSceneTestHelper.SetPlayerEscapeComponentValues(((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents), 10f, 90f, 1f);

                return PuzzleSceneTestHelper.EscapeFromPlayerYield(playerManager, mouseTestAIManager,
                    OnBeforeSettingPosition: null,
                    OnSamePositionSetted: () =>
                    {
                        Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                        Assert.IsTrue(mouseAIBheavior.IsEscapingFromPlayer());
                        Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                        Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);
                        PuzzleSceneTestHelper.SetPlayerEscapeComponentValues(((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents), 0f, 0f, 1f);
                        return null;
                    },
                    OnDestinationReached: () =>
                    {
                        //  yield return new WaitForFixedUpdate();
                        Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject());
                        Assert.IsFalse(mouseAIBheavior.IsEscapingFromPlayer());
                        Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                        Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
                        return null;
                    }
                    );
            },
            OnAttractiveObjectDestroyed: null
            );
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_WhenAIGetsOutOfRangeToReachIt_ThenStillAttracted_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            float attractiveRange = 10f;
            yield return PuzzleSceneTestHelper.AttractiveObjectYield(PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(attractiveRange, 0.2f), mouseTestAIManager.transform.position,
                OnAttractiveObjectSpawn: (AttractiveObjectType attractiveObjectType) =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject());
                    TestHelperMethods.SetAgentPosition(mouseTestAIManager.GetAgent(), mouseTestAIManager.transform.position + new Vector3(attractiveRange * 2, 0f, 0f));
                    return null;
                },
                OnAttractiveObjectDestroyed: null);
            Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject());
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_WithSmallRange_NoInfluence_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            var attractiveObjectInherentConfigurationData = PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(0.001f, 99f);
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return null;
            var attractiveObjectType = PuzzleSceneTestHelper.SpawnAttractiveObject(attractiveObjectInherentConfigurationData, TestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject(), "The AI is too far from attractive object zone.");
            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_WhenEnterInRange_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            var attractiveObjectInherentConfigurationData = PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(0.1f, 99f);
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return null;
            var attractiveObjectType = PuzzleSceneTestHelper.SpawnAttractiveObject(attractiveObjectInherentConfigurationData, TestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject(), "The AI is too far from attractive object zone.");
            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
            yield return new WaitForEndOfFrame(); //We wait to the end of frame in order to not take into account the natural movement of AI
            attractiveObjectType.transform.position = mouseTestAIManager.transform.position;
            yield return new WaitForFixedUpdate();
            Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject(), "The AI has entered the attractive object zone.");
            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_WhenDestinationReached_AndObjectStillUp_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            var attractiveObjectInherentConfigurationData = PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(999999f, 99f);
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return null;
            var attractiveObjectType = PuzzleSceneTestHelper.SpawnAttractiveObject(attractiveObjectInherentConfigurationData, TestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
            mouseTestAIManager.GetAgent().Warp(mouseTestAIManager.GetAgent().destination);
            yield return null;
            yield return new WaitForEndOfFrame();
            Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject(), "The AI must still be attracted by object at the end of frame.");
            Assert.AreEqual(attractiveObjectType.transform.position.x, mouseTestAIManager.GetAgent().destination.x, "The AI must have the target position to the attractive object.");
            Assert.AreEqual(attractiveObjectType.transform.position.z, mouseTestAIManager.GetAgent().destination.z, "The AI must have the target position to the attractive object.");
            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
        }

        #region AI_ProjectileReceived_WhenDestinationReached_WhenThereIsStillEscapeDistanceToTravel

        private IEnumerator SetupProjectileReceived_ThenDestinationReached_WhenThereIsStillEscapeDistanceToTravel(string sceneId)
        {
            yield return this.Before(sceneId);
            var launchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseAIBheavior.AIComponents, 179.5f);
            //high projectile escape distance
            ((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents).AIProjectileEscapeWithCollisionComponent.EscapeDistanceV2.Values[LaunchProjectileId.TEST] = 9999f;
            yield return null;
            Assert.IsTrue(mouseAIBheavior.IsPatrolling(), "The AI has no interaction -> Patrolling.");
            //wide angle to avoid navmesh ray cast to hit the same point.
            var projectileData = PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 30f);
            var lpTest = PuzzleSceneTestHelper.SpawnProjectile(projectileData, mouseTestAIManager.transform.position, launchProjectileContainerManager);
            yield return new WaitForFixedUpdate();
            //projectile taken into account
            yield return new WaitForEndOfFrame();
            //move agent to a near position
            var agent = mouseTestAIManager.GetAgent();
            TestHelperMethods.SetAgentDestinationPositionReached(agent);
            yield return null; //trigger SetDestination();
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_WhenDestinationReached_WhenThereIsStillEscapeDistanceToTravel_StillEscape_Test()
        {
            yield return this.SetupProjectileReceived_ThenDestinationReached_WhenThereIsStillEscapeDistanceToTravel(SceneConstants.OneAINoTargetZone);
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones(), "The AI should still escape from projectile when destination has been reached one time but there is still distance to travel.");
            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);
        }
        [UnityTest]
        public IEnumerator AI_ProjectileReceived_WhenDestinationReached_WhenThereIsStillEscapeDistanceToTravel_NoInteruptionByAttractive_Test()
        {
            yield return this.SetupProjectileReceived_ThenDestinationReached_WhenThereIsStillEscapeDistanceToTravel(SceneConstants.OneAINoTargetZone);
            var attractiveObjectInherentConfigurationData = PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(999999f, 99f);
            var attractiveObjectType = PuzzleSceneTestHelper.SpawnAttractiveObject(attractiveObjectInherentConfigurationData, TestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
            yield return new WaitForFixedUpdate();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones(), "The AI should still escape from projectile when destination has been reached one time but there is still distance to travel. And an attractive object has spawn in range.");
            Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject(), "");
            Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_SecondTimeInTargetZone_WhenDestinationReached_WhenThereIsStillEscapeDistanceToTravel_StillEscape_Test()
        {
            yield return this.AI_ProjectileReceived_SecondTime_EscapeWithoutTarget_DestinationInTargetZone(SceneConstants.OneAIForcedTargetZone, 9999f);
            Debug.Break();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseAIBheavior.AIComponents, 20f);
            var launchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            TestHelperMethods.SetAgentDestinationPositionReached(mouseTestAIManager.GetAgent());
            Debug.Log("Destination before AI update : " + mouseTestAIManager.GetAgent().destination);
            yield return null;
            Debug.Log("Destination after AI update : " + mouseTestAIManager.GetAgent().destination);
            Assert.IsTrue(mouseAIBheavior.IsEscapingWithoutTarget(), "The AI should ignore target zones while escaping from projectile for the second time.");
            Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);
            //second projectile hit when destination has been reached one time but there is still distance
            var projectileData = PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 30f);
            var currentFOVSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
            Debug.Log(MyLog.Format(mouseAIBheavior.AIFOVManager.GetFOVAngleSum()));
            PuzzleSceneTestHelper.SpawnProjectile(projectileData, TestPositionID.PROJECTILE_TARGET_2, launchProjectileContainerManager);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame(); //wait for destination position to update
            var targetZoneCollider = PuzzleSceneTestHelper.FindTargetZone(TargetZoneID.TEST_TARGET_ZONE).TargetZoneTriggerType.GetTargetZoneTriggerCollider();
            Assert.IsTrue(targetZoneCollider.bounds.Contains(mouseTestAIManager.GetAgent().destination), "AI Destination should contains the target zone the second (or more) hit.");
            Assert.IsTrue(mouseAIBheavior.IsEscapingWithoutTarget(), "The AI should ignore target zones while escaping from projectile for the second (or more) time.");
            Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < currentFOVSum);
        }
        #endregion

        [UnityTest]
        public IEnumerator AI_TargetZone_WhenAIIsNearTargetZone_AiFOVShoudlBeReducedBasedOnTargetZoneBoundCenter_Test()
        {
            yield return this.Before(SceneConstants.OneAIForcedTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            var targetZoneCollider = PuzzleSceneTestHelper.FindTargetZone(TargetZoneID.TEST_TARGET_ZONE).TargetZoneTriggerType.GetTargetZoneTriggerCollider();
            var gameConfiguration = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var targetEscapeSemiAngle = gameConfiguration.TargetZonesConfiguration()[TargetZoneID.TEST_TARGET_ZONE].EscapeFOVSemiAngle;
            var aiPosition = targetZoneCollider.transform.position + new Vector3(0, 0, 0.1f);
            TestHelperMethods.SetAgentDestinationPositionReached(mouseTestAIManager, aiPosition);
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
            var targetZoneCollider = PuzzleSceneTestHelper.FindTargetZone(TargetZoneID.TEST_TARGET_ZONE).TargetZoneTriggerType.GetTargetZoneTriggerCollider();
            var gameConfiguration = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var targetEscapeSemiAngle = gameConfiguration.TargetZonesConfiguration()[TargetZoneID.TEST_TARGET_ZONE].EscapeFOVSemiAngle;
            var aiPosition = targetZoneCollider.transform.position + new Vector3(-0.1f, 0f, 0f);
            TestHelperMethods.SetAgentDestinationPositionReached(mouseTestAIManager, aiPosition);
            yield return null; //AI is inside the target zone
            TestHelperMethods.SetAgentDestinationPositionReached(mouseTestAIManager, PuzzleSceneTestHelper.FindTestPosition(TestPositionID.FAR_AWAY_POSITION_1).position);
            yield return null;
            Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone(), "AI should still escape as there are distance to cross.");
            Assert.AreNotEqual(new FOVSlice(0f, 360f), mouseAIBheavior.GetFOV().FovSlices[0], "AI fov must not be resetted after destination reached and still distance to cross.");
            TestHelperMethods.SetAgentDestinationPositionReached(mouseTestAIManager.GetAgent());
            yield return null;
            Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone(), "AI should stop escape when destination is reached.");
            Assert.AreEqual(new FOVSlice(0f, 360f), mouseAIBheavior.GetFOV().FovSlices[0]);
        }

        [UnityTest]
        public IEnumerator AI_TargetZone_InterruptedBy_Projectile_TargetZoneOnEnterEvent()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseAIBheavior.AIComponents, 170f);
            yield return PuzzleSceneTestHelper.TargetZoneYield(new TargetZoneInherentData(9999, 170), mouseTestAIManager.transform.position,
                OnTargetZoneSpawn: (TargetZone targetZone) =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                    Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);
                    var currentFOVAngleSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
                    //Target zone is destroyed to simulate the fact that the AI is exiting the target zone
                    MonoBehaviour.DestroyImmediate(targetZone.gameObject);
                    return PuzzleSceneTestHelper.ProjectileYield(PuzzleSceneTestHelper.CreateProjectileInherentData(1000f, 1), mouseTestAIManager.transform.position,
                        OnProjectileSpawn: (LaunchProjectile projectile) =>
                        {
                            Assert.AreEqual(1, this.MockPuzzleEventsManagerTest.AiHittedByProjectileCallCount);
                            Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone());
                            Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                            Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < currentFOVAngleSum);
                            return null;
                        },
                        OnDistanceReached: () =>
                        {
                            Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone());
                            Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                            Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
                            return null;
                        });
                },
                OnDistanceReached: null);
        }

        [UnityTest]
        public IEnumerator AI_TargetZone_NotInterruptedBy_Projectile_TargetZoneOnStayEvent()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseAIBheavior.AIComponents, 170f);
            yield return PuzzleSceneTestHelper.TargetZoneYield(new TargetZoneInherentData(9999, 170), mouseTestAIManager.transform.position,
                OnTargetZoneSpawn: (TargetZone targetZone) =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                    Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);
                    var currentFOVAngleSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
                    return PuzzleSceneTestHelper.ProjectileYield(PuzzleSceneTestHelper.CreateProjectileInherentData(1000f, 1), mouseTestAIManager.transform.position,
                        OnProjectileSpawn: (LaunchProjectile projectile) =>
                        {
                            Assert.AreEqual(1, this.MockPuzzleEventsManagerTest.AiHittedByProjectileCallCount);
                            Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone());
                            Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                            Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                            //Event if not interrupted by projectile, FOV is still reduced to take the projectile into account.
                            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < currentFOVAngleSum);
                            return null;
                        },
                        OnDistanceReached: () =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone());
                            Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                            Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() != currentFOVAngleSum); //The FOV have been resetted, then recalculated by target zone stay
                            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);
                            return null;
                        });
                },
                OnDistanceReached: null);
        }

        [UnityTest]
        public IEnumerator AI_TargetZone_InterruptedBy_EscapeWithoutTarget()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseAIBheavior.AIComponents, 90f);
            ((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents).AIFearStunComponent.FOVSumThreshold = 0f; //prenvenitng
            yield return PuzzleSceneTestHelper.TargetZoneYield(new TargetZoneInherentData(9999, 170), mouseTestAIManager.transform.position,
                OnTargetZoneSpawn: (TargetZone targetZone) =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingWithoutTarget());
                    Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                    Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);
                    var currentFOVnalgeSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
                    //Target zone is destroyed to simulate the fact that the AI is exiting the target zone
                    MonoBehaviour.DestroyImmediate(targetZone.gameObject);
                    return PuzzleSceneTestHelper.ProjectileIngoreTargetYield(PuzzleSceneTestHelper.CreateProjectileInherentData(1000, 1), mouseTestAIManager.transform.position,
                        OnBeforeSecondProjectileSpawn: () =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                            Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone());
                            Assert.IsFalse(mouseAIBheavior.IsEscapingWithoutTarget());
                            Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < currentFOVnalgeSum);
                            currentFOVnalgeSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
                            return null;
                        },
                        OnSecondProjectileSpawned: (LaunchProjectile projectile) =>
                        {
                            Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                            Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone());
                            Assert.IsTrue(mouseAIBheavior.IsEscapingWithoutTarget());
                            Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < currentFOVnalgeSum);
                            currentFOVnalgeSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
                            return null;
                        },
                        OnSecondProjectileDistanceReached: null);
                },
                OnDistanceReached: null
            );
        }

        [UnityTest]
        public IEnumerator AI_TargetZone_InterruptedBy_PlayerEscape()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();

            yield return PuzzleSceneTestHelper.TargetZoneYield(new TargetZoneInherentData(9999, 170), mouseTestAIManager.transform.position,
               OnTargetZoneSpawn: (TargetZone targetZone) =>
               {
                   Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone());
                   Assert.IsFalse(mouseAIBheavior.IsEscapingFromPlayer());
                   Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                   Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);

                   var currentFOVAngleSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
                   PuzzleSceneTestHelper.SetPlayerEscapeComponentValues((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents, 1, 90f, 9999f);
                   return PuzzleSceneTestHelper.EscapeFromPlayerYield(playerManager, mouseTestAIManager,
                       OnBeforeSettingPosition: null,
                       OnSamePositionSetted: () =>
                       {
                           Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone());
                           Assert.IsTrue(mouseAIBheavior.IsEscapingFromPlayer());
                           Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                           Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < currentFOVAngleSum);

                           currentFOVAngleSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
                           PuzzleSceneTestHelper.SetPlayerEscapeComponentValues((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents, 1, 90f, 0f);
                           return null;
                       },
                       OnDestinationReached: () =>
                       {
                           Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone());
                           Assert.IsFalse(mouseAIBheavior.IsEscapingFromPlayer());
                           Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);

                           Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() != currentFOVAngleSum); //The FOV have been resetted, then recalculated by target zone stay
                           Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);
                           return null;
                       });

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
            yield return PuzzleSceneTestHelper.TargetZoneYield(new TargetZoneInherentData(1, 170), mouseTestAIManager.transform.position,
                OnTargetZoneSpawn: (TargetZone targetZone) =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone());
                    Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                    Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);

                    var currentFOVAngleSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
                    return PuzzleSceneTestHelper.AttractiveObjectYield(PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(300, 0.15f), mouseTestAIManager.transform.position,
                        OnAttractiveObjectSpawn: (AttractiveObjectType attractiveObject) =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone());
                            Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == currentFOVAngleSum);
                            return null;
                        },
                        OnAttractiveObjectDestroyed: null);
                },
                OnDistanceReached: () =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone());
                    Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject());
                    Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
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
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseAIBheavior.AIComponents, 0.5f);
            var fearTime = 0.05f;
            ((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents).AIFearStunComponent.TimeWhileBeginFeared = fearTime;

            var projectileData = PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 30f);
            var projectileSpanwed = PuzzleSceneTestHelper.SpawnProjectile(projectileData, mouseTestAIManager.transform.position, launchProjectileContainerManager);

            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
            Assert.IsTrue(mouseAIBheavior.IsFeared());
            yield return new WaitForSeconds(fearTime);
            Assert.IsFalse(mouseAIBheavior.IsFeared());
        }

        [UnityTest]
        public IEnumerator AI_FearStun_OnForcedFear_IsFeared()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseAIBheavior.AIComponents, 1f);
            mouseTestAIManager.GetAgent().Warp(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.PITFAL_Z_POSITION_FAR_EDGE).transform.position);
            ((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents).AIFearStunComponent.FOVSumThreshold = 0f;
            ((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents).AIProjectileEscapeWithCollisionComponent.EscapeDistanceV2.Values[LaunchProjectileId.TEST] = 5f;
            yield return null;
            Assert.IsFalse(mouseAIBheavior.IsFeared());
            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
            yield return PuzzleSceneTestHelper.ProjectileYield(PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 30f), mouseTestAIManager.transform.position - Vector3.forward,
                OnProjectileSpawn: (LaunchProjectile lauinchProjectile) =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsFeared());
                    Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);
                    return null;
                },
                OnDistanceReached: null
                );
        }

        [UnityTest]
        public IEnumerator AI_FearStun_NotInterruptedBy_Attractive()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();

            var fearTime = 0.3f;
            ((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents).AIFearStunComponent.TimeWhileBeginFeared = fearTime;
            yield return PuzzleSceneTestHelper.FearYield(mouseTestAIManager.transform.position,
                OnFearTriggered: () =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsFeared());
                    Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                    return PuzzleSceneTestHelper.AttractiveObjectYield(PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(999, 0.1f), mouseTestAIManager.transform.position,
                        OnAttractiveObjectSpawn: (AttractiveObjectType attractiveObject) =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsFeared());
                            Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                            return null;
                        },
                        OnAttractiveObjectDestroyed: () =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsFeared());
                            Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                            return null;
                        }
                        );
                },
                OnFearEnded: () =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsFeared());
                    Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
                    return null;
                },
               fearTime,
               mouseAIBheavior
             );
        }

        [UnityTest]
        public IEnumerator AI_FearStun_NotInterruptedBy_Projectile()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();

            var fearTime = 0.1f;
            ((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents).AIFearStunComponent.TimeWhileBeginFeared = fearTime;
            yield return PuzzleSceneTestHelper.FearYield(mouseTestAIManager.transform.position,
                OnFearTriggered: () =>
                {
                    PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseAIBheavior.AIComponents, 170f);

                    Assert.IsTrue(mouseAIBheavior.IsFeared());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                    Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                    return PuzzleSceneTestHelper.ProjectileYield(PuzzleSceneTestHelper.CreateProjectileInherentData(9999f, 1), mouseTestAIManager.transform.position,
                        OnProjectileSpawn: (LaunchProjectile projectile) =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsFeared());
                            Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                            Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                            return null;
                        },
                        OnDistanceReached: null
                    );
                },
                OnFearEnded: () =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsFeared());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                    Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                    return null;
                },
                fearTime,
                mouseAIBheavior
             );
        }

        [UnityTest]
        public IEnumerator Ai_FearStun_NotInterruptedBy_EscapeWithoutTarget()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();

            var fearTime = 0.1f;
            ((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents).AIFearStunComponent.TimeWhileBeginFeared = fearTime;
            yield return PuzzleSceneTestHelper.FearYield(mouseTestAIManager.transform.position,
                OnFearTriggered: () =>
                {
                    PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseAIBheavior.AIComponents, 170f);

                    Assert.IsTrue(mouseAIBheavior.IsFeared());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingWithoutTarget());
                    Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                    return PuzzleSceneTestHelper.ProjectileIngoreTargetYield(PuzzleSceneTestHelper.CreateProjectileInherentData(999f, 1), mouseTestAIManager.transform.position,
                       OnBeforeSecondProjectileSpawn: null,
                       OnSecondProjectileSpawned: (LaunchProjectile projectile) =>
                       {
                           Assert.IsTrue(mouseAIBheavior.IsFeared());
                           Assert.IsFalse(mouseAIBheavior.IsEscapingWithoutTarget());
                           Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                           return null;
                       },
                       OnSecondProjectileDistanceReached: null
                       );
                },
                OnFearEnded: () =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsFeared());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingWithoutTarget());
                    Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                    return null;
                },
                fearTime,
                mouseAIBheavior
             );
        }

        [UnityTest]
        public IEnumerator Ai_FearStun_NotInterruptedBy_TargetZone()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();

            var fearTime = 0.2f;
            ((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents).AIFearStunComponent.TimeWhileBeginFeared = fearTime;
            yield return PuzzleSceneTestHelper.FearYield(mouseTestAIManager.transform.position,
                OnFearTriggered: () =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsFeared());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone());
                    return PuzzleSceneTestHelper.TargetZoneYield(new TargetZoneInherentData(9999f, 170), mouseTestAIManager.transform.position,
                        OnTargetZoneSpawn: (TargetZone targetZone) =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsFeared());
                            Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone());
                            return null;
                        },
                        OnDistanceReached: null
                    );
                },
                OnFearEnded: () =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsFeared());
                    Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone());
                    return null;
                },
                fearTime,
                mouseAIBheavior
             );
        }

        [UnityTest]
        public IEnumerator AI_PlayerEscape_EscapeFromPlayerWhenInRange()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();

            PuzzleSceneTestHelper.SetPlayerEscapeComponentValues(((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents), 10f, 90f, 0.1f);

            yield return PuzzleSceneTestHelper.EscapeFromPlayerYield(playerManager, mouseTestAIManager,
                   OnBeforeSettingPosition: () =>
                   {
                       Assert.IsFalse(mouseAIBheavior.IsEscapingFromPlayer());
                       Assert.IsTrue(mouseAIBheavior.IsPatrolling());
                       Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                       Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
                       return null;
                   },
                   OnSamePositionSetted: () =>
                   {
                       Assert.IsTrue(mouseAIBheavior.IsEscapingFromPlayer());
                       Assert.IsFalse(mouseAIBheavior.IsPatrolling());
                       Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                       Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);
                       return null;
                   },
                   OnDestinationReached: () =>
                   {
                       Assert.IsFalse(mouseAIBheavior.IsEscapingFromPlayer());
                       Assert.IsTrue(mouseAIBheavior.IsPatrolling());
                       Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                       Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
                       return null;
                   });
        }

        [UnityTest]
        public IEnumerator AI_PlayerEscape_EscapeFromPlayerWhenInRange_StillEscapeAfterDestinationReached()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            var currentFOVAngleSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();

            PuzzleSceneTestHelper.SetPlayerEscapeComponentValues(((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents), 1f, 90f, 0.1f);

            yield return PuzzleSceneTestHelper.EscapeFromPlayerYield(playerManager, mouseTestAIManager,
                   OnBeforeSettingPosition: () =>
                   {
                       Assert.IsFalse(mouseAIBheavior.IsEscapingFromPlayer());
                       Assert.IsTrue(mouseAIBheavior.IsPatrolling());
                       Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                       Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
                       return null;
                   },
                   OnSamePositionSetted: () =>
                   {
                       Assert.IsTrue(mouseAIBheavior.IsEscapingFromPlayer());
                       Assert.IsFalse(mouseAIBheavior.IsPatrolling());
                       Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                       Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);

                       currentFOVAngleSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
                       PuzzleSceneTestHelper.SetPlayerEscapeComponentValues(((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents), 1f, 90f, 99999f);
                       return null;
                   },
                   OnDestinationReached: () =>
                   {
                       Assert.IsTrue(mouseAIBheavior.IsEscapingFromPlayer());
                       Assert.IsFalse(mouseAIBheavior.IsPatrolling());
                       Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                       Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == currentFOVAngleSum);
                       return null;
                   });
        }

        [UnityTest]
        public IEnumerator AI_PlayerEscape_EscapeFromPlayerWhenInRange_WhenThereIsNoAvailableDestination_BeFeared()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();
            var playerDataRetriver = GameObject.FindObjectOfType<PlayerManagerDataRetriever>();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();

            PuzzleSceneTestHelper.SetPlayerEscapeComponentValues(((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents), 1f, 20f, 1);
            mouseTestAIManager.GetAgent().Warp(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.PITFAL_Z_POSITION_1).transform.position);
            playerDataRetriver.GetPlayerRigidBody().transform.position = PuzzleSceneTestHelper.FindTestPosition(TestPositionID.PITFAL_Z_POSITION_1).transform.position - (Vector3.forward * 0.5f);
            yield return null;
            playerDataRetriver.GetPlayerRigidBody().transform.position = Vector3.zero;
            Assert.IsTrue(mouseAIBheavior.IsFeared());
            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);
        }

        [UnityTest]
        public IEnumerator AI_PlayerEscape_EscapeFromPlayerWhenInRange_StillEscapeWhenThereIsStillDistanceToTravel()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            var currentFOVAngleSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();

            PuzzleSceneTestHelper.SetPlayerEscapeComponentValues(((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents), 99999f, 90f, 0.1f);

            yield return PuzzleSceneTestHelper.EscapeFromPlayerYield(playerManager, mouseTestAIManager,
                   OnBeforeSettingPosition: () =>
                   {
                       Assert.IsFalse(mouseAIBheavior.IsEscapingFromPlayer());
                       Assert.IsTrue(mouseAIBheavior.IsPatrolling());
                       Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                       Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
                       return null;
                   },
                   OnSamePositionSetted: () =>
                   {
                       Assert.IsTrue(mouseAIBheavior.IsEscapingFromPlayer());
                       Assert.IsFalse(mouseAIBheavior.IsPatrolling());
                       Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                       Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);

                       currentFOVAngleSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
                       return null;
                   },
                   OnDestinationReached: null);
            TestHelperMethods.SetAgentDestinationPositionReached(mouseTestAIManager, mouseTestAIManager.transform.position + (Vector3.forward * 10f));
            Assert.IsTrue(mouseAIBheavior.IsEscapingFromPlayer());
            Assert.IsFalse(mouseAIBheavior.IsPatrolling());
            Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == currentFOVAngleSum);
        }

        [UnityTest]
        public IEnumerator AI_PlayerEscape_EscapeFromPlayerWhenInRange_WhenEscapingWithoutTargetZones_DestinationIntoTargetZone()
        {
            yield return this.Before(SceneConstants.OneAIForcedTargetZone);
            yield return null;

            var launchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();
            var targetZoneCollider = PuzzleSceneTestHelper.FindTargetZone(TargetZoneID.TEST_TARGET_ZONE).TargetZoneTriggerType.GetTargetZoneTriggerCollider();
            var currentFOVAngleSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseAIBheavior.AIComponents, 90f);

            PuzzleSceneTestHelper.SetPlayerEscapeComponentValues(((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents), 99999f, 90f, 0.2f);
            playerManager.transform.position = new Vector3(999, 999, 999);
            ((GenericPuzzleAIComponents)mouseTestAIManager.GetAIBehavior().AIComponents).AIProjectileEscapeWithCollisionComponent.EscapeDistanceV2.Values[LaunchProjectileId.TEST] = 999f;

            yield return PuzzleSceneTestHelper.EscapeFromPlayerIgnoreTargetYield(playerManager, mouseTestAIManager,
                PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 30f), mouseTestAIManager.transform.position + (Vector3.right * 0.1f),
                OnBeforeSettingPosition: () =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromPlayer());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingWithoutTarget());
                    Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                    Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);

                    currentFOVAngleSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
                    playerManager.transform.position = mouseTestAIManager.transform.position + Vector3.right;
                    return null;
                },
                OnSamePositionSetted: () =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromPlayer());
                    Assert.IsTrue(mouseAIBheavior.IsEscapingWithoutTarget());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectileWithTargetZones());
                    Assert.IsTrue(targetZoneCollider.bounds.Contains(mouseTestAIManager.GetAgent().destination));
                    Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < currentFOVAngleSum);
                    return null;
                },
                OnDestinationReached: null);
        }

        [UnityTest]
        public IEnumerator AI_PlayerEscape_InterruptedBy_TargetZone()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            var currentFOVAngleSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();

            PuzzleSceneTestHelper.SetPlayerEscapeComponentValues(((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents), 10f, 90f, 1f);

            yield return PuzzleSceneTestHelper.EscapeFromPlayerYield(playerManager, mouseTestAIManager,
                  OnBeforeSettingPosition: () =>
                  {
                      Assert.IsFalse(mouseAIBheavior.IsEscapingFromPlayer());
                      Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone());
                      Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                      Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
                      currentFOVAngleSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
                      return null;
                  },
                  OnSamePositionSetted: () =>
                  {
                      Assert.IsTrue(mouseAIBheavior.IsEscapingFromPlayer());
                      Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone());
                      Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                      Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);
                      currentFOVAngleSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
                      return PuzzleSceneTestHelper.TargetZoneYield(new TargetZoneInherentData(9999f, 170), mouseTestAIManager.transform.position,
                          OnTargetZoneSpawn: (TargetZone targetZone) =>
                          {
                              Assert.IsFalse(mouseAIBheavior.IsEscapingFromPlayer());
                              Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone());
                              Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                              //AI FOV is resetted when playerescape is interrupted by target zone
                              Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() > currentFOVAngleSum);
                              Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);
                              return null;
                          },
                          OnDistanceReached: null);
                  },
                  OnDestinationReached: null);
        }

        [UnityTest]
        public IEnumerator AI_PlayerEscape_InterruptedBy_EscapeWithoutTarget()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            var currentFOVAngleSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseAIBheavior.AIComponents, 170f);

            yield return PuzzleSceneTestHelper.ProjectileYield(PuzzleSceneTestHelper.CreateProjectileInherentData(999f, 999999f), mouseTestAIManager.transform.position,
                OnProjectileSpawn: (LaunchProjectile launchProjectile) =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsEscapingWithoutTarget());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromPlayer());
                    Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < 360f);

                    currentFOVAngleSum = mouseAIBheavior.AIFOVManager.GetFOVAngleSum();
                    PuzzleSceneTestHelper.SetPlayerEscapeComponentValues(((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents), 10f, 90f, 1f);
                    return PuzzleSceneTestHelper.EscapeFromPlayerYield(playerManager, mouseTestAIManager,
                        OnBeforeSettingPosition: null,
                        OnSamePositionSetted: () =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsEscapingWithoutTarget());
                            Assert.IsFalse(mouseAIBheavior.IsEscapingFromPlayer());
                            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() < currentFOVAngleSum);
                            return null;
                        },
                        OnDestinationReached: () =>
                        {
                            Assert.IsFalse(mouseAIBheavior.IsEscapingWithoutTarget());
                            Assert.IsFalse(mouseAIBheavior.IsEscapingFromPlayer());
                            Assert.IsTrue(mouseAIBheavior.AIFOVManager.GetFOVAngleSum() == 360f);
                            return null;
                        }
                     );
                },
                OnDistanceReached: null);
        }

        [UnityTest]
        public IEnumerator AI_PlayerEscape_InterruptedBy_FearStun()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetPlayerEscapeComponentValues(((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents), 10f, 90f, 1f);
            yield return PuzzleSceneTestHelper.EscapeFromPlayerYield(playerManager, mouseTestAIManager,
                OnBeforeSettingPosition: () =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsEscapingFromPlayer());
                    Assert.IsFalse(mouseAIBheavior.IsFeared());
                    Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                    return null;
                },
                OnSamePositionSetted: () =>
                {

                    Assert.IsTrue(mouseAIBheavior.IsEscapingFromPlayer());
                    Assert.IsFalse(mouseAIBheavior.IsFeared());
                    Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                    return PuzzleSceneTestHelper.FearYield(mouseTestAIManager.transform.position,
                       OnFearTriggered: () =>
                       {
                           Assert.IsFalse(mouseAIBheavior.IsEscapingFromPlayer());
                           Assert.IsTrue(mouseAIBheavior.IsFeared());
                           return null;
                       },
                       OnFearEnded: null, 1f, mouseAIBheavior);
                },
                OnDestinationReached: null);
        }


        [UnityTest]
        public IEnumerator AI_EscapeWithoutarget_InterruptedBy_FearStun()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseAIBheavior.AIComponents, 170f);
            var fearTime = 10f;
            ((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents).AIFearStunComponent.TimeWhileBeginFeared = fearTime;

            yield return PuzzleSceneTestHelper.ProjectileIngoreTargetYield(PuzzleSceneTestHelper.CreateProjectileInherentData(9999f, 30f), mouseTestAIManager.transform.position,
                OnBeforeSecondProjectileSpawn: () =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsFeared());
                    Assert.IsFalse(mouseAIBheavior.IsEscapingWithoutTarget());
                    return null;
                },
                OnSecondProjectileSpawned: (LaunchProjectile launchProjectile) =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsFeared());
                    Assert.IsTrue(mouseAIBheavior.IsEscapingWithoutTarget());
                    return PuzzleSceneTestHelper.FearYield(mouseTestAIManager.transform.position,
                        OnFearTriggered: () =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsFeared());
                            Assert.IsFalse(mouseAIBheavior.IsEscapingWithoutTarget());
                            return null;
                        },
                        OnFearEnded: null, 0f, mouseAIBheavior);
                },
                OnSecondProjectileDistanceReached: null);
        }

    }


}
