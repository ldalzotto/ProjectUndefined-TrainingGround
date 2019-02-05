using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterestEventManager : MonoBehaviour
{

    private Lazy<PlayerManager> PlayerManager => new Lazy<PlayerManager>(() => GameObject.FindObjectOfType<PlayerManager>());
    private Lazy<PointOfInterestManager> PointOfInterestManager => new Lazy<PointOfInterestManager>(() => GameObject.FindObjectOfType<PointOfInterestManager>());
    private Lazy<GhostsPOIManager> GhostsPOIManager => new Lazy<GhostsPOIManager>(() => GameObject.FindObjectOfType<GhostsPOIManager>());
    private LevelZonesEventManager levelZonesEventManager;



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
        GhostsPOIManager.Value.OnScenePOICreated(POICreated);
        PointOfInterestManager.Value.OnPOICreated(POICreated);
    }

    public void DestroyPOI(PointOfInterestType POITobeDestroyed)
    {
        if (!LevelZonesEventManager.IsNewZoneLoading())
        {
            POITobeDestroyed.OnPOIDestroyedFromPlayerAction();
        }
        PointOfInterestManager.Value.OnPOIDestroyed(POITobeDestroyed);
        GhostsPOIManager.Value.OnScenePOIDestroyed(POITobeDestroyed);
        PlayerManager.Value.OnPOIDestroyed(POITobeDestroyed);
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