using UnityEngine.SceneManagement;

public class SceneLoadHelper
{
    public static void LoadScene(Coroutiner coroutiner, LevelZonesID nextZone)
    {
        coroutiner.StopAllCoroutines();
        SceneManager.LoadScene(LevelZones.LevelZonesSceneName[nextZone]);
    }
}
