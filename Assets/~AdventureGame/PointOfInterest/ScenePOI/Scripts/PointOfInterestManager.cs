using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AdventureGame
{

    public class PointOfInterestManager : MonoBehaviour
    {
        
        private PointOfInterestContainerManager PointOfInterestContainerManager;
        private APointOfInterestEventManager PointOfInterestEventManager;

        public void Init()
        {
            this.PointOfInterestEventManager = GameObject.FindObjectOfType<APointOfInterestEventManager>();
            this.PointOfInterestContainerManager = new PointOfInterestContainerManager(this.PointOfInterestEventManager);
        }

        public void Tick(float d)
        {
            //When a POI is marked as enabled (setted from timeline) and the curretn state is desabled.
            //We re-enable the poi.
            List<PointOfInterestType> poisToReEnable = null;
            foreach (var disabledPointOfinterest in this.PointOfInterestContainerManager.GetDisabledPointOfInterest())
            {
                if (!disabledPointOfinterest.PointOfInterestModelState.IsDisabled)
                {
                    if (poisToReEnable == null)
                    {
                        poisToReEnable = new List<PointOfInterestType>();
                    }
                    poisToReEnable.Add(disabledPointOfinterest);
                }
            }
            if (poisToReEnable != null)
            {
                foreach (var poiToReEnable in poisToReEnable)
                {
                    this.PointOfInterestEventManager.EnablePOI(poiToReEnable);
                }
            }
        }

        #region External Events
        public void OnPOICreated(PointOfInterestType POICreated)
        {
            PointOfInterestContainerManager.OnPOICreated(POICreated);
        }

        public void OnPOIDisabled(PointOfInterestType POITobeDestroyed)
        {
            PointOfInterestContainerManager.OnPOIDisabled(POITobeDestroyed);
        }

        public void OnPOIEnabled(PointOfInterestType POITobeEnabled)
        {
            PointOfInterestContainerManager.OnPOIEnabled(POITobeEnabled);
        }

        public void OnActualZoneSwitched()
        {
            PointOfInterestContainerManager.DestroyAllPOI();
        }
        #endregion

        #region Data Retrieval
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
            return this.PointOfInterestContainerManager.GetAllPointOfInterests();
        }

        #endregion
    }

    class PointOfInterestContainerManager
    {

        private APointOfInterestEventManager PointOfInterestEventManager;

        public PointOfInterestContainerManager(APointOfInterestEventManager pointOfInterestEventManager)
        {
            PointOfInterestEventManager = pointOfInterestEventManager;
        }

        private List<PointOfInterestType> activePointOfInterests = new List<PointOfInterestType>();
        private List<PointOfInterestType> inactivePointOfInterest = new List<PointOfInterestType>();
        private List<PointOfInterestType> disabledPointOfInterest = new List<PointOfInterestType>();

        public void OnPOICreated(PointOfInterestType POICreated)
        {
            if (POICreated.IsInteractionWithPlayerAllowed())
            {
                activePointOfInterests.Add(POICreated);
            }
            else
            {
                inactivePointOfInterest.Add(POICreated);
            }

        }

        public void OnPOIDisabled(PointOfInterestType POITobeDisabled)
        {
            if (POITobeDisabled.IsInteractionWithPlayerAllowed())
            {
                activePointOfInterests.Remove(POITobeDisabled);
            }
            else
            {
                inactivePointOfInterest.Remove(POITobeDisabled);
            }
            this.disabledPointOfInterest.Add(POITobeDisabled);
        }

        public void OnPOIEnabled(PointOfInterestType POITobeEnabled)
        {
            this.disabledPointOfInterest.Remove(POITobeEnabled);
            this.OnPOICreated(POITobeEnabled);
        }

        #region Data Retrieval
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

        public List<PointOfInterestType> GetDisabledPointOfInterest()
        {
            return this.disabledPointOfInterest;
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
        #endregion

        public void DestroyAllPOI()
        {
            var allPois = activePointOfInterests.Concat(inactivePointOfInterest).ToList();
            allPois.RemoveAll(e => e == null);
            PointOfInterestEventManager.DisablePOIs(allPois.ConvertAll(p => (APointOfInterestType)p));
        }
    }

}