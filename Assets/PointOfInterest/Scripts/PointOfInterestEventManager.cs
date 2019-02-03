using System.Collections;
using UnityEngine;

public class PointOfInterestEventManager : MonoBehaviour
{

    private PlayerManager PlayerManager;
    private PointOfInterestPersistanceManager pointOfInterestPersistanceManager;
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

    public PointOfInterestPersistanceManager PointOfInterestPersistanceManager
    {
        get
        {
            if (pointOfInterestPersistanceManager == null)
            {
                pointOfInterestPersistanceManager = GameObject.FindObjectOfType<PointOfInterestPersistanceManager>(); ;
            }
            return pointOfInterestPersistanceManager;
        }
    }

    private void Start()
    {
        PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
        pointOfInterestPersistanceManager = GameObject.FindObjectOfType<PointOfInterestPersistanceManager>();
    }

    public void OnPOICreated(PointOfInterestType POICreated)
    {
        PointOfInterestManager.OnPOICreated(POICreated);
        PointOfInterestPersistanceManager.LoadStateToPOI(ref POICreated);
    }

    public void DestroyPOI(PointOfInterestType POITobeDestroyed)
    {
        PointOfInterestManager.OnPOIDestroyed(POITobeDestroyed);
        PlayerManager.OnPOIDestroyed(POITobeDestroyed);
        StartCoroutine(DestroyPOICoroutine(POITobeDestroyed));
    }

    public void OnPersistAllPOIStates()
    {
        PointOfInterestPersistanceManager.OnSavePOI();
    }

    private IEnumerator DestroyPOICoroutine(PointOfInterestType POITobeDestroyed)
    {
        yield return new WaitForEndOfFrame();
        Destroy(POITobeDestroyed.GetRootObject());
    }

}