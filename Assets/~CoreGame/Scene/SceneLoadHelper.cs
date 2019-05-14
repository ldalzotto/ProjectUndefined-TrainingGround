using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadHelper
{
    public static void LoadScene(LevelZonesID nextZone)
    {
        LoadScene(LoadSceneMode.Single, nextZone);
    }

    public static void LoadScene(LoadSceneMode loadSceneMode, LevelZonesID nextZone)
    {
        SceneManager.LoadScene(LevelZones.LevelZonesSceneName[nextZone], loadSceneMode);
    }
}
