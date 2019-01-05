using System.Collections;
using UnityEngine;

public class PointOfInterestEventManager : MonoBehaviour
{

    private PlayerManager PlayerManager;

    private void Start()
    {
        PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
    }

    public void DestroyPOI(PointOfInterestType POITobeDestroyed)
    {
        PlayerManager.OnPOIDestroyed(POITobeDestroyed);
        StartCoroutine(DestroyPOICoroutine(POITobeDestroyed));
    }

    private IEnumerator DestroyPOICoroutine(PointOfInterestType POITobeDestroyed)
    {
        yield return new WaitForEndOfFrame();
        Destroy(POITobeDestroyed.GetRootObject());
    }

}