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
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var oldPosition = mouseTestAIManager.transform.position;
            yield return null;
            var newPosition = mouseTestAIManager.transform.position;
            Assert.AreNotEqual(newPosition, oldPosition);
            var mouseAIBheavior = (MouseAIBehavior)mouseTestAIManager.GetAIBehavior();
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
            var mouseAIBheavior = (MouseAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return null;
            Assert.IsTrue(mouseAIBheavior.IsPatrolling(), "The AI has no interaction -> Patrolling.");
            var projectileData = PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 90f, 30f);
            var lpTest = PuzzleSceneTestHelper.SpawnProjectile(projectileData, mouseTestAIManager.transform.position, launchProjectileContainerManager);
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(mouseAIBheavior.IsPatrolling(), "The AI has been hit, no more patrolling.");
            Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectileOrExitZone(), "The AI has been hit, escaping.");
            Assert.AreEqual(this.MockPuzzleEventsManagerTest.AiHittedByProjectileCallCount, 1, "The 'hitted by projectile event' must be triggered only once.");
            yield return null;
            Assert.IsNull(lpTest, "The projectile must be detroyed after hit.");
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_FirstTimeNotIntoTargetZone()
        {
            yield return this.Before(SceneConstants.OneAIForcedTargetZone);
            var launchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (MouseAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return null;
            var projectileData = PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 90f, 30f);
            var firstProj = PuzzleSceneTestHelper.SpawnProjectile(projectileData, AITestPositionID.PROJECTILE_TARGET_1, launchProjectileContainerManager);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame(); //wait for destination position to update
            var aiDestination = mouseTestAIManager.GetAgent().destination;
            var targetZoneCollider = PuzzleSceneTestHelper.FindTargetZone(TargetZoneID.TEST_TARGET_ZONE).ZoneCollider;
            Assert.IsFalse(targetZoneCollider.bounds.Contains(aiDestination), "AI Destination should not be in the target zone the first hit.");
            var secondProj = PuzzleSceneTestHelper.SpawnProjectile(projectileData, AITestPositionID.PROJECTILE_TARGET_2, launchProjectileContainerManager);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame(); //wait for destination position to update
            aiDestination = mouseTestAIManager.GetAgent().destination;
            Assert.IsTrue(targetZoneCollider.bounds.Contains(aiDestination), "AI Destination should contains the target zone the second hit.");
        }


        [UnityTest]
        public IEnumerator AI_ProjectileRecevied_NoInteruptionByAttractive_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            var launchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (MouseAIBehavior)mouseTestAIManager.GetAIBehavior();
            var attractiveObjectInherentConfigurationData = PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(999999f, 99f);
            var projectileData = PuzzleSceneTestHelper.CreateProjectileInherentData(99999f, 90f, 30f);
            yield return null;
            PuzzleSceneTestHelper.SpawnProjectile(projectileData, mouseTestAIManager.transform.position, launchProjectileContainerManager);
            yield return new WaitForFixedUpdate();
            PuzzleSceneTestHelper.SpawnAttractiveObject(attractiveObjectInherentConfigurationData, AITestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
            Assert.IsTrue(mouseAIBheavior.IsEscapingFromProjectileOrExitZone());
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_Nominal_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            float attractiveObjectEffectiveTime = .1f;
            var attractiveObjectInherentConfigurationData = PuzzleSceneTestHelper.CreateAttractiveObjectInherentConfigurationData(999999f, attractiveObjectEffectiveTime);
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (MouseAIBehavior)mouseTestAIManager.GetAIBehavior();
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
            var mouseAIBheavior = (MouseAIBehavior)mouseTestAIManager.GetAIBehavior();
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
            var mouseAIBheavior = (MouseAIBehavior)mouseTestAIManager.GetAIBehavior();
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
            var mouseAIBheavior = (MouseAIBehavior)mouseTestAIManager.GetAIBehavior();
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
            var mouseAIBheavior = (MouseAIBehavior)mouseTestAIManager.GetAIBehavior();
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



    }


}
