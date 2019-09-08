using System;
using UnityEngine;

namespace AdventureGame
{
    public class AdventureEventsManager : MonoBehaviour
    {
        #region External Dependencies
        private ContextActionWheelManager ContextActionWheelManager;
        private ContextActionWheelEventManager ContextActionWheelEventManager;
        private PlayerPointOfInterestSelectionManager PlayerPointOfInterestSelectionManager;
        #endregion

        public void Init()
        {
            this.ContextActionWheelManager = GameObject.FindObjectOfType<ContextActionWheelManager>();
            this.ContextActionWheelEventManager = GameObject.FindObjectOfType<ContextActionWheelEventManager>();
            this.PlayerPointOfInterestSelectionManager = GameObject.FindObjectOfType<PlayerPointOfInterestSelectionManager>();
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
