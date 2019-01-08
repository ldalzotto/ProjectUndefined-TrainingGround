using System.Collections;
using UnityEngine;

public class PointOfInterestEventManager : MonoBehaviour
{

    private PlayerManager PlayerManager;
    private PointOfInterestManager PointOfInterestManager;

    private void Start()
    {
        PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
        PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
    }

    public void OnPOICreated(PointOfInterestType POICreated)
    {
        PointOfInterestManager.OnPOICreated(POICreated);
    }

    public void DestroyPOI(PointOfInterestType POITobeDestroyed)
    {
        PointOfInterestManager.OnPOIDestroyed(POITobeDestroyed);
        PlayerManager.OnPOIDestroyed(POITobeDestroyed);
        StartCoroutine(DestroyPOICoroutine(POITobeDestroyed));
    }

    private IEnumerator DestroyPOICoroutine(PointOfInterestType POITobeDestroyed)
    {
        yield return new WaitForEndOfFrame();
        Destroy(POITobeDestroyed.GetRootObject());
    }

}