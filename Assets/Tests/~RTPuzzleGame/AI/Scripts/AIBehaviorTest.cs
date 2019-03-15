using RTPuzzle;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class AIBehaviorTest : MonoBehaviour
    {

        private MockPuzzleEventsManager mockPuzzleEventsManager;

        [UnitySetUp]
        public IEnumerator Before()
        {
            this.mockPuzzleEventsManager = null;
            SceneManager.sceneLoaded += (Scene scene, LoadSceneMode loadSceneMode) =>
            {
                var puzzleEventManagerObject = FindObjectOfType<PuzzleEventsManager>().gameObject;
                MonoBehaviour.DestroyImmediate(puzzleEventManagerObject.GetComponent<PuzzleEventsManager>());
                this.mockPuzzleEventsManager = puzzleEventManagerObject.AddComponent(typeof(MockPuzzleEventsManager)) as MockPuzzleEventsManager;
            };
            SceneManager.LoadScene(LevelZones.LevelZonesSceneName[LevelZonesID.RTP_TEST]);
            yield return new WaitForFixedUpdate();
            GameObject.FindObjectOfType<PuzzleEventsManager>();
            var timeflowManager = GameObject.FindObjectOfType<TimeFlowManager>();
            timeflowManager.Init(FindObjectOfType<GameManager>().PuzzleId, FindObjectOfType<PlayerManagerDataRetriever>(), FindObjectOfType<PlayerManager>(), new MockedInputManager(), FindObjectOfType<PuzzleGameConfigurationManager>(),
                FindObjectOfType<TimeFlowBarManager>(), FindObjectOfType<LevelManager>(), FindObjectOfType<PuzzleEventsManager>());
        }

        [UnityTest]
        public IEnumerator AI_RandomPatrol_Nominal_Test()
        {        
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var oldPosition = mouseTestAIManager.transform.position;
            yield return null;
            var newPosition = mouseTestAIManager.transform.position;
            Assert.AreNotEqual(newPosition, oldPosition);
            var mouseAIBheavior = (MouseAIBehavior)mouseTestAIManager.GetAIBehavior();
            Assert.IsTrue(mouseAIBheavior.IsPatrolling());
            var beforeReachDestination = mouseTestAIManager.GetAgent().destination;
            yield return new WaitUntil(() => mockPuzzleEventsManager.OnDestinationReachedCalled);
            mockPuzzleEventsManager.OnDestinationReachedCalled = false;
            Assert.IsTrue(mouseAIBheavior.IsPatrolling());
            var afterReachDestination = mouseTestAIManager.GetAgent().destination;
            Assert.AreNotEqual(beforeReachDestination, afterReachDestination);
        }

        [UnityTest]
        public IEnumerator AI_ProjectileReceived_Nominal_Test()
        {
            yield return new WaitForSeconds(10f);
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

        public override void OnDestinationReached(AiID aiID)
        {
            base.OnDestinationReached(aiID);
            this.OnDestinationReachedCalled = true;
        }
    }
}
