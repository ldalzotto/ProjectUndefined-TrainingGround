using RTPuzzle;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Tests
{
    public class AIBehaviorTest : AbstractPuzzleSceneTest
    {

        [UnityTest]
        public IEnumerator AI_RandomPatrol_Nominal_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null; // let the initialisatoin process pass
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var oldPosition = mouseTestAIManager.transform.position;
            yield return null;
            var newPosition = mouseTestAIManager.transform.position;
            Assert.AreNotEqual(newPosition, oldPosition);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            Assert.IsTrue(mouseAIBheavior.IsPatrolling());
            var beforeReachDestination = mouseTestAIManager.GetAgent().destination;
            yield return new WaitUntil(() => this.MockPuzzleEventsManagerTest.OnDestinationReachedCalled);
            this.MockPuzzleEventsManagerTest.ClearCalls();
            Assert.IsTrue(mouseAIBheavior.IsPatrolling());
            var afterReachDestination = mouseTestAIManager.GetAgent().destination;
            Assert.AreNotEqual(beforeReachDestination, afterReachDestination);
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_Nominal_Test()
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
            Assert.IsFalse(mouseAIBheavior.IsPatrolling(), "The AI has been hit, no more patrolling.");
            Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectile(), "The AI has been hit, escaping.");
            Assert.AreEqual(this.MockPuzzleEventsManagerTest.AiHittedByProjectileCallCount, 1, "The 'hitted by projectile event' must be triggered only once.");
            yield return null;
            Assert.IsNull(lpTest, "The projectile must be detroyed after hit.");
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_WhenDestinatioReached_NoMoreEscape_Test()
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
            yield return null; //trigger SetDestination();
            Assert.IsFalse(mouseAIBheavior.IsEscapingFromProjectile(), "The AI should no more escape from projectile when destination is reached.");
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
            var targetZoneCollider = PuzzleSceneTestHelper.FindTargetZone(TargetZoneID.TEST_TARGET_ZONE).ZoneCollider;
            Assert.IsFalse(targetZoneCollider.bounds.Contains(aiDestination), "AI Destination should not be in the target zone the first hit.");
        }

        private IEnumerator AI_ProjectileReceived_SecondTimeInTargetZone(string sceneName, float? projectileEscapeDistance)
        {
            yield return this.AI_ProjectileReceived_FirstTimeNotIntoTargetZone(sceneName, AiID.MOUSE_TEST);
            var launchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            if (projectileEscapeDistance != null)
            {
                ((GenericPuzzleAIComponents)mouseTestAIManager.GetAIBehavior().AIComponents).AIProjectileEscapeComponent.EscapeDistance = projectileEscapeDistance.Value;
            }
            var projectileData = PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 90f, 30f);
            var secondProj = PuzzleSceneTestHelper.SpawnProjectile(projectileData, AITestPositionID.PROJECTILE_TARGET_2, launchProjectileContainerManager);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame(); //wait for destination position to update
            var aiDestination = mouseTestAIManager.GetAgent().destination;
            var targetZoneCollider = PuzzleSceneTestHelper.FindTargetZone(TargetZoneID.TEST_TARGET_ZONE).ZoneCollider;
            Assert.IsTrue(targetZoneCollider.bounds.Contains(aiDestination), "AI Destination should contains the target zone the second hit.");
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_FirstTimeNotIntoTargetZone()
        {
            yield return this.AI_ProjectileReceived_FirstTimeNotIntoTargetZone(SceneConstants.OneAIForcedTargetZone, AiID.MOUSE_TEST);
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_SecondTimeInTargetZone()
        {
            yield return this.AI_ProjectileReceived_SecondTimeInTargetZone(SceneConstants.OneAIForcedTargetZone, null);
        }


        [UnityTest]
        public IEnumerator AI_ProjectileRecevied_NoInteruptionByAttractive_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            var launchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            var attractiveObjectInherentConfigurationData = PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(999999f, 99f);
            var projectileData = PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 90f, 30f);
            yield return null;
            PuzzleSceneTestHelper.SpawnProjectile(projectileData, mouseTestAIManager.transform.position, launchProjectileContainerManager);
            yield return new WaitForFixedUpdate();
            PuzzleSceneTestHelper.SpawnAttractiveObject(attractiveObjectInherentConfigurationData, AITestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
            Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectile());
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
        public IEnumerator AI_AttractiveObject_DisruptedByProjectile_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            var launchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            var attractiveObjectInherentConfigurationData = PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(999999f, 99f);
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return null;
            var attractiveObjectType = PuzzleSceneTestHelper.SpawnAttractiveObject(attractiveObjectInherentConfigurationData, AITestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
            yield return new WaitForFixedUpdate();
            Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject(), "The AI is being attracted.");
            var projectileData = PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 90f, 30f);
            PuzzleSceneTestHelper.SpawnProjectile(projectileData, mouseTestAIManager.transform.position, launchProjectileContainerManager);
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject(), "A projectile has hit the AI. Abort attract.");
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
            ((GenericPuzzleAIComponents)mouseAIBheavior.AIComponents).AIProjectileEscapeComponent.EscapeDistance = 9999f;
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
            Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectile(), "The AI should still escape from projectile when destination has been reached one time but there is still distance to travel.");
            //second projectile hit when destination has been reached one time but there is still distance
            var projectileData = PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 180f, 30f);
            PuzzleSceneTestHelper.SpawnProjectile(projectileData, AITestPositionID.PROJECTILE_TARGET_2, launchProjectileContainerManager);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame(); //wait for destination position to update
            Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectile(), "The AI should still escape from projectile at second hit.");
            var targetZoneCollider = PuzzleSceneTestHelper.FindTargetZone(TargetZoneID.TEST_TARGET_ZONE).ZoneCollider;
            Assert.IsTrue(targetZoneCollider.bounds.Contains(mouseTestAIManager.GetAgent().destination), "AI Destination should contains the target zone the second (or more) hit.");
        }
        #endregion


        [UnityTest]
        public IEnumerator AI_TargetZone_WhenAIIsNearTargetZone_ShouldEscape_Test()
        {
            yield return this.Before(SceneConstants.OneAIForcedTargetZone);
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            var targetZoneCollider = PuzzleSceneTestHelper.FindTargetZone(TargetZoneID.TEST_TARGET_ZONE).ZoneCollider;
            TestHelperMethods.SetAgentDestinationPositionReached(mouseTestAIManager.GetAgent(), targetZoneCollider.bounds.center);
            yield return null;
            Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone(), "The AI should escape from the target zone if it enters inside target trigger zone.");
            Assert.IsTrue(mouseAIBheavior.IsInTargetZone(), "The AI should be inside target trigger zone.");
        }

        [UnityTest]
        public IEnumerator AI_TargetZone_WhenAIIsNearTargetZone_AiFOVShoudlBeReducedBasedOnTargetZoneBoundCenter_Test()
        {
            yield return this.Before(SceneConstants.OneAIForcedTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (GenericPuzzleAIBehavior)mouseTestAIManager.GetAIBehavior();
            var targetZoneCollider = PuzzleSceneTestHelper.FindTargetZone(TargetZoneID.TEST_TARGET_ZONE).ZoneCollider;
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
            var targetZoneCollider = PuzzleSceneTestHelper.FindTargetZone(TargetZoneID.TEST_TARGET_ZONE).ZoneCollider;
            var gameConfiguration = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var targetEscapeSemiAngle = gameConfiguration.TargetZonesConfiguration()[TargetZoneID.TEST_TARGET_ZONE].EscapeFOVSemiAngle;
            var aiPosition = targetZoneCollider.transform.position + new Vector3(-0.1f, 0f, 0f);
            TestHelperMethods.SetAgentDestinationPositionReached(mouseTestAIManager.GetAgent(), aiPosition);
            yield return null; //AI is inside the target zone
            TestHelperMethods.SetAgentDestinationPositionReached(mouseTestAIManager.GetAgent(), PuzzleSceneTestHelper.FindAITestPosition(AITestPositionID.FAR_AWAY_POSITION_1).position);
            // TestHelperMethods.SetAgentDestinationPositionReached(mouseTestAIManager.GetAgent());
            yield return null;
            Assert.IsTrue(mouseAIBheavior.IsEscapingFromExitZone(), "AI should still escape as there are distance to cross.");
            Assert.IsFalse(mouseAIBheavior.IsInTargetZone(), "AI has exited the target zone.");
            Assert.AreNotEqual(new FOVSlice(0f, 360f), mouseAIBheavior.GetFOV().FovSlices[0], "AI fov must not be resetted after destination reached and still distance to cross.");
            TestHelperMethods.SetAgentDestinationPositionReached(mouseTestAIManager.GetAgent());
            yield return null;
            Assert.IsFalse(mouseAIBheavior.IsEscapingFromExitZone(), "AI should stop escape when destination is reached.");
            Assert.AreEqual(new FOVSlice(0f, 360f), mouseAIBheavior.GetFOV().FovSlices[0]);
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
            yield return null;
            Assert.IsTrue(mouseAIBheavior.IsFeared());
            yield return new WaitForSeconds(fearTime);
            Assert.IsFalse(mouseAIBheavior.IsFeared());
        }
    }


}
