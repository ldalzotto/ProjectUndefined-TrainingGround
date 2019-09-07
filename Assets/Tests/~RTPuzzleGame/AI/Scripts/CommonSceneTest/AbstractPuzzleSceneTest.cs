using CoreGame;
using GameConfigurationID;
using RTPuzzle;
using System;
using System.Collections;
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
            var dontDestroyOnLoadObject = new GameObject("go");
            DontDestroyOnLoad(dontDestroyOnLoadObject);
            foreach (var root in dontDestroyOnLoadObject.scene.GetRootGameObjects())
            {
                Destroy(root);
            }

            this.mockPuzzleEventsManagerTest = null;
            this.objectDynamicInstancesCreation = objectDynamicInstancesCreation;
            SceneManager.sceneLoaded += this.OnSceneLoadCallBack;
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

            yield return new WaitForFixedUpdate();

            //Always timeforward
            GameObject.FindObjectOfType<GameTestMockedInputManager>().GetGameTestMockedXInput().timeForwardButtonDH = true;

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