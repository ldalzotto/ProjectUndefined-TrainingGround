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

        public IEnumerator Before()
        {
            this.mockPuzzleEventsManager = null;
            SceneManager.sceneLoaded += (Scene scene, LoadSceneMode loadSceneMode) =>
            {
                var puzzleEventManagerObject = FindObjectOfType<PuzzleEventsManager>().gameObject;
                var puzzleEventManager = puzzleEventManagerObject.GetComponent<PuzzleEventsManager>();
                this.mockPuzzleEventsManager = puzzleEventManagerObject.AddComponent(typeof(MockPuzzleEventsManager)) as MockPuzzleEventsManager;
                this.mockPuzzleEventsManager.ClearCalls();
            };
            SceneManager.LoadScene(LevelZones.LevelZonesSceneName[LevelZonesID.RTP_TEST], LoadSceneMode.Single);
            yield return new WaitForFixedUpdate();
            GameObject.FindObjectOfType<PuzzleEventsManager>();
            var timeflowManager = GameObject.FindObjectOfType<TimeFlowManager>();
            timeflowManager.Init(FindObjectOfType<GameManager>().PuzzleId, FindObjectOfType<PlayerManagerDataRetriever>(), FindObjectOfType<PlayerManager>(), new MockedInputManager(), FindObjectOfType<PuzzleGameConfigurationManager>(),
                FindObjectOfType<TimeFlowBarManager>(), FindObjectOfType<LevelManager>(), FindObjectOfType<PuzzleEventsManager>());
        }

        [UnityTest]
        public IEnumerator AI_RandomPatrol_Nominal_Test()
        {
            yield return this.Before();
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
            yield return this.Before();
            var gameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (MouseAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return null;
            Assert.IsTrue(mouseAIBheavior.IsPatrolling(), "The AI has no interaction -> Patrolling.");
            var lpTest = this.SpawnProjectile(gameConfigurationManager.ProjectileConf()[LaunchProjectileId.PROJECTILE_TEST], mouseTestAIManager.transform.position);
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(mouseAIBheavior.IsPatrolling(), "The AI has been hit, no more patrolling.");
            Assert.IsTrue(mouseAIBheavior.IsEscaping(), "The AI has been hit, escaping.");
            Assert.AreEqual(this.mockPuzzleEventsManager.AiHittedByProjectileCallCount, 1, "The 'hitted by projectile event' must be triggered only once.");
            yield return null;
            Assert.IsNull(lpTest, "The projectile must be detroyed after hit.");
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_Nominal_Test()
        {
            yield return this.Before();
            var attractiveObjectInherentConfigurationData = ScriptableObject.CreateInstance<AttractiveObjectInherentConfigurationData>();
            attractiveObjectInherentConfigurationData.Init(999999f, 99f);
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (MouseAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return null;
            this.SpawnAttractiveObject(attractiveObjectInherentConfigurationData, AITestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(mouseAIBheavior.IsPatrolling(), "The AI has been attracted, no more patrolling.");
            Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject(), "The AI is being attracted.");
        }

        [UnityTest]
        public IEnumerator AI_AttractiveObject_DisruptedByProjectile_Test()
        {
            yield return this.Before();
            var gameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var attractiveObjectInherentConfigurationData = ScriptableObject.CreateInstance<AttractiveObjectInherentConfigurationData>();
            attractiveObjectInherentConfigurationData.Init(999999f, 99f);
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var mouseAIBheavior = (MouseAIBehavior)mouseTestAIManager.GetAIBehavior();
            yield return null;
            this.SpawnAttractiveObject(attractiveObjectInherentConfigurationData, AITestPositionID.ATTRACTIVE_OBJECT_NOMINAL);
            yield return new WaitForFixedUpdate();
            Assert.IsTrue(mouseAIBheavior.IsInfluencedByAttractiveObject(), "The AI is being attracted.");
            this.SpawnProjectile(gameConfigurationManager.ProjectileConf()[LaunchProjectileId.PROJECTILE_TEST], mouseTestAIManager.transform.position);
            yield return new WaitForFixedUpdate();
            Assert.IsFalse(mouseAIBheavior.IsInfluencedByAttractiveObject(), "A projectile has hit the AI. Abort attract.");
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

        private void SpawnAttractiveObject(AttractiveObjectInherentConfigurationData attractiveObjectInherentConfigurationData, AITestPositionID aITestPositionID)
        {
            var attractiveObjectSpawnPosition = GameObject.FindObjectsOfType<AITestPosition>().ToList().Select(a => a).Where(pos => pos.aITestPositionID == aITestPositionID).First().transform.position;
            var attractiveObject = AttractiveObjectType.Instanciate(attractiveObjectSpawnPosition, null, attractiveObjectInherentConfigurationData);
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
