using CoreGame;
using UnityEngine;

namespace AdventureGame
{
    public class PlayerPointOfInterestSelectionManager : AbstractSelectableObjectSelectionManager<POISelectableObject>, SelectableObjectSelectionManagerEventListener<POISelectableObject>
    {
        public override SelectableObjectSelectionManagerEventListener<POISelectableObject> SelectableObjectSelectionManagerEventListener => this;

        private AdventureEventsManager AdventureEventsManager;

        public override void Init()
        {
            this.AdventureEventsManager = GameObject.FindObjectOfType<AdventureEventsManager>();
            base.Init();
        }

        public void OnSelectableObjectDeSelected(POISelectableObject SelectableObject) {  }

        public void OnSelectableObjectSelected(POISelectableObject SelectableObject)
        {
            this.AdventureEventsManager.ADV_EVT_OnSelectableObjectSelected(SelectableObject);
        }

        #region Data Retrieval
        public PointOfInterestType GetCurrentSelectedPOI()
        {
            var currentSelectedObject = this.GetCurrentSelectedObject();
            if (currentSelectedObject == null)
            {
                return null;
            }
            return currentSelectedObject.PointOfInterestType;
        }
        #endregion

        #region External Events
        public void OnPointOfInterestInRange(PointOfInterestType pointOfInterestType)
        {
            this.AddInteractiveObjectFromSelectable(pointOfInterestType, new POISelectableObject(pointOfInterestType.GetPointOfInterestModelObject(), pointOfInterestType));
        }
        public void OnPointOfInterestExitRange(PointOfInterestType pointOfInterestType)
        {
            this.RemoveInteractiveObjectFromSelectable(pointOfInterestType);
        }
        #endregion
    }

    public class POISelectableObject : AbstractSelectableObject
    {
        public PointOfInterestType PointOfInterestType;

        public POISelectableObject(IRendererRetrievable modelObjectModule, PointOfInterestType PointOfInterestType) : base(modelObjectModule)
        {
            this.PointOfInterestType = PointOfInterestType;
        }
    }


}