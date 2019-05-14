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
                return SceneManager.LoadSceneAsync(sceneToLoadName, LoadSceneMode.Additive);
            }
            return null;
        }
    }

}
