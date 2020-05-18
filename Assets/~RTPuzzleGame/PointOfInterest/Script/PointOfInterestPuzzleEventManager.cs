﻿using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    public class PointOfInterestPuzzleEventManager : APointOfInterestEventManager
    {
        private AGhostPOIManager AGhostPOIManager;

  
        public override void Init()
        {
            this.AGhostPOIManager = GameObject.FindObjectOfType<AGhostPOIManager>();
        }

        public override void OnPOICreated(APointOfInterestType POICreated)
        {
            this.AGhostPOIManager.OnPOICreated(POICreated);
        }

        public override void OnPOIDestroyed(APointOfInterestType POIToDestroy)
        {
            this.DisablePOI(POIToDestroy);
        }

        public override void DisablePOI(APointOfInterestType POITobeDisabled)
        {
            if (POITobeDisabled != null)
            {
                POITobeDisabled.OnPOIDisabled(POITobeDisabled);
                AGhostPOIManager.OnPOIDisabled(POITobeDisabled);
            }
        }
    }

}
