using CoreGame;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{
    public class PointOfInterestEventManager : MonoBehaviour
    {

        private PlayerManager PlayerManager;
        private PointOfInterestManager PointOfInterestManager;
        private GhostsPOIManager GhostsPOIManager;
        private LevelTransitionManager levelTransitionManager;


        public void Init()
        {
            PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
            PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
            GhostsPOIManager = GameObject.FindObjectOfType<GhostsPOIManager>();
            levelTransitionManager = GameObject.FindObjectOfType<LevelTransitionManager>();
        }

        private LevelTransitionManager LevelTransitionManager
        {
            get
            {
                if (levelTransitionManager == null)
                {
                    levelTransitionManager = GameObject.FindObjectOfType<LevelTransitionManager>(); ;
                }
                return levelTransitionManager;
            }
        }

        public void OnPOICreated(PointOfInterestType POICreated)
        {
            GhostsPOIManager.OnPOICreated(POICreated);
            PointOfInterestManager.OnPOICreated(POICreated);
        }

        public void DisablePOI(PointOfInterestType POITobeDisabled)
        {
            if (POITobeDisabled != null)
            {
                if (!LevelTransitionManager.IsNewZoneLoading())
                {
                    POITobeDisabled.OnPOIDisabled();
                }
                PointOfInterestManager.OnPOIDisabled(POITobeDisabled);
                PlayerManager.OnPOIDestroyed(POITobeDisabled);
                GhostsPOIManager.OnPOIDisabled(POITobeDisabled);
            }
        }

        public void EnablePOI(PointOfInterestType POIToEnable)
        {
            this.PointOfInterestManager.OnPOIEnabled(POIToEnable);
            POIToEnable.OnPOIEnabled();
        }

        public void DisablePOIs(List<PointOfInterestType> POIsTobeDisabled)
        {
            foreach (var POITobeDisabled in POIsTobeDisabled)
            {
                DisablePOI(POITobeDisabled);
            }
        }

    }

}