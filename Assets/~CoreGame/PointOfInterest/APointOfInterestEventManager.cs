using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CoreGame
{
    public abstract class APointOfInterestEventManager : MonoBehaviour
    {
        public abstract void Init();
        public abstract void OnPOICreated(APointOfInterestType POICreated);
        public abstract void DisablePOI(APointOfInterestType POITobeDisabled);
        
        public virtual void EnablePOI(APointOfInterestType POIToEnable)
        {
            POIToEnable.OnPOIEnabled();
        }
        public void DisablePOIs(List<APointOfInterestType> POIsTobeDisabled)
        {
            foreach (var POITobeDisabled in POIsTobeDisabled)
            {
                DisablePOI(POITobeDisabled);
            }
        }
    }
}
