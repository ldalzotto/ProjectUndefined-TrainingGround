
using CoreGame;

namespace AdventureGame
{

    public class AddGrabbableItem : TimelineNodeWorkflowAction<GhostsPOIManager>
    {
        private PointOfInterestId poiInvolved;
        private ItemID itemID;
        private AContextAction contextAction;

        public AddGrabbableItem(ItemID itemID, PointOfInterestId poiInvolved, AContextAction contextAction)
        {
            this.itemID = itemID;
            this.poiInvolved = poiInvolved;
            this.contextAction = contextAction;
            this.contextAction.ContextActionWheelNodeConfigurationId = SelectionWheelNodeConfigurationId.GRAB_CONTEXT_ACTION_WHEEL_CONFIG;
        }

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNode<GhostsPOIManager> timelineNodeRefence)
        {
            var foundedPoi = GhostsPOIManager.GetGhostPOI(poiInvolved);
            if (foundedPoi != null)
            {
                foundedPoi.OnGrabbableItemAdd(itemID, contextAction);
            }
        }
    }

    public class RemoveGrabbableItem : TimelineNodeWorkflowAction<GhostsPOIManager>
    {
        private ItemID itemInvolved;
        private PointOfInterestId poiInvolved;

        public RemoveGrabbableItem(ItemID itemInvolved, PointOfInterestId poiInvolved)
        {
            this.itemInvolved = itemInvolved;
            this.poiInvolved = poiInvolved;
        }

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNode<GhostsPOIManager> timelineNodeRefence)
        {
            var foundedPoi = GhostsPOIManager.GetGhostPOI(poiInvolved);
            if (foundedPoi != null)
            {
                foundedPoi.OnGrabbableItemRemove(itemInvolved);
            }
        }
    }

    public class AddReceivableItem : TimelineNodeWorkflowAction<GhostsPOIManager>
    {
        private ItemID itemInvolved;
        private PointOfInterestId poiInvolved;

        public AddReceivableItem(ItemID itemInvolved, PointOfInterestId poiInvolved)
        {
            this.itemInvolved = itemInvolved;
            this.poiInvolved = poiInvolved;
        }

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNode<GhostsPOIManager> timelineNodeRefence)
        {
            var foundedPoi = GhostsPOIManager.GetGhostPOI(poiInvolved);
            if (foundedPoi != null)
            {
                foundedPoi.OnReceivableItemAdd(itemInvolved);
            }
        }
    }

    public class RemoveReceivableItem : TimelineNodeWorkflowAction<GhostsPOIManager>
    {
        private ItemID itemInvolved;
        private PointOfInterestId poiInvolved;

        public RemoveReceivableItem(ItemID itemInvolved, PointOfInterestId poiInvolved)
        {
            this.itemInvolved = itemInvolved;
            this.poiInvolved = poiInvolved;
        }

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNode<GhostsPOIManager> timelineNodeRefence)
        {
            var foundedPoi = GhostsPOIManager.GetGhostPOI(poiInvolved);
            if (foundedPoi != null)
            {
                foundedPoi.OnReceivableItemRemove(itemInvolved);
            }
        }
    }

    public class AddPOIInteractableItem : TimelineNodeWorkflowAction<GhostsPOIManager>
    {
        private ItemID itemInvolved;
        private PointOfInterestId poiInvolved;

        public AddPOIInteractableItem(ItemID itemInvolved, PointOfInterestId poiInvolved)
        {
            this.itemInvolved = itemInvolved;
            this.poiInvolved = poiInvolved;
        }

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNode<GhostsPOIManager> timelineNodeRefence)
        {
            var foundedPoi = GhostsPOIManager.GetGhostPOI(poiInvolved);
            if (foundedPoi != null)
            {
                foundedPoi.OnInteractableItemAdd(itemInvolved);
            }
        }
    }

    public class RemovePOIInteractableItem : TimelineNodeWorkflowAction<GhostsPOIManager>
    {
        private ItemID itemInvolved;
        private PointOfInterestId poiInvolved;

        public RemovePOIInteractableItem(ItemID itemInvolved, PointOfInterestId poiInvolved)
        {
            this.itemInvolved = itemInvolved;
            this.poiInvolved = poiInvolved;
        }

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNode<GhostsPOIManager> timelineNodeRefence)
        {
            var foundedPoi = GhostsPOIManager.GetGhostPOI(poiInvolved);
            if (foundedPoi != null)
            {
                foundedPoi.OnInteractableItemRemove(itemInvolved);
            }
        }
    }

    public class AddTransitionLevel : TimelineNodeWorkflowAction<GhostsPOIManager>
    {

        private LevelZonesID nextLevelZone;
        private PointOfInterestId poiInvolved;
        private AContextAction contextAction;

        public AddTransitionLevel(LevelZonesID nextLevelZone, PointOfInterestId poiInvolved, AContextAction contextAction)
        {
            this.nextLevelZone = nextLevelZone;
            this.poiInvolved = poiInvolved;
            this.contextAction = contextAction;
        }

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNode<GhostsPOIManager> timelineNodeRefence)
        {
            var foundedPoi = GhostsPOIManager.GetGhostPOI(poiInvolved);
            if (foundedPoi != null)
            {
                foundedPoi.OnLevelZoneTransitionAdd(nextLevelZone, contextAction);
            }
        }
    }
}