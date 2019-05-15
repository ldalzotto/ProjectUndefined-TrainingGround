using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace CoreGame
{
    public class SceneLoadingHelper
    {
        public static AsyncOperation SceneLoadWithoutDuplicates(string sceneToLoadName)
        {
            bool loadScene = true;
            int sceneCount = SceneManager.sceneCount;
            for (var i = 0; i < sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == sceneToLoadName)
                {
                    loadScene = false;
                }
            }
            if (loadScene)
            {
                Debug.Log(MyLog.Format("Load : " + sceneToLoadName));
                return SceneManager.LoadSceneAsync(sceneToLoadName, LoadSceneMode.Additive);
            }
            return null;
        }

        public static AsyncOperation SceneUnLoadWIthoutDuplicates(string sceneToUnloadName)
        {
            bool unloadScene = false;
            int sceneCount = SceneManager.sceneCount;
            for(var i = 0;i < sceneCount; i++)
            {
                if(SceneManager.GetSceneAt(i).name == sceneToUnloadName)
                {
                    unloadScene = true;
                }
            }
            if (unloadScene)
            {
                Debug.Log(MyLog.Format("UnLoad : " + sceneToUnloadName));
                return SceneManager.UnloadSceneAsync(sceneToUnloadName);
            }
            return null;
        }
    }

}
