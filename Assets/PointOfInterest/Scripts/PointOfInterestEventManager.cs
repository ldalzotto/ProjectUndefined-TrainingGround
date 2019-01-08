using System.Collections;
using UnityEngine;

public class PointOfInterestEventManager : MonoBehaviour
{

    private PlayerManager PlayerManager;
    private PointOfInterestManager pointOfInterestManager;

    private PointOfInterestManager PointOfInterestManager
    {
        get
        {
            if (pointOfInterestManager == null)
            {
                pointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>(); ;
            }
            return pointOfInterestManager;
        }
    }

    private void Start()
    {
        PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
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