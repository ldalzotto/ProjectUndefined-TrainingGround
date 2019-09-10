using CoreGame;
using UnityEngine;

namespace AdventureGame
{
    public class PlayerPointOfInterestSelectionManager : AbstractSelectableObjectSelectionManager<PointOfInterestType>, SelectableObjectSelectionManagerEventListener<PointOfInterestType>
    {
        public override SelectableObjectSelectionManagerEventListener<PointOfInterestType> SelectableObjectSelectionManagerEventListener => this;

        private AdventureEventsManager AdventureEventsManager;
        private PointOfInterestTrackerModule playerPointOfInterestTrackerModule;

        public void AdventureInit(IGameInputManager GameInputManager, PointOfInterestTrackerModule playerPointOfInterestTrackerModule)
        {
            this.Init(GameInputManager);
            this.AdventureEventsManager = AdventureGameSingletonInstances.AdventureEventsManager;
            this.playerPointOfInterestTrackerModule = playerPointOfInterestTrackerModule;
        }

        public void OnSelectableObjectDeSelected(PointOfInterestType SelectableObject) {  }

        public void OnSelectableObjectSelected(PointOfInterestType SelectableObject)
        {
            this.AdventureEventsManager.ADV_EVT_OnSelectableObjectSelected(SelectableObject);
        }

        public override void Tick(float d)
        {
           this.ReplaceSelectableObjects(this.playerPointOfInterestTrackerModule.GetAllPointOfInterestsInRangeAndInteractable());
            base.Tick(d);
        }

        #region Data Retrieval
        public PointOfInterestType GetCurrentSelectedPOI()
        {
            var currentSelectedObject = this.GetCurrentSelectedObject();
            if (currentSelectedObject == null)
            {
                return null;
            }
            return currentSelectedObject;
        }
        #endregion
    }


}