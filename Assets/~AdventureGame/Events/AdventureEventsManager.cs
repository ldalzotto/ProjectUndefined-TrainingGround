using UnityEngine;

namespace AdventureGame
{
    public class AdventureEventsManager : MonoBehaviour
    {
        public void Init()
        {
            ContextActionWheelManager = AdventureGameSingletonInstances.ContextActionWheelManager;
            ContextActionWheelEventManager = AdventureGameSingletonInstances.ContextActionWheelEventManager;
        }

        #region Selection POI Events

        public void ADV_EVT_OnSelectableObjectSelected(PointOfInterestType selectableObject)
        {
            if (ContextActionWheelManager.IsWheelEnabled()) ContextActionWheelEventManager.OnWheelRefresh(selectableObject.GetContextActions(), WheelTriggerSource.PLAYER);
        }

        #endregion

        #region External Dependencies

        private ContextActionWheelManager ContextActionWheelManager;
        private ContextActionWheelEventManager ContextActionWheelEventManager;

        #endregion
    }
}