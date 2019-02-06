using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterestEventManager : MonoBehaviour
{

    private PlayerManager PlayerManager;
    private PointOfInterestManager PointOfInterestManager;
    private GhostsPOIManager GhostsPOIManager;
    private LevelZonesEventManager levelZonesEventManager;


    public void Init()
    {
        PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
        PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
        GhostsPOIManager = GameObject.FindObjectOfType<GhostsPOIManager>();
        levelZonesEventManager = GameObject.FindObjectOfType<LevelZonesEventManager>();
    }

    private LevelZonesEventManager LevelZonesEventManager
    {
        get
        {
            if (levelZonesEventManager == null)
            {
                levelZonesEventManager = GameObject.FindObjectOfType<LevelZonesEventManager>(); ;
            }
            return levelZonesEventManager;
        }
    }

    public void OnPOICreated(PointOfInterestType POICreated)
    {
        GhostsPOIManager.OnScenePOICreated(POICreated);
        PointOfInterestManager.OnPOICreated(POICreated);
    }

    public void DestroyPOI(PointOfInterestType POITobeDestroyed)
    {
        if (!LevelZonesEventManager.IsNewZoneLoading())
        {
            POITobeDestroyed.OnPOIDestroyedFromPlayerAction();
        }
        PointOfInterestManager.OnPOIDestroyed(POITobeDestroyed);
        GhostsPOIManager.OnScenePOIDestroyed(POITobeDestroyed);
        PlayerManager.OnPOIDestroyed(POITobeDestroyed);
        StartCoroutine(DestroyPOICoroutine(POITobeDestroyed));
    }

    public void DetroyPOIs(List<PointOfInterestType> POIsTobeDestroyed)
    {
        foreach (var POITobeDestroyed in POIsTobeDestroyed)
        {
            DestroyPOI(POITobeDestroyed);
        }
    }

    private IEnumerator DestroyPOICoroutine(PointOfInterestType POITobeDestroyed)
    {
        yield return new WaitForEndOfFrame();
        Destroy(POITobeDestroyed.GetRootObject());
    }

}