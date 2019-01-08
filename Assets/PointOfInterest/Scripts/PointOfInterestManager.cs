using System.Collections.Generic;
using UnityEngine;

public class PointOfInterestManager : MonoBehaviour
{

    private PointOfInterestContainerManager PointOfInterestContainerManager;

    private void Start()
    {
        PointOfInterestContainerManager = new PointOfInterestContainerManager();
    }


    #region External Events
    public void OnPOICreated(PointOfInterestType POICreated)
    {
        PointOfInterestContainerManager.OnPOICreated(POICreated);
    }

    public void OnPOIDestroyed(PointOfInterestType POITobeDestroyed)
    {
        PointOfInterestContainerManager.OnPOIDestroyed(POITobeDestroyed);
    }
    #endregion
}

class PointOfInterestContainerManager
{
    private List<PointOfInterestType> activePointOfInterests = new List<PointOfInterestType>();

    public void OnPOICreated(PointOfInterestType POICreated)
    {
        activePointOfInterests.Add(POICreated);
    }

    public void OnPOIDestroyed(PointOfInterestType POITobeDestroyed)
    {
        activePointOfInterests.Remove(POITobeDestroyed);
    }

}
