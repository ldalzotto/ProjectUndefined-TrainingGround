using System.Collections.Generic;
using UnityEngine;

public class PointOfInterestManager : MonoBehaviour
{

    private PointOfInterestContainerManager pointOfInterestContainerManager;

    private PointOfInterestContainerManager PointOfInterestContainerManager
    {
        get
        {
            if (pointOfInterestContainerManager == null)
            {
                pointOfInterestContainerManager = new PointOfInterestContainerManager();
            }
            return pointOfInterestContainerManager;
        }
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

    public PointOfInterestType GetActivePointOfInterest(PointOfInterestId pointOfInterestId)
    {
        var activePOI = PointOfInterestContainerManager.GetActivePointOfInterest(pointOfInterestId);
        if (activePOI != null)
        {
            return activePOI;
        }
        else
        {
            return PointOfInterestContainerManager.GetInactivePointOfInterest(pointOfInterestId);
        }
    }
}

class PointOfInterestContainerManager
{
    private List<PointOfInterestType> activePointOfInterests = new List<PointOfInterestType>();
    private List<PointOfInterestType> inactivePointOfInterest = new List<PointOfInterestType>();

    public void OnPOICreated(PointOfInterestType POICreated)
    {
        if (POICreated.InteractionWithPlayerAllowed)
        {
            activePointOfInterests.Add(POICreated);
        }
        else
        {
            inactivePointOfInterest.Add(POICreated);
        }

    }

    public void OnPOIDestroyed(PointOfInterestType POITobeDestroyed)
    {
        if (POITobeDestroyed.InteractionWithPlayerAllowed)
        {
            activePointOfInterests.Remove(POITobeDestroyed);
        }
        else
        {
            inactivePointOfInterest.Remove(POITobeDestroyed);
        }

    }
    public PointOfInterestType GetActivePointOfInterest(PointOfInterestId pointOfInterestId)
    {
        foreach (var activePOI in activePointOfInterests)
        {
            if (activePOI.PointOfInterestId == pointOfInterestId)
            {
                return activePOI;
            }
        }
        return null;
    }

    public PointOfInterestType GetInactivePointOfInterest(PointOfInterestId pointOfInterestId)
    {
        foreach (var inactivePOI in inactivePointOfInterest)
        {
            if (inactivePOI.PointOfInterestId == pointOfInterestId)
            {
                return inactivePOI;
            }
        }
        return null;
    }
}
