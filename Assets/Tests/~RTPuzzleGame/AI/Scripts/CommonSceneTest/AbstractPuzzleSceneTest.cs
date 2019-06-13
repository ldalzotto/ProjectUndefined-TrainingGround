using CoreGame;
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

        public MockPuzzleEventsManager MockPuzzleEventsManagerTest { get => mockPuzzleEventsManagerTest; }

        public IEnumerator Before(string sceneName)
        {
            yield return this.Before(sceneName, AiID.MOUSE_TEST);
        }

        public IEnumerator Before(string sceneName, AiID choosenId)
        {
            this.mockPuzzleEventsManagerTest = null;
            SceneManager.sceneLoaded += (Scene scene, LoadSceneMode loadSceneMode) =>
            {
                var puzzleEventManagerObject = GameObject.FindObjectOfType<PuzzleEventsManager>().gameObject;
                var puzzleEventManager = puzzleEventManagerObject.GetComponent<PuzzleEventsManager>();
                this.mockPuzzleEventsManagerTest = puzzleEventManagerObject.AddComponent(typeof(MockPuzzleEventsManager)) as MockPuzzleEventsManager;
                this.mockPuzzleEventsManagerTest.ClearCalls();

                var npcAIManager = GameObject.FindObjectOfType<NPCAIManager>();
                npcAIManager.AiID = choosenId;
            };
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            yield return new WaitForFixedUpdate();
            GameObject.FindObjectOfType<PuzzleEventsManager>();
            var timeflowManager = GameObject.FindObjectOfType<TimeFlowManager>();
            timeflowManager.Init(
                    GameObject.FindObjectOfType<PlayerManagerDataRetriever>(),
                    GameObject.FindObjectOfType<PlayerManager>(),
                    new MockedInputManager(),
                    GameObject.FindObjectOfType<PuzzleGameConfigurationManager>(),
                    GameObject.FindObjectOfType<TimeFlowBarManager>(),
                    GameObject.FindObjectOfType<LevelManager>()
            );

            //AI Components default intialization
            GameObject.FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManagers().Values.ToList().ForEach((NPCAIManager npcAimanager) =>
            {
                PuzzleSceneTestHelper.InitializeAIComponents(npcAimanager.GetAIBehavior().AIComponents);
            });
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

            public bool TimeForwardButtonDH()
            {
                return true;
            }
        }

        public class MockPuzzleEventsManager : PuzzleEventsManager
        {

            public bool OnDestinationReachedCalled;
            public int AiHittedByProjectileCallCount;

            public override void PZ_EVT_AI_DestinationReached(AiID aiID)
            {
                base.PZ_EVT_AI_DestinationReached(aiID);
                this.OnDestinationReachedCalled = true;
            }

            public override void PZ_EVT_AI_Projectile_Hitted(AiID aiID)
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

        public class MockPersistanceManager : PersistanceManager
        {
            public override void Init()
            {
            
            }

            public override void OnPersistRequested(Action persistAction)
            {
            }

            public override T Load<T>(string folderPath, string dataPath, string filename, string fileExtension)
            {
                return default(T);
            }
        }

        public class MockDottedLineRendererManager : DottedLineRendererManager
        {
            public override void Init()
            {
            }

            public override void OnComputeBeziersInnerPointEvent(DottedLine DottedLine)
            {
                
            }

            public override void OnComputeBeziersInnerPointResponse(ComputeBeziersInnerPointResponse ComputeBeziersInnerPointResponse)
            {
                
            }

            public override void OnDottedLineDestroyed(DottedLine dottedLine)
            {
            
            }

            public override void OnLevelExit()
            {
            }

            public override void Tick()
            {
            }

        }
    }
}
