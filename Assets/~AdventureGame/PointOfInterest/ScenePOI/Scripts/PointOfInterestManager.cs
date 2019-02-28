using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AdventureGame
{

    public class PointOfInterestManager : MonoBehaviour
    {

        private PointOfInterestContainerManager pointOfInterestContainerManager;

        private PointOfInterestContainerManager PointOfInterestContainerManager
        {
            get
            {
                if (pointOfInterestContainerManager == null)
                {
                    pointOfInterestContainerManager = new PointOfInterestContainerManager(PointOfInterestEventManager);
                }
                return pointOfInterestContainerManager;
            }
        }

        #region External Dependencies
        private PointOfInterestEventManager pointOfInterestEventManager;
        #endregion

        private PointOfInterestEventManager PointOfInterestEventManager
        {
            get
            {
                if (pointOfInterestEventManager == null)
                {
                    pointOfInterestEventManager = GameObject.FindObjectOfType<PointOfInterestEventManager>();
                }
                return pointOfInterestEventManager;
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

        public void OnActualZoneSwitched()
        {
            PointOfInterestContainerManager.DestroyAllPOI();
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

        public List<PointOfInterestType> GetAllPointOfInterests()
        {
            return PointOfInterestContainerManager.GetAllPointOfInterests();
        }
    }

    class PointOfInterestContainerManager
    {

        private PointOfInterestEventManager PointOfInterestEventManager;

        public PointOfInterestContainerManager(PointOfInterestEventManager pointOfInterestEventManager)
        {
            PointOfInterestEventManager = pointOfInterestEventManager;
        }

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

        public List<PointOfInterestType> GetAllPointOfInterests()
        {
            var returnList = new List<PointOfInterestType>();
            foreach (var poi in activePointOfInterests)
            {
                returnList.Add(poi);
            }
            foreach (var poi in inactivePointOfInterest)
            {
                returnList.Add(poi);
            }
            return returnList;
        }

        internal void DestroyAllPOI()
        {
            var allPois = activePointOfInterests.Concat(inactivePointOfInterest).ToList();
            allPois.RemoveAll(e => e == null);
            PointOfInterestEventManager.DetroyPOIs(allPois);
        }
    }

}