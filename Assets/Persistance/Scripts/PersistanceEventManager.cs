using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistanceEventManager : MonoBehaviour
{

    private PersistanceManager PersistanceManager;

    public void Init()
    {
        PersistanceManager = GameObject.FindObjectOfType<PersistanceManager>();
        SceneManager.sceneLoaded += LoadAllAtEndOfFrame;
    }

    public void SaveAll()
    {
        Debug.Log("Save ALL " + Time.frameCount);
        var allPOI = GameObject.FindObjectOfType<PointOfInterestManager>().GetAllPointOfInterests();
        foreach (var poi in allPOI)
        {
            PersistanceManager.SavePersistantable(poi as IPersistantable);
        }
    }

    public void LoadAllAtEndOfFrame(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(LoadAllAtEndOfFrame());
    }

    private IEnumerator LoadAllAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("Load ALL " + Time.frameCount);
        var allPOI = GameObject.FindObjectOfType<PointOfInterestManager>().GetAllPointOfInterests();
        Debug.Log(allPOI.Count);
        foreach (var poi in allPOI)
        {
            PersistanceManager.Load(poi as IPersistantable);
        }
    }
}
