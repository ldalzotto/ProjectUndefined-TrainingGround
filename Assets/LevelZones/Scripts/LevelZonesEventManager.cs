using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelZonesEventManager : MonoBehaviour
{

    #region External Dependencies
    private PointOfInterestManager PointOfInterestManager;
    private Coroutiner Coroutiner;
    #endregion

    private bool isNewZoneLoading;

    // Use this for initialization
    void Start()
    {
        PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
        Coroutiner = GameObject.FindObjectOfType<Coroutiner>();
    }

    #region External Events
    public void OnLevelZoneChange(LevelZonesID nextZone)
    {
        isNewZoneLoading = true;
        PointOfInterestManager.OnActualZoneSwitched();
        Coroutiner.StopAllCoroutines();
        SceneManager.LoadScene(LevelZones.LevelZonesSceneName[nextZone]);
        isNewZoneLoading = false;
    }
    #endregion

    #region Logical Conditions
    public bool IsNewZoneLoading() { return isNewZoneLoading; }
    #endregion
}
