using CoreGame;
using UnityEngine;

namespace AdventureGame
{
    public class PointOfInterestAdventureEventManager : APointOfInterestEventManager
    {

        private PointOfInterestManager PointOfInterestManager;
        private GhostsPOIManager GhostsPOIManager;
        private PlayerManager PlayerManager;
        private LevelTransitionManager LevelTransitionManager;

        public override void Init()
        {
            this.PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
            this.GhostsPOIManager = GameObject.FindObjectOfType<GhostsPOIManager>();
            this.PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
            this.LevelTransitionManager = GameObject.FindObjectOfType<LevelTransitionManager>();
        }

        public override void OnPOICreated(APointOfInterestType POICreated)
        {

            GhostsPOIManager.OnPOICreated(POICreated);
            PointOfInterestManager.OnPOICreated((PointOfInterestType)POICreated);
        }

        public override void EnablePOI(APointOfInterestType POIToEnable)
        {
            this.PointOfInterestManager.OnPOIEnabled((PointOfInterestType)POIToEnable);
            base.EnablePOI(POIToEnable);
        }

        public override void OnPOIDestroyed(APointOfInterestType POIToDestroy)
        {
            if (POIToDestroy != null)
            {
                var POIToBeDisabledCasted = (PointOfInterestType)POIToDestroy;
                if (!LevelTransitionManager.IsNewZoneLoading())
                {
                    foreach (var poi in this.PointOfInterestManager.GetAllPointOfInterests())
                    {
                            poi.OnPOIDestroyed(POIToDestroy);
                    }
                }
                PointOfInterestManager.OnPOIDestroyed(POIToBeDisabledCasted);
            }
        }

        public override void DisablePOI(APointOfInterestType POITobeDisabled)
        {
            if (POITobeDisabled != null)
            {
                var POIToBeDisabledCasted = (PointOfInterestType)POITobeDisabled;
                if (!LevelTransitionManager.IsNewZoneLoading())
                {
                    foreach (var poi in this.PointOfInterestManager.GetAllPointOfInterests())
                    {
                        poi.OnPOIDisabled(POITobeDisabled);
                    }
                }
                PointOfInterestManager.OnPOIDisabled(POIToBeDisabledCasted);
                GhostsPOIManager.OnPOIDisabled(POIToBeDisabledCasted);
            }
        }


    }

}
