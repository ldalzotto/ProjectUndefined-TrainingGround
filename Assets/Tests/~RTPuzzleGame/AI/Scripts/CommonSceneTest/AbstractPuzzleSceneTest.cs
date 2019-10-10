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
        private Action objectDynamicInstancesCreation;
        
        public IEnumerator Before(string sceneName, Action objectDynamicInstancesCreation = null)
        {
            var dontDestroyOnLoadObject = new GameObject("go");
            DontDestroyOnLoad(dontDestroyOnLoadObject);
            foreach (var root in dontDestroyOnLoadObject.scene.GetRootGameObjects())
            {
                Destroy(root);
            }
            
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
        }
        
    }
}