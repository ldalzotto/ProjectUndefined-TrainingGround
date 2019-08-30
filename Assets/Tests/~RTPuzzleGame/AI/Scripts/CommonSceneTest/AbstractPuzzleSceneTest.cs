using CoreGame;
using GameConfigurationID;
using RTPuzzle;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tests
{
    public abstract class AbstractPuzzleSceneTest : MonoBehaviour
    {
        private MockPuzzleEventsManager mockPuzzleEventsManagerTest;
        private Action objectDynamicInstancesCreation;

        public MockPuzzleEventsManager MockPuzzleEventsManagerTest { get => mockPuzzleEventsManagerTest; }
        
        public IEnumerator Before(string sceneName, Action objectDynamicInstancesCreation = null)
        {
            this.mockPuzzleEventsManagerTest = null;
            this.objectDynamicInstancesCreation = objectDynamicInstancesCreation;
            SceneManager.sceneLoaded += this.OnSceneLoadCallBack;
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            
            yield return new WaitForFixedUpdate();

            var timeflowManager = GameObject.FindObjectOfType<TimeFlowManager>();
            timeflowManager.Init(
                    GameObject.FindObjectOfType<PlayerManagerDataRetriever>(),
                    GameObject.FindObjectOfType<PlayerManager>(),
                    new MockedInputManager(),
                    GameObject.FindObjectOfType<PuzzleGameConfigurationManager>(),
                    GameObject.FindObjectOfType<TimeFlowBarManager>(),
                    GameObject.FindObjectOfType<LevelManager>()
            );
            
            SceneManager.sceneLoaded -= this.OnSceneLoadCallBack;
        }

        private void OnSceneLoadCallBack(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (objectDynamicInstancesCreation != null) { objectDynamicInstancesCreation.Invoke(); }
            var puzzleEventManagerObject = GameObject.FindObjectOfType<PuzzleEventsManager>().gameObject;
            var puzzleEventManager = puzzleEventManagerObject.GetComponent<PuzzleEventsManager>();
            this.mockPuzzleEventsManagerTest = puzzleEventManagerObject.AddComponent(typeof(MockPuzzleEventsManager)) as MockPuzzleEventsManager;
            this.mockPuzzleEventsManagerTest.ClearCalls();
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

            public bool CancelButtonD()
            {
                return false;
            }

            public bool CancelButtonDH()
            {
                return false;
            }

            public Vector3 CursorDisplacement()
            {
                throw new NotImplementedException();
            }

            public bool InventoryButtonD()
            {
                return false;
            }

            public float LeftRotationCameraDH()
            {
                throw new NotImplementedException();
            }

            public Vector3 LocomotionAxis()
            {
                return Vector3.zero;
            }

            public bool PuzzleResetButton()
            {
                return false;
            }

            public float RightRotationCameraDH()
            {
                throw new NotImplementedException();
            }

            public bool SwitchSelectionButtonD()
            {
                throw new NotImplementedException();
            }

            public bool TimeForwardButtonDH()
            {
                return true;
            }
        }

        public class MockPuzzleEventsManager : PuzzleEventsManager
        {

            public bool OnDestinationReachedCalled;
            public int AiHittedByProjectileCallCount;

            public override void PZ_EVT_AI_DestinationReached(AIObjectID aiID)
            {
                base.PZ_EVT_AI_DestinationReached(aiID);
                this.OnDestinationReachedCalled = true;
            }

            public override void PZ_EVT_AI_Projectile_Hitted(AIObjectID aiID)
            {
                base.PZ_EVT_AI_Projectile_Hitted(aiID);
                this.AiHittedByProjectileCallCount += 1;
            }

            public void ClearCalls()
            {
                this.OnDestinationReachedCalled = false;
                this.AiHittedByProjectileCallCount = 0;
            }
        }

    }
}