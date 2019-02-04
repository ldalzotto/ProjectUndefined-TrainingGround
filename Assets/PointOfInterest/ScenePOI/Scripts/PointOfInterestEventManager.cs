using System.Collections;
using UnityEngine;

public class PointOfInterestEventManager : MonoBehaviour
{

    private PlayerManager PlayerManager;
    private PointOfInterestManager pointOfInterestManager;
    private GhostsPOIManager ghostsPOIManager;

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

    private GhostsPOIManager GhostsPOIManager
    {
        get
        {
            if (ghostsPOIManager == null)
            {
                ghostsPOIManager = GameObject.FindObjectOfType<GhostsPOIManager>(); ;
            }
            return ghostsPOIManager;
        }
    }


    private void Start()
    {
        PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
    }

    public void OnPOICreated(PointOfInterestType POICreated)
    {
        GhostsPOIManager.OnScenePOICreated(POICreated);
        PointOfInterestManager.OnPOICreated(POICreated);
    }

    public void DestroyPOI(PointOfInterestType POITobeDestroyed)
    {
        GhostsPOIManager.OnScenePOIDestroyed(POITobeDestroyed);
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