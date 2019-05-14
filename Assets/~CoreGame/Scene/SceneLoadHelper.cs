using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadHelper
{
    public static void LoadScene(Coroutiner coroutiner, LevelZonesID nextZone)
    {
        LoadScene(coroutiner, LoadSceneMode.Single, nextZone);
    }

    public static void LoadScene(Coroutiner coroutiner, LoadSceneMode loadSceneMode, LevelZonesID nextZone)
    {
        coroutiner.StopAllCoroutines();
        SceneManager.LoadScene(LevelZones.LevelZonesSceneName[nextZone], loadSceneMode);
    }
}
