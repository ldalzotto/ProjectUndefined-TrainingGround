using RTPuzzle;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class AIBehaviorTest : MonoBehaviour
    {

        private MockPuzzleEventsManager mockPuzzleEventsManager;

        public IEnumerator Before(string sceneName)
        {
            this.mockPuzzleEventsManager = null;
            SceneManager.sceneLoaded += (Scene scene, LoadSceneMode loadSceneMode) =>
            {
                var puzzleEventManagerObject = FindObjectOfType<PuzzleEventsManager>().gameObject;
                var puzzleEventManager = puzzleEventManagerObject.GetComponent<PuzzleEventsManager>();
                this.mockPuzzleEventsManager = puzzleEventManagerObject.AddComponent(typeof(MockPuzzleEventsManager)) as MockPuzzleEventsManager;
                this.mockPuzzleEventsManager.ClearCalls();
            };
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            yield return new WaitForFixedUpdate();
            GameObject.FindObjectOfType<PuzzleEventsManager>();
            var timeflowManager = GameObject.FindObjectOfType<TimeFlowManager>();
            timeflowManager.Init(FindObjectOfType<GameManager>().PuzzleId, FindObjectOfType<PlayerManagerDataRetriever>(), FindObjectOfType<PlayerManager>(), new MockedInputManager(), FindObjectOfType<PuzzleGameConfigurationManager>(),
                FindObjectOfType<TimeFlowBarManager>(), FindObjectOfType<LevelManager>(), FindObjectOfType<PuzzleEventsManager>());
        }

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
            yield return new WaitUntil(() => mockPuzzleEventsManager.OnDestinationReachedCalled);
            mockPuzzleEventsManager.ClearCalls();
            Assert.IsTrue(mouseAIBheavior.IsPatrolling());
            var afterReachDestination = mouseTestAIManager.GetAgent().destination;
            Assert.AreNotEqual(beforeReachDestination, afterReachDestination);
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_Nominal_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (MouseAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return null;
            Assert.IsTrue(mouseAIBheavior.IsPatrolling(), "The AI has no interaction -> Patrolling.");
            var projectileData = ScriptableObject.CreateInstance<ProjectileInherentData>();
            projectileData.Init(99999f, 90f, 30f);
            var lpTest = this.SpawnProjectile(projectileData, mouseTestAIManager.transform.position);
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(mouseAIBheavior.IsPatrolling(), "The AI has been hit, no more patrolling.");
            Assert.IsTrue(mouseAIBheavior.IsEscaping(), "The AI has been hit, escaping.");
            Assert.AreEqual(this.mockPuzzleEventsManager.AiHittedByProjectileCallCount, 1, "The 'hitted by projectile event' must be triggered only once.");
            yield return null;
            Assert.IsNull(lpTest, "The projectile must be detroyed after hit.");
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_FirstTimeNotIntoTargetZone()
        {
            yield return this.Before(SceneConstants.OneAIForcedTargetZone);
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (MouseAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return null;
            var projectileData = ScriptableObject.CreateInstance<ProjectileInherentData>();
            projectileData.Init(99999f, 90f, 30f);
            var firstProj = this.SpawnProjectile(projectileData, AITestPositionID.PROJECTILE_TARGET_1);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame(); //wait for destination position to update
            var aiDestination = mouseTestAIManager.GetAgent().destination;
            var targetZoneCollider = FindTargetZone(TargetZoneID.TEST_TARGET_ZONE).ZoneCollider;
            Assert.IsFalse(targetZoneCollider.bounds.Contains(aiDestination), "AI Destination should not be in the target zone the first hit.");
            var secondProj = this.SpawnProjectile(projectileData, AITestPositionID.PROJECTILE_TARGET_2);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame(); //wait for destination position to update
            aiDestination = mouseTestAIManager.GetAgent().destination;
            Assert.IsTrue(targetZoneCollider.bounds.Contains(aiDestination), "AI Destination should contains the target zone the second hit.");
        }


        [UnityTest]
        public IEnumerator AI_ProjectileRecevied_NoInteruptionByAttractive_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (MouseAIBehavior)mouseTestAIManager.GetAIBehavior();
            var attractiveObjectInherentConfigurationData = ScriptableObject.CreateInstance<AttractiveObjectInherentConfigurationData>();
            attractiveObjectInherentConfigurationData.Init(999999f, 99f);
            var projectileData = ScriptableObject.CreateInstance<ProjectileInherentData>();
            projectileData.Init(99999f, 90f, 30f);
            yield return null;
            this.SpawnProjectile(projectileData, mouseTestAIManager.transform.position);
            yield return new WaitForFixedUpdate();
            this.SpawnAttractiveObject(attractiveObjectInherentConfigurationData, AITestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject());
            Assert.IsTrue(mouseAIBheavior.IsEscaping());
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_Nominal_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            var attractiveObjectInherentConfigurationData = ScriptableObject.CreateInstance<AttractiveObjectInherentConfigurationData>();
            float attractiveObjectEffectiveTime = .1f;
            attractiveObjectInherentConfigurationData.Init(999999f, attractiveObjectEffectiveTime);
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (MouseAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return null;
            var attractiveObjectType = this.SpawnAttractiveObject(attractiveObjectInherentConfigurationData, AITestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
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
            var attractiveObjectInherentConfigurationData = ScriptableObject.CreateInstance<AttractiveObjectInherentConfigurationData>();
            attractiveObjectInherentConfigurationData.Init(999999f, 99f);
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (MouseAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return null;
            var attractiveObjectType = this.SpawnAttractiveObject(attractiveObjectInherentConfigurationData, AITestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
            yield return new WaitForFixedUpdate();
            Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject(), "The AI is being attracted.");
            var projectileData = ScriptableObject.CreateInstance<ProjectileInherentData>();
            projectileData.Init(99999f, 90f, 30f);
            this.SpawnProjectile(projectileData, mouseTestAIManager.transform.position);
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject(), "A projectile has hit the AI. Abort attract.");
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_WithSmallRange_NoInfluence_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            var attractiveObjectInherentConfigurationData = ScriptableObject.CreateInstance<AttractiveObjectInherentConfigurationData>();
            attractiveObjectInherentConfigurationData.Init(0.001f, 99f);
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (MouseAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return null;
            var attractiveObjectType = this.SpawnAttractiveObject(attractiveObjectInherentConfigurationData, AITestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject(), "The AI is too far from attractive object zone.");
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_WhenEnterInRange_Test()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            var attractiveObjectInherentConfigurationData = ScriptableObject.CreateInstance<AttractiveObjectInherentConfigurationData>();
            attractiveObjectInherentConfigurationData.Init(0.1f, 99f);
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (MouseAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return null;
            var attractiveObjectType = this.SpawnAttractiveObject(attractiveObjectInherentConfigurationData, AITestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
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
            var attractiveObjectInherentConfigurationData = ScriptableObject.CreateInstance<AttractiveObjectInherentConfigurationData>();
            attractiveObjectInherentConfigurationData.Init(999999f, 99f);
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (MouseAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return null;
            var attractiveObjectType = this.SpawnAttractiveObject(attractiveObjectInherentConfigurationData, AITestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
            mouseTestAIManager.GetAgent().Warp(mouseTestAIManager.GetAgent().destination);
            yield return null;
            yield return new WaitForEndOfFrame();
            Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject(), "The AI must still be attracted by object at the end of frame.");
            Assert.AreEqual(attractiveObjectType.transform.position.x, mouseTestAIManager.GetAgent().destination.x, "The AI must have the target position to the attractive object.");
            Assert.AreEqual(attractiveObjectType.transform.position.z, mouseTestAIManager.GetAgent().destination.z, "The AI must have the target position to the attractive object.");
        }

        private LaunchProjectile SpawnProjectile(ProjectileInherentData projectileInherentData, AITestPositionID projectilePoistion)
        {
            var projectilePosition = GameObject.FindObjectsOfType<AITestPosition>().ToList().Select(a => a).Where(pos => pos.aITestPositionID == projectilePoistion).First().transform.position;
            return SpawnProjectile(projectileInherentData, projectilePosition);
        }

        private LaunchProjectile SpawnProjectile(ProjectileInherentData projectileInherentData, Vector3 projectilePoistion)
        {
            var launchProjectile = LaunchProjectile.Instantiate(projectileInherentData, new BeziersControlPoints(), GameObject.FindObjectOfType<Canvas>());
            launchProjectile.transform.position = projectilePoistion;
            return launchProjectile;
        }

        private AttractiveObjectType SpawnAttractiveObject(AttractiveObjectInherentConfigurationData attractiveObjectInherentConfigurationData, AITestPositionID aITestPositionID)
        {
            var attractiveObjectSpawnPosition = GameObject.FindObjectsOfType<AITestPosition>().ToList().Select(a => a).Where(pos => pos.aITestPositionID == aITestPositionID).First().transform.position;
            return AttractiveObjectType.Instanciate(attractiveObjectSpawnPosition, null, attractiveObjectInherentConfigurationData);
        }

        private TargetZone FindTargetZone(TargetZoneID targetZoneID)
        {
            return GameObject.FindObjectsOfType<TargetZone>().ToArray().Select(t => t).Where(targetZone => targetZone.TargetZoneID == targetZoneID).First();
        }

    }

    class MockedInputManager : IGameInputManager
    {
        public XInput CurrentInput => new TimePressedMockedInput();

    }

    public class TimePressedMockedInput : XInput
    {
        public bool ActionButtonD()
        {
            return false;
        }

        public bool ActionButtonDH()
        {
            return false;
        }

        public Vector3 CameraRotationAxis()
        {
            return Vector3.zero;
        }

        public bool CancelButtonD()
        {
            return false;
        }

        public bool CancelButtonDH()
        {
            return false;
        }

        public bool InventoryButtonD()
        {
            return false;
        }

        public Vector3 LocomotionAxis()
        {
            return Vector3.zero;
        }

        public bool TimeForwardButtonDH()
        {
            return true;
        }
    }

    class MockPuzzleEventsManager : PuzzleEventsManager
    {

        public bool OnDestinationReachedCalled;
        public int AiHittedByProjectileCallCount;

        public override void OnDestinationReached(AiID aiID)
        {
            base.OnDestinationReached(aiID);
            this.OnDestinationReachedCalled = true;
        }

        public override void OnAiHittedByProjectile(AiID aiID, int timesInARow)
        {
            base.OnAiHittedByProjectile(aiID, timesInARow);
            this.AiHittedByProjectileCallCount += 1;
        }

        public void ClearCalls()
        {
            this.OnDestinationReachedCalled = false;
            this.AiHittedByProjectileCallCount = 0;
        }
    }
}
