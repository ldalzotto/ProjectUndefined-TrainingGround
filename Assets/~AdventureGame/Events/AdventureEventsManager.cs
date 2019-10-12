using System;
using UnityEngine;

namespace AdventureGame
{
    public class AdventureEventsManager : MonoBehaviour
    {
        #region External Dependencies
        private ContextActionWheelManager ContextActionWheelManager;
        private ContextActionWheelEventManager ContextActionWheelEventManager;
        #endregion

        public void Init()
        {
            this.ContextActionWheelManager = AdventureGameSingletonInstances.ContextActionWheelManager;
            this.ContextActionWheelEventManager = AdventureGameSingletonInstances.ContextActionWheelEventManager;
        }

        #region Selection POI Events
        public void ADV_EVT_OnSelectableObjectSelected(PointOfInterestType selectableObject)
        {
            if (this.ContextActionWheelManager.IsWheelEnabled())
            {
                this.ContextActionWheelEventManager.OnWheelRefresh(selectableObject.GetContextActions(), WheelTriggerSource.PLAYER);
            }
        }
        #endregion

    }
}
