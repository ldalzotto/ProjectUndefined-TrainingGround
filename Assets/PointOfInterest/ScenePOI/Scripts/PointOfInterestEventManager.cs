using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterestEventManager : MonoBehaviour
{

    private PlayerManager PlayerManager;
    private PointOfInterestManager pointOfInterestManager;
    private GhostsPOIManager ghostsPOIManager;
    private LevelZonesEventManager levelZonesEventManager;

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