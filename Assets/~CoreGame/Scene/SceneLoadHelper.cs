using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadHelper
{
    public static void LoadScene(Coroutiner coroutiner, LevelZonesID nextZone)
    {
        coroutiner.StopAllCoroutines();
        SceneManager.LoadScene(LevelZones.LevelZonesSceneName[nextZone]);
    }

    public static void LoadSceneAsync(Coroutiner coroutiner, LevelZonesID nextZone, LoadSceneMode loadSceneMode, Action<AsyncOperation> onCompleted)
    {
        coroutiner.StopAllCoroutines();
        SceneManager.LoadSceneAsync(LevelZones.LevelZonesSceneName[nextZone], loadSceneMode).completed += onCompleted;
    }
}
