using CoreGame;
using RTPuzzle;
using System;
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
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aiManager = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_1).Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            // (1) - Initialization process
            yield return null;
            var oldPosition = aiManager.transform.position;
            // (2) - Let a frame pass to let the AI travel distance
            yield return null;
            var newPosition = aiManager.transform.position;
            Assert.AreNotEqual(newPosition, oldPosition);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            // (3) - The AI is in a patrolling state
            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>());
            // (4) - The AI FOV is not modified for patrolling
            Assert.AreEqual(new StartEndSlice(0, 360), aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOV().FovSlices[0]);
            var beforeReachDestination = aiManager.GetAgent().destination;
            // (5) - The AI has reached the position
            TestHelperMethods.SetAgentDestinationPositionReached(aiManager.GetAgent());
            yield return null;
            this.MockPuzzleEventsManagerTest.ClearCalls();
            // (6) - The AI is still in a patrolling state because nothing has happened
            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>());
            var afterReachDestination = aiManager.GetAgent().destination;
            Assert.AreNotEqual(beforeReachDestination, afterReachDestination);
        }

        [UnityTest]
        public IEnumerator AI_RandomPatrol_InterruptedBy_AttractiveObject()
        {
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aiManager = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_1).Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>());
            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
            yield return PuzzleSceneTestHelper.AttractiveObjectYield(AttractiveObjectDefinition.AttractiveObjectOnly(InteractiveObjectTestID.TEST_1, 1000, 0.02f), aiManager.transform.position,
                OnAttractiveObjectSpawn: (InteractiveObjectType attractiveObjectType) =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>());
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                    return null;
                },
                OnAttractiveObjectDestroyed: () =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                    return null;
                });
        }
        [UnityTest]
        public IEnumerator AI_RandomPatrol_InterruptedBy_Projectile()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_1);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();

            var projectile = ProjectileInteractiveObjectDefinitions.ExplodingProjectile(InteractiveObjectTestID.TEST_2, 1000, 0.1f);
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(InteractiveObjectTestID.TEST_2, aIObjectInitialization, 175f);

            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>());
            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
            yield return PuzzleSceneTestHelper.ProjectileYield(projectile, aiManager.transform.position,
                OnProjectileSpawn: (InteractiveObjectType p) =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>());
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                    Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);
                    return null;
                },
                OnDistanceReached: () =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                    Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
                    return null;
                },
                OnDistanceReaderAIObjectType: () => aiManager
             );
        }

        [UnityTest]
        public IEnumerator AI_RandomPatrol_InterruptedBy_TargetZone()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_1);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });

            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
            yield return PuzzleSceneTestHelper.TargetZoneYield(new TargetZoneInherentData(1, 170), aiManager.transform.position,
                OnTargetZoneSpawn: (InteractiveObjectType targetZone) =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>());
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                    Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);
                    return null;
                },
                OnDistanceReached: () =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                    Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
                    return null;
                },
                OnDistanceReachedAIObjectType: () => aiManager
                );
        }

        [UnityTest]
        public IEnumerator AI_RandomPatrol_InterruptedBy_EscapeIgnoreTarget()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_1);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(InteractiveObjectTestID.TEST_2, aIObjectInitialization, 170f);

            var currentFOVSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
            Assert.IsTrue(currentFOVSum == 360f);
            yield return PuzzleSceneTestHelper.ProjectileIngoreTargetYield(ProjectileInteractiveObjectDefinitions.ExplodingProjectile(InteractiveObjectTestID.TEST_2, 3, 1), aiManager.transform.position,
                OnBeforeSecondProjectileSpawn: () =>
                {
                    currentFOVSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
                    Assert.IsTrue(currentFOVSum < 360f);
                    return null;
                },
                OnSecondProjectileSpawned: (InteractiveObjectType projectile) =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
                    Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < currentFOVSum);
                    currentFOVSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
                    return null;
                },
                OnSecondProjectileDistanceReached: () =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
                    Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
                    return null;
                },
                OnSecondProjectileDistanceReachedAIObjectType: () => aiManager);
        }

        [UnityTest]
        public IEnumerator AI_RandomPatrol_InterruptedBy_Fear()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_1);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            var fearTime = 0.05f;

            aIObjectInitialization.AIObjectTypeDefinitionInherentData.GenericPuzzleAIComponents.GetDefinitionModule<AIFearStunComponent>().TimeWhileBeginFeared = fearTime;

            yield return PuzzleSceneTestHelper.FearYield(aiManager.transform.position, InteractiveObjectTestID.TEST_2, aIObjectInitialization,
                OnFearTriggered: () =>
                {
                    Debug.Log(mouseAIBheavior.ToString());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>());
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
                    return null;
                },
                OnFearEnded: () =>
                {
                    Debug.Log(mouseAIBheavior.ToString());
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
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
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_1);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();

            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(InteractiveObjectTestID.TEST_2, aIObjectInitialization, 90f);
            // (1) - Initialization process
            yield return null;
            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>(), "The AI has no interaction -> Patrolling.");
            Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
            var projectileData = ProjectileInteractiveObjectDefinitions.ExplodingProjectile(InteractiveObjectTestID.TEST_2, 99999f, 30f);
            var lpTest = PuzzleSceneTestHelper.SpawnProjectile(projectileData, aiManager.transform.position);
            // (2) - Wait for projectile to be processed by physics engine. The AI must be in escape from projectile state.
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>(), "The AI has been hit, no more patrolling.");
            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>(), "The AI has been hit, escaping from projectile while taking into account target zones.");
            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>(), "The AI should not ignore target zones while escaping from projectile for the first time.");
            Assert.AreEqual(this.MockPuzzleEventsManagerTest.AiHittedByProjectileCallCount, 1, "The 'hitted by projectile event' must be triggered only once.");
            Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);
            // (3) - After processing, the projecile must be destroyed
            yield return null;
            Assert.IsNull(lpTest, "The projectile must be detroyed after hit.");
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_WhenDestinationReached_NoMoreEscape_Test()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_1);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(InteractiveObjectTestID.TEST_2, aIObjectInitialization, 90f);
            yield return null;
            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>(), "The AI has no interaction -> Patrolling.");
            var projectileData = ProjectileInteractiveObjectDefinitions.ExplodingProjectile(InteractiveObjectTestID.TEST_2, 99999f, 30f);
            var lpTest = PuzzleSceneTestHelper.SpawnProjectile(projectileData, aiManager.transform.position);
            yield return new WaitForFixedUpdate();
            //projectile taken into account
            yield return new WaitForEndOfFrame();
            //clear agent destinations
            var agent = aiManager.GetAgent();
            TestHelperMethods.SetAgentDestinationPositionReached(agent);
            // wait for set destination
            yield return null;
            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>(), "The AI should no more escape from projectile when destination is reached.");
            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>(), "The AI should no more escape from projectile when destination is reached.");
            Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
        }

        private IEnumerator AI_ProjectileReceived_FirstTimeNotIntoTargetZone(string sceneName, AIObjectInitialization AIObjectInitialization,
            InteractiveObjectTestID projectileInteractiveObjectTestID,
            InteractiveObjectTestID targetZoneInteractiveObjectTestID, Action objectDynamicInstancesCreation = null)
        {
            yield return this.Before(sceneName, objectDynamicInstancesCreation);
            var mouseTestAIManager = FindObjectOfType<AIManagerContainer>().GetNPCAiManager(AIObjectInitialization.AIObjectID);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(projectileInteractiveObjectTestID, AIObjectInitialization, 90f);
            yield return null;
            var projectileData = ProjectileInteractiveObjectDefinitions.ExplodingProjectile(projectileInteractiveObjectTestID, 99999f, 30f);
            Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
            var firstProj = PuzzleSceneTestHelper.SpawnProjectile(projectileData, TestPositionID.PROJECTILE_TARGET_1);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame(); //wait for destination position to update
            var aiDestination = mouseTestAIManager.GetAgent().destination;
            var targetZoneCollider = PuzzleSceneTestHelper.FindTargetZone(InteractiveObjectTestIDTree.InteractiveObjectTestIDs[targetZoneInteractiveObjectTestID].TargetZoneID).ILevelCompletionTriggerModuleDataRetriever.GetTargetZoneTriggerCollider();
            Assert.IsFalse(targetZoneCollider.bounds.Contains(aiDestination), "AI Destination should not be in the target zone the first hit.");
            Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
        }

        private IEnumerator AI_ProjectileReceived_SecondTime_EscapeWithoutTarget_DestinationInTargetZone(string sceneName, float? projectileEscapeDistance,
                    AIObjectInitialization AIObjectInitialization,
                    InteractiveObjectTestID projectileInteractiveObjectTestID,
                    InteractiveObjectTestID targetZoneInteractiveObjectTestID, Action objectDynamicInstancesCreation = null)
        {
            yield return this.AI_ProjectileReceived_FirstTimeNotIntoTargetZone(sceneName, AIObjectInitialization, projectileInteractiveObjectTestID, targetZoneInteractiveObjectTestID, objectDynamicInstancesCreation);
            var mouseTestAIManager = FindObjectOfType<AIManagerContainer>().GetNPCAiManager(AIObjectInitialization.AIObjectID);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(projectileInteractiveObjectTestID, AIObjectInitialization, 90f);
            if (projectileEscapeDistance != null)
            {
                PuzzleSceneTestHelper.SetAIEscapeDistanceFromProjectile(projectileInteractiveObjectTestID, AIObjectInitialization, projectileEscapeDistance.Value);
            }
            var projectileData = ProjectileInteractiveObjectDefinitions.ExplodingProjectile(projectileInteractiveObjectTestID, 99999f, 30f);
            var secondProj = PuzzleSceneTestHelper.SpawnProjectile(projectileData, TestPositionID.PROJECTILE_TARGET_2);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame(); //wait for destination position to update
            var aiDestination = mouseTestAIManager.GetAgent().destination;
            var targetZoneCollider = PuzzleSceneTestHelper.FindTargetZone(InteractiveObjectTestIDTree.InteractiveObjectTestIDs[targetZoneInteractiveObjectTestID].TargetZoneID).ILevelCompletionTriggerModuleDataRetriever.GetTargetZoneTriggerCollider();
            Assert.IsTrue(targetZoneCollider.bounds.Contains(aiDestination), "AI Destination should contains the target zone the second hit.");
            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>(), "The AI should ignore target zones while escaping from projectile for the second time.");
            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>(), "The AI should not escape with target zone consideration.");
            Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_FirstTimeNotIntoTargetZone()
        {
            var targetZoneTestID = InteractiveObjectTestID.TEST_3;
            AIObjectInitialization aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_1);
            AIObjectType aiManager = null;
            yield return this.AI_ProjectileReceived_FirstTimeNotIntoTargetZone(SceneConstants.OneAIForcedTargetZone, aIObjectInitialization,
                InteractiveObjectTestID.TEST_2, targetZoneTestID, () =>
                {
                    aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
                    LevelCompletionTriggerDefinition.LevelCOmpletionTargetZone(targetZoneTestID, Vector3.zero, new Vector3(25.29f, 12.17f, 12.4f), 17.8f, 110f).Instanciate(new Vector3(-36.08f, 0.5f, -4.4f));
                });
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_FirstTimeIntoTargetZoneDistanceCheck()
        {
            AIObjectInitialization aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_3);
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAIForcedTargetZone, () =>
            {
                LevelCompletionTriggerDefinition.LevelCOmpletionTargetZone(InteractiveObjectTestID.TEST_2, Vector3.zero, new Vector3(25.29f, 12.17f, 12.4f), 17.8f, 110f).Instanciate(new Vector3(-36.08f, 0.5f, -4.4f));
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();
            playerManager.transform.position = new Vector3(9999, 999, 999);
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(InteractiveObjectTestID.TEST_1, aIObjectInitialization, 20f);
            yield return null;
            var projectileData = ProjectileInteractiveObjectDefinitions.ExplodingProjectile(InteractiveObjectTestID.TEST_1, 99999f, 30f);
            var firstProj = PuzzleSceneTestHelper.SpawnProjectile(projectileData, TestPositionID.PROJECTILE_TARGET_1);
            var currentFOVSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
            firstProj.transform.position = aiManager.transform.position + new Vector3(-0.1f, 0, 0);
            Assert.IsTrue(currentFOVSum == 360f);
            yield return new WaitForFixedUpdate(); //projectile processing
            currentFOVSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
            TestHelperMethods.SetAgentDestinationPositionReached(aiManager.GetAgent());
            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>(), "AI should not escape from target zone when a projectile is triggered.");
            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>(), "AI should escape form projectile when a projectile is triggered.");
            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>(), "AI should escape while ignoring target zones when a projectile is triggered for the first time.");
            Assert.IsTrue(currentFOVSum < 360f);
            yield return new WaitForFixedUpdate();
            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>(), "AI should escape from target zone when target zone distance check is triggered.");
            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>(), "AI should not escape form projectile because target zone is cancelling it.");
            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() != currentFOVSum, "The AI FOV must be reseted before processing target zone event.");
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_WhenThereIsNoAvailableDestination_BeFeared()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            aiManager.GetAgent().Warp(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.PITFAL_Z_POSITION_1).transform.position);
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(InteractiveObjectTestID.TEST_1, aIObjectInitialization, 20f);
            yield return null;
            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
            yield return PuzzleSceneTestHelper.ProjectileYield(ProjectileInteractiveObjectDefinitions.ExplodingProjectile(InteractiveObjectTestID.TEST_1, 99999f, 30f), aiManager.transform.position - Vector3.forward,
                OnProjectileSpawn: (InteractiveObjectType lauinchProjectile) =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
                    Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);
                    return null;
                },
                OnDistanceReached: null,
                OnDistanceReaderAIObjectType: null
                );
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_InterruptedBy_TargetZone_Test()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(InteractiveObjectTestID.TEST_1, aIObjectInitialization, 170f);
            var currentFOVSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
            Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
            yield return PuzzleSceneTestHelper.ProjectileYield(ProjectileInteractiveObjectDefinitions.ExplodingProjectile(InteractiveObjectTestID.TEST_1, 99999f, 30f), aiManager.transform.position,
                 OnProjectileSpawn: (InteractiveObjectType lauinchProjectile) =>
                 {
                     Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                     Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                     Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                     Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < currentFOVSum);
                     currentFOVSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
                     return PuzzleSceneTestHelper.TargetZoneYield(new TargetZoneInherentData(0.1f, 170), aiManager.transform.position,
                         OnTargetZoneSpawn: (InteractiveObjectType targetZone) =>
                         {
                             Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                             Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                             Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                             Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < currentFOVSum);
                             currentFOVSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
                             return null;
                         },
                         OnDistanceReached: () =>
                         {
                             Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                             Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                             Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                             Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360);
                             return null;
                         },
                         OnDistanceReachedAIObjectType: () => aiManager
                     );
                 },
                 OnDistanceReached: null,
                 OnDistanceReaderAIObjectType: null
                 );
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_InterruptedBy_EscapeWithoutTarget()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(InteractiveObjectTestID.TEST_1, aIObjectInitialization, 170f);
            var currentFOVSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();

            Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
            yield return PuzzleSceneTestHelper.ProjectileYield(ProjectileInteractiveObjectDefinitions.ExplodingProjectile(InteractiveObjectTestID.TEST_1, 99999f, 30f), aiManager.transform.position,
                 OnProjectileSpawn: (InteractiveObjectType lauinchProjectile) =>
                 {
                     Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                     Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
                     Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < currentFOVSum);
                     currentFOVSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
                     return PuzzleSceneTestHelper.ProjectileYield(ProjectileInteractiveObjectDefinitions.ExplodingProjectile(InteractiveObjectTestID.TEST_1, 99999f, 30f), aiManager.transform.position,
                         OnProjectileSpawn: (InteractiveObjectType launchProjectile) =>
                         {
                             Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                             Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
                             Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < currentFOVSum);
                             return null;
                         },
                         OnDistanceReached: () =>
                         {
                             Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                             Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
                             Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360);
                             return null;
                         },
                         OnDistanceReaderAIObjectType: () => aiManager);
                 },
                 OnDistanceReached: null,
                 OnDistanceReaderAIObjectType: null
                 );
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_InterruptedBy_PlayerEscape()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(InteractiveObjectTestID.TEST_1, aIObjectInitialization, 170f);
            var currentFOVSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();

            Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
            yield return PuzzleSceneTestHelper.ProjectileYield(ProjectileInteractiveObjectDefinitions.ExplodingProjectile(InteractiveObjectTestID.TEST_1, 9999f, 30f), aiManager.transform.position,
                OnProjectileSpawn: (InteractiveObjectType launchProjectile) =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
                    Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                    Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);

                    currentFOVSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
                    PuzzleSceneTestHelper.SetPlayerEscapeComponentValues(aIObjectInitialization, 10f, 90f, 1f);
                    return PuzzleSceneTestHelper.EscapeFromPlayerYield(playerManager, aiManager,
                        OnBeforeSettingPosition: null,
                        OnSamePositionSetted: () =>
                        {
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                            Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
                            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < currentFOVSum);
                            return null;
                        },
                        OnDestinationReached: () =>
                        {
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                            Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
                            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360);
                            return null;
                        },
                        OnDestinationReachedAIObjectType: () => aiManager);
                },
                OnDistanceReached: null,
                OnDistanceReaderAIObjectType: null);
        }

        [UnityTest]
        public IEnumerator AI_ProjectileRecevied_NotInterruptedBy_Attractive_Test()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(InteractiveObjectTestID.TEST_1, aIObjectInitialization, 170f);

            Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);

            yield return PuzzleSceneTestHelper.ProjectileYield(ProjectileInteractiveObjectDefinitions.ExplodingProjectile(InteractiveObjectTestID.TEST_1, 99999f, 30f), aiManager.transform.position,
                OnProjectileSpawn: (InteractiveObjectType lauinchProjectile) =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                    Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                    Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);
                    return PuzzleSceneTestHelper.AttractiveObjectYield(AttractiveObjectDefinition.AttractiveObjectOnly(InteractiveObjectTestID.TEST_1, 999999f, 0.2f), aiManager.transform.position,
                        OnAttractiveObjectSpawn: (InteractiveObjectType attractiveObjectType) =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                            Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);
                            return null;
                        },
                        OnAttractiveObjectDestroyed: () =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                            Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);
                            return null;
                        });
                },
                OnDistanceReached: () =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                    Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                    Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
                    return null;
                },
                OnDistanceReaderAIObjectType: () => aiManager);
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_Nominal_Test()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            float attractiveObjectEffectiveTime = .1f;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            yield return null;
            var attractiveObjectType = PuzzleSceneTestHelper.SpawnAttractiveObject(AttractiveObjectDefinition.AttractiveObjectOnly(InteractiveObjectTestID.TEST_1, 999999f, attractiveObjectEffectiveTime), TestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>(), "The AI has been attracted, no more patrolling.");
            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>(), "The AI is being attracted.");
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
            yield return new WaitForSeconds(attractiveObjectEffectiveTime);
            Assert.IsNull(attractiveObjectType);
        }

        //Ai is following attractive object position
        [UnityTest]
        public IEnumerator AI_AttractiveObject_OnStay_AIFollowPosition()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            var attractiveObjectType = PuzzleSceneTestHelper.SpawnAttractiveObject(AttractiveObjectDefinition.AttractiveObjectOnly(InteractiveObjectTestID.TEST_1, 999999f, 1f), aiManager.transform.position);
            yield return new WaitForFixedUpdate();
            yield return null; //Wait for AI to set the destination
            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
            Assert.AreEqual(attractiveObjectType.transform.position, aiManager.GetAgent().destination);
            attractiveObjectType.transform.position = attractiveObjectType.transform.position + Vector3.forward;
            yield return new WaitForFixedUpdate();
            yield return null; //Wait for AI to set the destination
            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
            Assert.AreEqual(attractiveObjectType.transform.position, aiManager.GetAgent().destination);
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_InterruptedBy_Projectile_Test()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(InteractiveObjectTestID.TEST_1, aIObjectInitialization, 170f);
            yield return PuzzleSceneTestHelper.AttractiveObjectYield(AttractiveObjectDefinition.AttractiveObjectOnly(InteractiveObjectTestID.TEST_1, 999999f, 0.2f), aiManager.transform.position,
                OnAttractiveObjectSpawn: (InteractiveObjectType attractiveObject) =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
                    Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
                    return PuzzleSceneTestHelper.ProjectileYield(ProjectileInteractiveObjectDefinitions.ExplodingProjectile(InteractiveObjectTestID.TEST_1, 99999f, 30f), aiManager.transform.position,
                        OnProjectileSpawn: (InteractiveObjectType projectile) =>
                        {
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
                            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);
                            return null;
                        },
                        OnDistanceReached: () =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>(), "The attractive object is stille living and in range. AttractiveObject_TriggerStay -> attracted = true.");
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
                            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
                            return null;
                        },
                        OnDistanceReaderAIObjectType: () => aiManager
                   );
                },
                OnAttractiveObjectDestroyed: () =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
                    Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
                    return null;
                }
                );
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_InterruptedBy_TargetZone_Test()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            yield return PuzzleSceneTestHelper.AttractiveObjectYield(AttractiveObjectDefinition.AttractiveObjectOnly(InteractiveObjectTestID.TEST_1, 999999f, 0.2f), aiManager.transform.position,
               OnAttractiveObjectSpawn: (InteractiveObjectType attractiveObject) =>
               {
                   Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                   Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                   Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
                   return PuzzleSceneTestHelper.TargetZoneYield(new TargetZoneInherentData(0.1f, 170), aiManager.transform.position,
                       OnTargetZoneSpawn: (InteractiveObjectType targetZone) =>
                       {
                           Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                           Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                           Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);
                           return null;
                       },
                       OnDistanceReached: () =>
                       {
                           Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                           Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                           Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
                           return null;
                       },
                       OnDistanceReachedAIObjectType: () => aiManager
                       );
               },
               OnAttractiveObjectDestroyed: () =>
               {
                   Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                   Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                   Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
                   return null;
               }
               );
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_InterruptedBy_Fear_Test()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_3);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            var fearTime = 0.05f;
            aIObjectInitialization.AIObjectTypeDefinitionInherentData.GenericPuzzleAIComponents.GetDefinitionModule<AIFearStunComponent>().TimeWhileBeginFeared = fearTime;
            yield return PuzzleSceneTestHelper.AttractiveObjectYield(AttractiveObjectDefinition.AttractiveObjectOnly(InteractiveObjectTestID.TEST_1, 999999f, 0.2f), aiManager.transform.position,
             OnAttractiveObjectSpawn: (InteractiveObjectType attractiveObject) =>
             {
                 Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                 Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
                 Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);

                 return PuzzleSceneTestHelper.FearYield(aiManager.transform.position, InteractiveObjectTestID.TEST_2, aIObjectInitialization,
                     OnFearTriggered: () =>
                     {
                         Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                         Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
                         Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);
                         return null;
                     },
                     OnFearEnded: () =>
                     {
                         Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                         Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
                         Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
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
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();

            Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
            yield return PuzzleSceneTestHelper.AttractiveObjectYield(AttractiveObjectDefinition.AttractiveObjectOnly(InteractiveObjectTestID.TEST_1, 999999f, 999f), aiManager.transform.position,
            OnAttractiveObjectSpawn: (InteractiveObjectType attractiveObject) =>
            {
                Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);

                PuzzleSceneTestHelper.SetPlayerEscapeComponentValues(aIObjectInitialization, 10f, 90f, 1f);

                return PuzzleSceneTestHelper.EscapeFromPlayerYield(playerManager, aiManager,
                    OnBeforeSettingPosition: null,
                    OnSamePositionSetted: () =>
                    {
                        Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                        Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                        Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                        Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);
                        PuzzleSceneTestHelper.SetPlayerEscapeComponentValues(aIObjectInitialization, 0f, 0f, 1f);
                        return null;
                    },
                    OnDestinationReached: () =>
                    {
                        //  yield return new WaitForFixedUpdate();
                        Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                        Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                        Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                        Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
                        return null;
                    },
                    OnDestinationReachedAIObjectType: () => aiManager
                    );
            },
            OnAttractiveObjectDestroyed: null
            );
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_WhenAIGetsOutOfRangeToReachIt_ThenStillAttracted_Test()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            float attractiveRange = 10f;
            yield return PuzzleSceneTestHelper.AttractiveObjectYield(AttractiveObjectDefinition.AttractiveObjectOnly(InteractiveObjectTestID.TEST_1, 999999f, 0.2f), aiManager.transform.position,
                OnAttractiveObjectSpawn: (InteractiveObjectType attractiveObjectType) =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                    TestHelperMethods.SetAgentPosition(aiManager.GetAgent(), aiManager.transform.position + new Vector3(attractiveRange * 2, 0f, 0f));
                    return null;
                },
                OnAttractiveObjectDestroyed: null);
            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_WithSmallRange_NoInfluence_Test()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            yield return null;
            var attractiveObjectType = PuzzleSceneTestHelper.SpawnAttractiveObject(AttractiveObjectDefinition.AttractiveObjectOnly(InteractiveObjectTestID.TEST_1, 0.001f, 99f), TestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>(), "The AI is too far from attractive object zone.");
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_WhenEnterInRange_Test()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            yield return null;
            var attractiveObjectType = PuzzleSceneTestHelper.SpawnAttractiveObject(AttractiveObjectDefinition.AttractiveObjectOnly(InteractiveObjectTestID.TEST_1, 0.01f, 99f), TestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>(), "The AI is too far from attractive object zone.");
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
            yield return new WaitForEndOfFrame(); //We wait to the end of frame in order to not take into account the natural movement of AI
            attractiveObjectType.transform.position = aiManager.transform.position;
            yield return new WaitForFixedUpdate();
            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>(), "The AI has entered the attractive object zone.");
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_WhenDestinationReached_AndObjectStillUp_Test()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            yield return null;
            var attractiveObjectType = PuzzleSceneTestHelper.SpawnAttractiveObject(AttractiveObjectDefinition.AttractiveObjectOnly(InteractiveObjectTestID.TEST_1, 99999f, 99f), TestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
            aiManager.GetAgent().Warp(aiManager.GetAgent().destination);
            yield return null;
            yield return new WaitForEndOfFrame();
            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>(), "The AI must still be attracted by object at the end of frame.");
            Assert.AreEqual(attractiveObjectType.transform.position.x, aiManager.GetAgent().destination.x, "The AI must have the target position to the attractive object.");
            Assert.AreEqual(attractiveObjectType.transform.position.z, aiManager.GetAgent().destination.z, "The AI must have the target position to the attractive object.");
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_WhenOccludedByObstacle_MustNotAttract_Test()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZoneObstacles, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            TestHelperMethods.SetAgentPosition(aiManager.GetAgent(), PuzzleSceneTestHelper.FindTestPosition(TestPositionID.OBSTACLE_LISTENER_POSITION_1).transform.position);
            yield return null;
            var attractiveObjectType = PuzzleSceneTestHelper.SpawnAttractiveObject(AttractiveObjectDefinition.AttractiveObjectOnly(InteractiveObjectTestID.TEST_1, 9999f, 99f), TestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>(), "The AI must not be attracted by attractive object because it is occluded.");
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
            TestHelperMethods.SetAgentPosition(aiManager.GetAgent(), PuzzleSceneTestHelper.FindTestPosition(TestPositionID.OBSTACLE_LISTENER_POSITION_2).transform.position);
            yield return new WaitForFixedUpdate();
            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>(), "The AI must be attracted by attractive object because it is not occluded.");
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_WhenOccludedByObstacle_MustNotAttract_PreciseCalculation_Test()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZoneObstacles, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            TestHelperMethods.SetAgentPosition(aiManager.GetAgent(), PuzzleSceneTestHelper.FindTestPosition(TestPositionID.OBSTACLE_LISTENER_POSITION_1).transform.position);
            yield return null;
            var attractiveObjectType = PuzzleSceneTestHelper.SpawnAttractiveObject(AttractiveObjectDefinition.AttractiveObjectOnly(InteractiveObjectTestID.TEST_1, 99999f, 99f), TestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>(), "The AI must not be attracted by attractive object because it is occluded.");
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
            TestHelperMethods.SetAgentPosition(aiManager.GetAgent(), PuzzleSceneTestHelper.FindTestPosition(TestPositionID.OBSTACLE_LISTENER_POSITION_3).transform.position);
            yield return new WaitForFixedUpdate();
            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>(), "The AI must be attracted by attractive object because it is not occluded. (Only AI transform position is taken into account.)");
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
        }

        #region AI_ProjectileReceived_WhenDestinationReached_WhenThereIsStillEscapeDistanceToTravel

        private IEnumerator SetupProjectileReceived_ThenDestinationReached_WhenThereIsStillEscapeDistanceToTravel(string sceneId, InteractiveObjectTestID projectileInteractiveTestID,
                        AIObjectInitialization AIObjectInitialization, Action<AIObjectType> objectDynamicInstancesCreation)
        {
            AIObjectType aiManager = null;
            yield return this.Before(sceneId, () =>
            {
                aiManager = AIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
                if (objectDynamicInstancesCreation != null) { objectDynamicInstancesCreation.Invoke(aiManager); }
            });
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(projectileInteractiveTestID, AIObjectInitialization, 179.5f);
            //high projectile escape distance
            PuzzleSceneTestHelper.SetAIEscapeDistanceFromProjectile(projectileInteractiveTestID, AIObjectInitialization, 9999f);
            yield return null;
            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>(), "The AI has no interaction -> Patrolling.");
            //wide angle to avoid navmesh ray cast to hit the same point.
            var projectileData = ProjectileInteractiveObjectDefinitions.ExplodingProjectile(projectileInteractiveTestID, 99999f, 30f);
            var lpTest = PuzzleSceneTestHelper.SpawnProjectile(projectileData, aiManager.transform.position);
            yield return new WaitForFixedUpdate();
            //projectile taken into account
            yield return new WaitForEndOfFrame();
            //move agent to a near position
            var agent = aiManager.GetAgent();
            TestHelperMethods.SetAgentDestinationPositionReached(agent);
            yield return null; //trigger SetDestination();
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_WhenDestinationReached_WhenThereIsStillEscapeDistanceToTravel_StillEscape_Test()
        {
            AIObjectInitialization aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2);
            AIObjectType aiManager = null;
            yield return this.SetupProjectileReceived_ThenDestinationReached_WhenThereIsStillEscapeDistanceToTravel(SceneConstants.OneAINoTargetZone, InteractiveObjectTestID.TEST_1, aIObjectInitialization,
                    (instacniatedAIManager) => aiManager = instacniatedAIManager);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>(), "The AI should still escape from projectile when destination has been reached one time but there is still distance to travel.");
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);
        }
        [UnityTest]
        public IEnumerator AI_ProjectileReceived_WhenDestinationReached_WhenThereIsStillEscapeDistanceToTravel_NoInteruptionByAttractive_Test()
        {
            AIObjectInitialization aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2);
            AIObjectType aiManager = null;
            yield return this.SetupProjectileReceived_ThenDestinationReached_WhenThereIsStillEscapeDistanceToTravel(SceneConstants.OneAINoTargetZone, InteractiveObjectTestID.TEST_1, aIObjectInitialization,
                (instacniatedAIManager) => aiManager = instacniatedAIManager);
            var attractiveObjectType = PuzzleSceneTestHelper.SpawnAttractiveObject(AttractiveObjectDefinition.AttractiveObjectOnly(InteractiveObjectTestID.TEST_1, 99999f, 99f), TestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
            yield return new WaitForFixedUpdate();
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>(), "The AI should still escape from projectile when destination has been reached one time but there is still distance to travel. And an attractive object has spawn in range.");
            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>(), "");
            Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_SecondTimeInTargetZone_WhenDestinationReached_WhenThereIsStillEscapeDistanceToTravel_StillEscape_Test()
        {
            AIObjectInitialization AIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_3);
            AIObjectType aiManager = null;
            yield return this.AI_ProjectileReceived_SecondTime_EscapeWithoutTarget_DestinationInTargetZone(SceneConstants.OneAIForcedTargetZone, 9999f,
                    AIObjectInitialization, InteractiveObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2, () =>
            {
                aiManager = AIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
                LevelCompletionTriggerDefinition.LevelCOmpletionTargetZone(InteractiveObjectTestID.TEST_2, Vector3.zero, new Vector3(25.29f, 12.17f, 12.4f), 17.8f, 110f).Instanciate(new Vector3(-36.08f, 0.5f, -4.4f));
            });
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(InteractiveObjectTestID.TEST_1, AIObjectInitialization, 100f);
            TestHelperMethods.SetAgentDestinationPositionReached(aiManager.GetAgent());
            Debug.Log("Destination before AI update : " + aiManager.GetAgent().destination);
            yield return null;
            Debug.Log("Destination after AI update : " + aiManager.GetAgent().destination);
            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>(), "The AI should ignore target zones while escaping from projectile for the second time.");
            Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);
            //second projectile hit when destination has been reached one time but there is still distance
            var projectileData = ProjectileInteractiveObjectDefinitions.ExplodingProjectile(InteractiveObjectTestID.TEST_1, 99999f, 30f);
            var currentFOVSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
            Debug.Log(MyLog.Format(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum()));
            PuzzleSceneTestHelper.SpawnProjectile(projectileData, TestPositionID.PROJECTILE_TARGET_2);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame(); //wait for destination position to update
            var targetZoneCollider = PuzzleSceneTestHelper.FindTargetZone(InteractiveObjectTestIDTree.InteractiveObjectTestIDs[InteractiveObjectTestID.TEST_2].TargetZoneID).ILevelCompletionTriggerModuleDataRetriever.GetTargetZoneTriggerCollider();
            Assert.IsTrue(targetZoneCollider.bounds.Contains(aiManager.GetAgent().destination), "AI Destination should contains the target zone the second (or more) hit.");
            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>(), "The AI should ignore target zones while escaping from projectile for the second (or more) time.");
            Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < currentFOVSum);
        }
        #endregion

        [UnityTest]
        public IEnumerator AI_TargetZone_WhenAIIsNearTargetZone_AiFOVShoudlBeReducedBasedOnTargetZoneBoundCenter_Test()
        {
            var targetEscapeSemiAngle = 110f;
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAIForcedTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_3);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
                LevelCompletionTriggerDefinition.LevelCOmpletionTargetZone(InteractiveObjectTestID.TEST_2, Vector3.zero, new Vector3(25.29f, 12.17f, 12.4f), 17.8f, targetEscapeSemiAngle).Instanciate(new Vector3(-36.08f, 0.5f, -4.4f));
            });
            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            var targetZoneCollider = PuzzleSceneTestHelper.FindTargetZone(InteractiveObjectTestIDTree.InteractiveObjectTestIDs[InteractiveObjectTestID.TEST_2].TargetZoneID).ILevelCompletionTriggerModuleDataRetriever.GetTargetZoneTriggerCollider();
            var gameConfiguration = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var aiPosition = targetZoneCollider.transform.position + new Vector3(0, 0, 0.1f);
            TestHelperMethods.SetAgentDestinationPositionReached(aiManager, aiPosition);
            yield return null;
            var aiFov = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOV();
            Assert.AreEqual(new StartEndSlice(360f - targetEscapeSemiAngle, 360f), aiFov.FovSlices[0]);
            Assert.AreEqual(new StartEndSlice(0f, targetEscapeSemiAngle), aiFov.FovSlices[1]);
        }

        [UnityTest]
        public IEnumerator AI_TargetZone_WhenAIExitTargetZone_WhenThereIsStillEscapeDistanceToTravel_StillEscape()
        {
            var targetEscapeSemiAngle = 110f;
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAIForcedHighDistanceTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_3);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
                LevelCompletionTriggerDefinition.LevelCOmpletionTargetZone(InteractiveObjectTestID.TEST_1, Vector3.zero, new Vector3(0.5f, 0.5f, 0.5f), 17.8f, targetEscapeSemiAngle).Instanciate(new Vector3(-36.08f, 0.5f, -8.46f));
            });
            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            var targetZoneCollider = PuzzleSceneTestHelper.FindTargetZone(InteractiveObjectTestIDTree.InteractiveObjectTestIDs[InteractiveObjectTestID.TEST_1].TargetZoneID).ILevelCompletionTriggerModuleDataRetriever.GetTargetZoneTriggerCollider();
            var gameConfiguration = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var aiPosition = targetZoneCollider.transform.position + new Vector3(-0.1f, 0f, 0f);
            TestHelperMethods.SetAgentDestinationPositionReached(aiManager, aiPosition);
            yield return null; //AI is inside the target zone
            TestHelperMethods.SetAgentDestinationPositionReached(aiManager, PuzzleSceneTestHelper.FindTestPosition(TestPositionID.FAR_AWAY_POSITION_1).position);
            yield return null;
            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>(), "AI should still escape as there are distance to cross.");
            Assert.AreNotEqual(new StartEndSlice(0f, 360f), aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOV().FovSlices[0], "AI fov must not be resetted after destination reached and still distance to cross.");
            TestHelperMethods.SetAgentDestinationPositionReached(aiManager.GetAgent());
            yield return null;
            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>(), "AI should stop escape when destination is reached.");
            Assert.AreEqual(new StartEndSlice(0f, 360f), aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOV().FovSlices[0]);
        }

        [UnityTest]
        public IEnumerator AI_TargetZone_InterruptedBy_Projectile_TargetZoneOnEnterEvent()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var interactiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(InteractiveObjectTestID.TEST_1, aIObjectInitialization, 170f);
            yield return PuzzleSceneTestHelper.TargetZoneYield(new TargetZoneInherentData(9999, 170), aiManager.transform.position,
                OnTargetZoneSpawn: (InteractiveObjectType targetZone) =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                    Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);
                    var currentFOVAngleSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
                    //Target zone is destroyed to simulate the fact that the AI is exiting the target zone
                    interactiveObjectContainer.TEST_OnInteractiveObjectDestroyed(targetZone);
                    return PuzzleSceneTestHelper.ProjectileYield(ProjectileInteractiveObjectDefinitions.ExplodingProjectile(InteractiveObjectTestID.TEST_1, 1000f, 1), aiManager.transform.position,
                        OnProjectileSpawn: (InteractiveObjectType projectile) =>
                        {
                            Assert.AreEqual(1, this.MockPuzzleEventsManagerTest.AiHittedByProjectileCallCount);
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                            Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < currentFOVAngleSum);
                            return null;
                        },
                        OnDistanceReached: () =>
                        {
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                            Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
                            return null;
                        },
                        OnDistanceReaderAIObjectType: () => aiManager);
                },
                OnDistanceReached: null,
                OnDistanceReachedAIObjectType: null);
        }

        [UnityTest]
        public IEnumerator AI_TargetZone_NotInterruptedBy_Projectile_TargetZoneOnStayEvent()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(InteractiveObjectTestID.TEST_1, aIObjectInitialization, 170f);
            yield return PuzzleSceneTestHelper.TargetZoneYield(new TargetZoneInherentData(9999, 170), aiManager.transform.position,
                OnTargetZoneSpawn: (InteractiveObjectType targetZone) =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                    Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);
                    var currentFOVAngleSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
                    return PuzzleSceneTestHelper.ProjectileYield(ProjectileInteractiveObjectDefinitions.ExplodingProjectile(InteractiveObjectTestID.TEST_1, 1000f, 1), aiManager.transform.position,
                        OnProjectileSpawn: (InteractiveObjectType projectile) =>
                        {
                            Assert.AreEqual(1, this.MockPuzzleEventsManagerTest.AiHittedByProjectileCallCount);
                            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                            Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);

                            //Event if not interrupted by projectile, FOV is still reduced to take the projectile into account.
                            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < currentFOVAngleSum);
                            currentFOVAngleSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
                            return null;
                        },
                        OnDistanceReached: () =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                            Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() != currentFOVAngleSum); //The FOV have been resetted, then recalculated by target zone stay
                            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);
                            return null;
                        },
                        OnDistanceReaderAIObjectType: () => aiManager);
                },
                OnDistanceReached: null,
                OnDistanceReachedAIObjectType: null);
        }

        [UnityTest]
        public IEnumerator AI_TargetZone_InterruptedBy_EscapeWithoutTarget()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var interactiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(InteractiveObjectTestID.TEST_1, aIObjectInitialization, 90f);
            aIObjectInitialization.AIObjectTypeDefinitionInherentData.GenericPuzzleAIComponents.GetDefinitionModule<AIFearStunComponent>().FOVSumThreshold = 0f; //prenvenitng
            yield return PuzzleSceneTestHelper.TargetZoneYield(new TargetZoneInherentData(9999, 170), aiManager.transform.position,
                OnTargetZoneSpawn: (InteractiveObjectType targetZone) =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
                    Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                    Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);
                    var currentFOVnalgeSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
                    //Target zone is destroyed to simulate the fact that the AI is exiting the target zone
                    interactiveObjectContainer.TEST_OnInteractiveObjectDestroyed(targetZone);
                    //     MonoBehaviour.DestroyImmediate(targetZone.gameObject);
                    return PuzzleSceneTestHelper.ProjectileIngoreTargetYield(ProjectileInteractiveObjectDefinitions.ExplodingProjectile(InteractiveObjectTestID.TEST_1, 1000, 1), aiManager.transform.position,
                        OnBeforeSecondProjectileSpawn: () =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
                            Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < currentFOVnalgeSum);
                            currentFOVnalgeSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
                            return null;
                        },
                        OnSecondProjectileSpawned: (InteractiveObjectType projectile) =>
                        {
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
                            Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < currentFOVnalgeSum);
                            currentFOVnalgeSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
                            return null;
                        },
                        OnSecondProjectileDistanceReached: null,
                        OnSecondProjectileDistanceReachedAIObjectType: null);
                },
                OnDistanceReached: null,
                OnDistanceReachedAIObjectType: null
            );
        }

        [UnityTest]
        public IEnumerator AI_TargetZone_InterruptedBy_PlayerEscape()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();

            yield return PuzzleSceneTestHelper.TargetZoneYield(new TargetZoneInherentData(9999, 170), aiManager.transform.position,
               OnTargetZoneSpawn: (InteractiveObjectType targetZone) =>
               {
                   Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                   Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                   Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                   Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);

                   var currentFOVAngleSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
                   PuzzleSceneTestHelper.SetPlayerEscapeComponentValues(aIObjectInitialization, 1, 90f, 9999f);
                   return PuzzleSceneTestHelper.EscapeFromPlayerYield(playerManager, aiManager,
                       OnBeforeSettingPosition: null,
                       OnSamePositionSetted: () =>
                       {
                           Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                           Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                           Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                           Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < currentFOVAngleSum);

                           currentFOVAngleSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
                           PuzzleSceneTestHelper.SetPlayerEscapeComponentValues(aIObjectInitialization, 1, 90f, 0f);
                           return null;
                       },
                       OnDestinationReached: () =>
                       {
                           Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                           Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                           Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);

                           Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() != currentFOVAngleSum); //The FOV have been resetted, then recalculated by target zone stay
                           Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);
                           return null;
                       },
                       OnDestinationReachedAIObjectType: () => aiManager);

               },
               OnDistanceReached: null,
               OnDistanceReachedAIObjectType: null
           );
        }

        [UnityTest]
        public IEnumerator AI_TargetZone_NotInterruptedBy_Attractive()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            var interactiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();
            yield return PuzzleSceneTestHelper.TargetZoneYield(new TargetZoneInherentData(1, 170), aiManager.transform.position,
                OnTargetZoneSpawn: (InteractiveObjectType targetZone) =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                    Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);

                    interactiveObjectContainer.TEST_OnInteractiveObjectDestroyed(targetZone);

                    var currentFOVAngleSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
                    return PuzzleSceneTestHelper.AttractiveObjectYield(AttractiveObjectDefinition.AttractiveObjectOnly(InteractiveObjectTestID.TEST_1, 99999f, 99f), aiManager.transform.position,
                        OnAttractiveObjectSpawn: (InteractiveObjectType attractiveObject) =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == currentFOVAngleSum);
                            return null;
                        },
                        OnAttractiveObjectDestroyed: null);
                },
                OnDistanceReached: () =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                    Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
                    return null;
                },
                OnDistanceReachedAIObjectType: () => aiManager);
        }

        [UnityTest]
        public IEnumerator AI_FearStun_WhenReceivingAProjectile_IsFeared()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });

            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(InteractiveObjectTestID.TEST_1, aIObjectInitialization, 0.5f);
            var fearTime = 0.05f;
            aIObjectInitialization.AIObjectTypeDefinitionInherentData.GenericPuzzleAIComponents.GetDefinitionModule<AIFearStunComponent>().TimeWhileBeginFeared = fearTime;

            var projectileData = ProjectileInteractiveObjectDefinitions.ExplodingProjectile(InteractiveObjectTestID.TEST_1, 99999f, 30f);
            var projectileSpanwed = PuzzleSceneTestHelper.SpawnProjectile(projectileData, aiManager.transform.position);

            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
            yield return new WaitForSeconds(fearTime);
            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
        }

        [UnityTest]
        public IEnumerator AI_FearStun_OnForcedFear_IsFeared()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_2);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(InteractiveObjectTestID.TEST_1, aIObjectInitialization, 1f);
            aiManager.GetAgent().Warp(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.PITFAL_Z_POSITION_FAR_EDGE).transform.position);
            aIObjectInitialization.AIObjectTypeDefinitionInherentData.GenericPuzzleAIComponents.GetDefinitionModule<AIFearStunComponent>().FOVSumThreshold = 0f;
            PuzzleSceneTestHelper.SetAIEscapeDistanceFromProjectile(InteractiveObjectTestID.TEST_1, aIObjectInitialization, 5f);
            yield return null;
            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
            yield return PuzzleSceneTestHelper.ProjectileYield(ProjectileInteractiveObjectDefinitions.ExplodingProjectile(InteractiveObjectTestID.TEST_1, 99999f, 30f), aiManager.transform.position - Vector3.forward,
                OnProjectileSpawn: (InteractiveObjectType lauinchProjectile) =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
                    Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);
                    return null;
                },
                OnDistanceReached: null,
                OnDistanceReaderAIObjectType: null
                );
        }

        [UnityTest]
        public IEnumerator AI_FearStun_NotInterruptedBy_Attractive()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_3);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();

            var fearTime = 0.3f;
            aIObjectInitialization.AIObjectTypeDefinitionInherentData.GenericPuzzleAIComponents.GetDefinitionModule<AIFearStunComponent>().TimeWhileBeginFeared = fearTime;
            yield return PuzzleSceneTestHelper.FearYield(aiManager.transform.position, InteractiveObjectTestID.TEST_2, aIObjectInitialization,
                OnFearTriggered: () =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                    return PuzzleSceneTestHelper.AttractiveObjectYield(AttractiveObjectDefinition.AttractiveObjectOnly(InteractiveObjectTestID.TEST_1, 999f, 0.1f), aiManager.transform.position,
                        OnAttractiveObjectSpawn: (InteractiveObjectType attractiveObject) =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                            return null;
                        },
                        OnAttractiveObjectDestroyed: () =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                            return null;
                        }
                        );
                },
                OnFearEnded: () =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>());
                    return null;
                },
               fearTime,
               mouseAIBheavior
             );
        }

        [UnityTest]
        public IEnumerator AI_FearStun_NotInterruptedBy_Projectile()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_3);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();

            var fearTime = 0.1f;
            aIObjectInitialization.AIObjectTypeDefinitionInherentData.GenericPuzzleAIComponents.GetDefinitionModule<AIFearStunComponent>().TimeWhileBeginFeared = fearTime;
            yield return PuzzleSceneTestHelper.FearYield(aiManager.transform.position, InteractiveObjectTestID.TEST_2, aIObjectInitialization,
                OnFearTriggered: () =>
                {
                    PuzzleSceneTestHelper.SetAIEscapeSemiAngle(InteractiveObjectTestID.TEST_1, aIObjectInitialization, 170f);

                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                    Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                    return PuzzleSceneTestHelper.ProjectileYield(ProjectileInteractiveObjectDefinitions.ExplodingProjectile(InteractiveObjectTestID.TEST_1, 9999f, 1), aiManager.transform.position,
                        OnProjectileSpawn: (InteractiveObjectType projectile) =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                            Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                            return null;
                        },
                        OnDistanceReached: null,
                        OnDistanceReaderAIObjectType: null
                    );
                },
                OnFearEnded: () =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
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
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_3);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();

            var fearTime = 0.1f;
            aIObjectInitialization.AIObjectTypeDefinitionInherentData.GenericPuzzleAIComponents.GetDefinitionModule<AIFearStunComponent>().TimeWhileBeginFeared = fearTime;
            yield return PuzzleSceneTestHelper.FearYield(aiManager.transform.position, InteractiveObjectTestID.TEST_2, aIObjectInitialization,
                OnFearTriggered: () =>
                {
                    PuzzleSceneTestHelper.SetAIEscapeSemiAngle(InteractiveObjectTestID.TEST_1, aIObjectInitialization, 170f);

                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
                    Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                    return PuzzleSceneTestHelper.ProjectileIngoreTargetYield(ProjectileInteractiveObjectDefinitions.ExplodingProjectile(InteractiveObjectTestID.TEST_1, 999f, 1), aiManager.transform.position,
                       OnBeforeSecondProjectileSpawn: null,
                       OnSecondProjectileSpawned: (InteractiveObjectType projectile) =>
                       {
                           Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
                           Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
                           Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                           return null;
                       },
                       OnSecondProjectileDistanceReached: null,
                       OnSecondProjectileDistanceReachedAIObjectType: null
                       );
                },
                OnFearEnded: () =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
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
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_3);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();

            var fearTime = 0.2f;
            aIObjectInitialization.AIObjectTypeDefinitionInherentData.GenericPuzzleAIComponents.GetDefinitionModule<AIFearStunComponent>().TimeWhileBeginFeared = fearTime;
            yield return PuzzleSceneTestHelper.FearYield(aiManager.transform.position, InteractiveObjectTestID.TEST_2, aIObjectInitialization,
                OnFearTriggered: () =>
                {
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                    return PuzzleSceneTestHelper.TargetZoneYield(new TargetZoneInherentData(9999f, 170), aiManager.transform.position,
                        OnTargetZoneSpawn: (InteractiveObjectType targetZone) =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                            return null;
                        },
                        OnDistanceReached: null,
                        OnDistanceReachedAIObjectType: null
                    );
                },
                OnFearEnded: () =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                    return null;
                },
                fearTime,
                mouseAIBheavior
             );
        }

        [UnityTest]
        public IEnumerator AI_PlayerEscape_EscapeFromPlayerWhenInRange()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_3);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();

            PuzzleSceneTestHelper.SetPlayerEscapeComponentValues(aIObjectInitialization, 10f, 90f, 0.1f);

            yield return PuzzleSceneTestHelper.EscapeFromPlayerYield(playerManager, aiManager,
                   OnBeforeSettingPosition: () =>
                   {
                       Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                       Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>());
                       Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                       Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
                       return null;
                   },
                   OnSamePositionSetted: () =>
                   {
                       Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                       Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>());
                       Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                       Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);
                       return null;
                   },
                   OnDestinationReached: () =>
                   {
                       Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                       Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>());
                       Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                       Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
                       return null;
                   },
                   OnDestinationReachedAIObjectType: () => aiManager);
        }

        [UnityTest]
        public IEnumerator AI_PlayerEscape_EscapeFromPlayerWhenInRange_StillEscapeAfterDestinationReached()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_3);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            var currentFOVAngleSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();

            PuzzleSceneTestHelper.SetPlayerEscapeComponentValues(aIObjectInitialization, 1f, 90f, 0.1f);

            yield return PuzzleSceneTestHelper.EscapeFromPlayerYield(playerManager, aiManager,
                   OnBeforeSettingPosition: () =>
                   {
                       Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                       Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>());
                       Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                       Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
                       return null;
                   },
                   OnSamePositionSetted: () =>
                   {
                       Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                       Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>());
                       Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                       Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);

                       currentFOVAngleSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
                       PuzzleSceneTestHelper.SetPlayerEscapeComponentValues(aIObjectInitialization, 1f, 90f, 99999f);
                       return null;
                   },
                   OnDestinationReached: () =>
                   {
                       Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                       Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>());
                       Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                       Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == currentFOVAngleSum);
                       return null;
                   },
                   OnDestinationReachedAIObjectType: () => aiManager);
        }

        [UnityTest]
        public IEnumerator AI_PlayerEscape_EscapeFromPlayerWhenInRange_WhenThereIsNoAvailableDestination_BeFeared()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_3);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();
            var playerDataRetriver = GameObject.FindObjectOfType<PlayerManagerDataRetriever>();
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();

            PuzzleSceneTestHelper.SetPlayerEscapeComponentValues(aIObjectInitialization, 1f, 20f, 1);
            aiManager.GetAgent().Warp(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.PITFAL_Z_POSITION_1).transform.position);
            playerDataRetriver.GetPlayerRigidBody().transform.position = PuzzleSceneTestHelper.FindTestPosition(TestPositionID.PITFAL_Z_POSITION_1).transform.position - (Vector3.forward * 0.5f);
            yield return null;
            playerDataRetriver.GetPlayerRigidBody().transform.position = Vector3.zero;
            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);
        }

        [UnityTest]
        public IEnumerator AI_PlayerEscape_EscapeFromPlayerWhenInRange_StillEscapeWhenThereIsStillDistanceToTravel()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_3);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            var currentFOVAngleSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();

            PuzzleSceneTestHelper.SetPlayerEscapeComponentValues(aIObjectInitialization, 99999f, 90f, 0.1f);

            yield return PuzzleSceneTestHelper.EscapeFromPlayerYield(playerManager, aiManager,
                   OnBeforeSettingPosition: () =>
                   {
                       Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                       Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>());
                       Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                       Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
                       return null;
                   },
                   OnSamePositionSetted: () =>
                   {
                       Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                       Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>());
                       Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                       Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);

                       currentFOVAngleSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
                       return null;
                   },
                   OnDestinationReached: null,
                   OnDestinationReachedAIObjectType: null);
            TestHelperMethods.SetAgentDestinationPositionReached(aiManager, aiManager.transform.position + (Vector3.forward * 10f));
            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIPatrolComponentManager>());
            Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == currentFOVAngleSum);
        }

        [UnityTest]
        public IEnumerator AI_PlayerEscape_EscapeFromPlayerWhenInRange_WhenEscapingWithoutTargetZones_DestinationIntoTargetZone()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAIForcedTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_3);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
                LevelCompletionTriggerDefinition.LevelCOmpletionTargetZone(InteractiveObjectTestID.TEST_2, Vector3.zero, new Vector3(25.29f, 12.17f, 12.4f), 17.8f, 110f).Instanciate(new Vector3(-36.08f, 0.5f, -4.4f));
            });
            yield return null;

            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();
            var targetZoneCollider = PuzzleSceneTestHelper.FindTargetZone(InteractiveObjectTestIDTree.InteractiveObjectTestIDs[InteractiveObjectTestID.TEST_2].TargetZoneID).ILevelCompletionTriggerModuleDataRetriever.GetTargetZoneTriggerCollider();
            var currentFOVAngleSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(InteractiveObjectTestID.TEST_1, aIObjectInitialization, 90f);

            PuzzleSceneTestHelper.SetPlayerEscapeComponentValues(aIObjectInitialization, 99999f, 90f, 0.2f);
            playerManager.transform.position = new Vector3(999, 999, 999);
            PuzzleSceneTestHelper.SetAIEscapeDistanceFromProjectile(InteractiveObjectTestID.TEST_1, aIObjectInitialization, 999f);

            yield return PuzzleSceneTestHelper.EscapeFromPlayerIgnoreTargetYield(playerManager, aiManager,
                ProjectileInteractiveObjectDefinitions.ExplodingProjectile(InteractiveObjectTestID.TEST_1, 99999f, 30f), aiManager.transform.position + (Vector3.right * 0.1f),
                OnBeforeSettingPosition: () =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                    Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);

                    currentFOVAngleSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
                    playerManager.transform.position = aiManager.transform.position + Vector3.right;
                    return null;
                },
                OnSamePositionSetted: () =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>());
                    Assert.IsTrue(targetZoneCollider.bounds.Contains(aiManager.GetAgent().destination));
                    Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < currentFOVAngleSum);
                    return null;
                },
                OnDestinationReached: null);
        }

        [UnityTest]
        public IEnumerator AI_PlayerEscape_InterruptedBy_TargetZone()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_3);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            var currentFOVAngleSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();

            PuzzleSceneTestHelper.SetPlayerEscapeComponentValues(aIObjectInitialization, 10f, 90f, 1f);

            yield return PuzzleSceneTestHelper.EscapeFromPlayerYield(playerManager, aiManager,
                  OnBeforeSettingPosition: () =>
                  {
                      Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                      Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                      Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                      Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
                      currentFOVAngleSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
                      return null;
                  },
                  OnSamePositionSetted: () =>
                  {
                      Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                      Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                      Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                      Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);
                      currentFOVAngleSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
                      return PuzzleSceneTestHelper.TargetZoneYield(new TargetZoneInherentData(9999f, 170), aiManager.transform.position,
                          OnTargetZoneSpawn: (InteractiveObjectType targetZone) =>
                          {
                              Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                              Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAITargetZoneManager>());
                              Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                              //AI FOV is resetted when playerescape is interrupted by target zone
                              Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() > currentFOVAngleSum);
                              Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);
                              return null;
                          },
                          OnDistanceReached: null,
                          OnDistanceReachedAIObjectType: null);
                  },
                  OnDestinationReached: null,
                  OnDestinationReachedAIObjectType: null);
        }

        [UnityTest]
        public IEnumerator AI_PlayerEscape_InterruptedBy_EscapeWithoutTarget()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_3);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            var currentFOVAngleSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(InteractiveObjectTestID.TEST_1, aIObjectInitialization, 170f);

            yield return PuzzleSceneTestHelper.ProjectileYield(ProjectileInteractiveObjectDefinitions.ExplodingProjectile(InteractiveObjectTestID.TEST_1, 999f, 999999f), aiManager.transform.position,
                OnProjectileSpawn: (InteractiveObjectType launchProjectile) =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                    Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < 360f);

                    currentFOVAngleSum = aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum();
                    PuzzleSceneTestHelper.SetPlayerEscapeComponentValues(aIObjectInitialization, 10f, 90f, 1f);
                    return PuzzleSceneTestHelper.EscapeFromPlayerYield(playerManager, aiManager,
                        OnBeforeSettingPosition: null,
                        OnSamePositionSetted: () =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() < currentFOVAngleSum);
                            return null;
                        },
                        OnDestinationReached: () =>
                        {
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                            Assert.IsTrue(aiManager.GetComponent<InteractiveObjectType>().GetModule<FovModule>().FovManager.GetFOVAngleSum() == 360f);
                            return null;
                        },
                        OnDestinationReachedAIObjectType: () => aiManager
                     );
                },
                OnDistanceReached: null,
                OnDistanceReaderAIObjectType: null);
        }

        [UnityTest]
        public IEnumerator AI_PlayerEscape_InterruptedBy_FearStun()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_3);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetPlayerEscapeComponentValues(aIObjectInitialization, 10f, 90f, 1f);
            yield return PuzzleSceneTestHelper.EscapeFromPlayerYield(playerManager, aiManager,
                OnBeforeSettingPosition: () =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
                    Assert.IsFalse(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                    return null;
                },
                OnSamePositionSetted: () =>
                {

                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
                    Assert.IsTrue(this.GetEscapeWhileIgnoringTargetZoneTracker(mouseAIBheavior).IsEscapingWhileIgnoringTargets);
                    return PuzzleSceneTestHelper.FearYield(aiManager.transform.position, InteractiveObjectTestID.TEST_2, aIObjectInitialization,
                       OnFearTriggered: () =>
                       {
                           Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractPlayerEscapeManager>());
                           Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
                           return null;
                       },
                       OnFearEnded: null, 1f, mouseAIBheavior);
                },
                OnDestinationReached: null,
                OnDestinationReachedAIObjectType: null);
        }

        [UnityTest]
        public IEnumerator AI_EscapeWithoutarget_InterruptedBy_FearStun()
        {
            AIObjectInitialization aIObjectInitialization = null;
            AIObjectType aiManager = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                aIObjectInitialization = AIObjectDefinition.SewersAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_3);
                aiManager = aIObjectInitialization.Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });
            yield return null;
            var mouseAIBheavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(InteractiveObjectTestID.TEST_1, aIObjectInitialization, 170f);
            var fearTime = 10f;
            aIObjectInitialization.AIObjectTypeDefinitionInherentData.GenericPuzzleAIComponents.GetDefinitionModule<AIFearStunComponent>().TimeWhileBeginFeared = fearTime;

            yield return PuzzleSceneTestHelper.ProjectileIngoreTargetYield(ProjectileInteractiveObjectDefinitions.ExplodingProjectile(InteractiveObjectTestID.TEST_1, 9999f, 30f), aiManager.transform.position,
                OnBeforeSecondProjectileSpawn: () =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
                    return null;
                },
                OnSecondProjectileSpawned: (InteractiveObjectType launchProjectile) =>
                {
                    Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
                    Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
                    return PuzzleSceneTestHelper.FearYield(aiManager.transform.position, InteractiveObjectTestID.TEST_2, aIObjectInitialization,
                        OnFearTriggered: () =>
                        {
                            Assert.IsTrue(mouseAIBheavior.IsManagerEnabled<AbstractAIFearStunManager>());
                            Assert.IsFalse(mouseAIBheavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>());
                            return null;
                        },
                        OnFearEnded: null, 0f, mouseAIBheavior);
                },
                OnSecondProjectileDistanceReached: null,
                OnSecondProjectileDistanceReachedAIObjectType: null);
        }
    }
}
