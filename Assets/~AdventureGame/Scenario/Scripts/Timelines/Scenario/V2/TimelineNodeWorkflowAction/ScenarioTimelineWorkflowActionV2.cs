using UnityEngine;
using System.Collections;
using CoreGame;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AdventureGame
{
    [System.Serializable]
    public class AddGrabbableItemV2 : ScenarioTimelineBaseWorkflowAction
    {
        [SerializeField]
        private PointOfInterestId poiInvolved;
        [SerializeField]
        private ItemID itemInvolved;

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNodeV2<GhostsPOIManager, ScenarioTimelineNodeID> timelineNodeRefence)
        {
            var foundedPoi = GhostsPOIManager.GetGhostPOI(poiInvolved);
            if (foundedPoi != null)
            {
                var createdContextAction = this.ContextActionDataChain.BuildContextActionChain();
                createdContextAction.ContextActionWheelNodeConfigurationId = SelectionWheelNodeConfigurationId.GRAB_CONTEXT_ACTION_WHEEL_CONFIG;
                foundedPoi.OnContextActionAdd(itemInvolved, createdContextAction);
            }
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.poiInvolved = (PointOfInterestId)EditorGUILayout.EnumPopup("POI : ", this.poiInvolved);
            this.itemInvolved = (ItemID)EditorGUILayout.EnumPopup("ITEM : ", this.itemInvolved);
            base.ActionGUI();
        }
#endif
    }

    [System.Serializable]
    public class RemoveGrabbableItemV2 : ScenarioTimelineBaseWorkflowAction
    {
        [SerializeField]
        private ItemID itemInvolved;
        [SerializeField]
        private PointOfInterestId poiInvolved;

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNodeV2<GhostsPOIManager, ScenarioTimelineNodeID> timelineNodeRefence)
        {
            var foundedPoi = GhostsPOIManager.GetGhostPOI(poiInvolved);
            if (foundedPoi != null)
            {
                foundedPoi.OnGrabbableItemRemove(itemInvolved);
            }
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.poiInvolved = (PointOfInterestId)EditorGUILayout.EnumPopup("POI : ", this.poiInvolved);
            this.itemInvolved = (ItemID)EditorGUILayout.EnumPopup("ITEM : ", this.itemInvolved);
        }
#endif
    }

    [System.Serializable]
    public class AddReceivableItemV2 : ScenarioTimelineBaseWorkflowAction
    {
        [SerializeField]
        private ItemID itemInvolved;
        [SerializeField]
        private PointOfInterestId poiInvolved;

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNodeV2<GhostsPOIManager, ScenarioTimelineNodeID> timelineNodeRefence)
        {
            var foundedPoi = GhostsPOIManager.GetGhostPOI(poiInvolved);
            if (foundedPoi != null)
            {
                foundedPoi.OnReceivableItemAdd(itemInvolved);
            }
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.poiInvolved = (PointOfInterestId)EditorGUILayout.EnumPopup("POI : ", this.poiInvolved);
            this.itemInvolved = (ItemID)EditorGUILayout.EnumPopup("ITEM : ", this.itemInvolved);
        }
#endif
    }

    [System.Serializable]
    public class RemoveReceivableItemV2 : ScenarioTimelineBaseWorkflowAction
    {
        [SerializeField]
        private ItemID itemInvolved;
        [SerializeField]
        private PointOfInterestId poiInvolved;

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNodeV2<GhostsPOIManager, ScenarioTimelineNodeID> timelineNodeRefence)
        {
            var foundedPoi = GhostsPOIManager.GetGhostPOI(poiInvolved);
            if (foundedPoi != null)
            {
                foundedPoi.OnReceivableItemRemove(itemInvolved);
            }
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.poiInvolved = (PointOfInterestId)EditorGUILayout.EnumPopup("POI : ", this.poiInvolved);
            this.itemInvolved = (ItemID)EditorGUILayout.EnumPopup("ITEM : ", this.itemInvolved);
        }
#endif
    }

    [System.Serializable]
    public class AddPOIInteractableItemV2 : ScenarioTimelineBaseWorkflowAction
    {
        [SerializeField]
        private ItemID itemInvolved;
        [SerializeField]
        private PointOfInterestId poiInvolved;

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNodeV2<GhostsPOIManager, ScenarioTimelineNodeID> timelineNodeRefence)
        {
            var foundedPoi = GhostsPOIManager.GetGhostPOI(poiInvolved);
            if (foundedPoi != null)
            {
                foundedPoi.OnInteractableItemAdd(itemInvolved);
            }
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.poiInvolved = (PointOfInterestId)EditorGUILayout.EnumPopup("POI : ", this.poiInvolved);
            this.itemInvolved = (ItemID)EditorGUILayout.EnumPopup("ITEM : ", this.itemInvolved);
        }
#endif
    }

    [System.Serializable]
    public class AddInventoryItemGiveActionV2 : ScenarioTimelineBaseWorkflowAction
    {
        [SerializeField]
        private ItemID itemInvolved;
        [SerializeField]
        private PointOfInterestId poiInvolved;

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNodeV2<GhostsPOIManager, ScenarioTimelineNodeID> timelineNodeRefence)
        {
            var foundedPoi = GhostsPOIManager.GetGhostPOI(poiInvolved);
            if (foundedPoi != null)
            {
                foundedPoi.OnContextActionAdd(this.itemInvolved, this.ContextActionDataChain.BuildContextActionChain());
            }
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.poiInvolved = (PointOfInterestId)EditorGUILayout.EnumPopup("POI : ", this.poiInvolved);
            this.itemInvolved = (ItemID)EditorGUILayout.EnumPopup("ITEM : ", this.itemInvolved);
            base.ActionGUI();
        }
#endif
    }

    [System.Serializable]
    public class RemovePOIInteractableItemV2 : ScenarioTimelineBaseWorkflowAction
    {
        [SerializeField]
        private ItemID itemInvolved;
        [SerializeField]
        private PointOfInterestId poiInvolved;

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNodeV2<GhostsPOIManager, ScenarioTimelineNodeID> timelineNodeRefence)
        {
            var foundedPoi = GhostsPOIManager.GetGhostPOI(poiInvolved);
            if (foundedPoi != null)
            {
                foundedPoi.OnInteractableItemRemove(itemInvolved);
            }
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.poiInvolved = (PointOfInterestId)EditorGUILayout.EnumPopup("POI : ",  this.poiInvolved);
            this.itemInvolved = (ItemID)EditorGUILayout.EnumPopup("ITEM : ", this.itemInvolved);
        }
#endif
    }

    [System.Serializable]
    public class AddTransitionLevelV2 : ScenarioTimelineBaseWorkflowAction
    {

        [SerializeField]
        private LevelZonesID nextLevelZone;
        [SerializeField]
        private PointOfInterestId poiInvolved;

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNodeV2<GhostsPOIManager, ScenarioTimelineNodeID> timelineNodeRefence)
        {
            var foundedPoi = GhostsPOIManager.GetGhostPOI(poiInvolved);
            if (foundedPoi != null)
            {
                foundedPoi.OnLevelZoneTransitionAdd(nextLevelZone, this.ContextActionDataChain.BuildContextActionChain());
            }
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.nextLevelZone = (LevelZonesID)EditorGUILayout.EnumPopup("LEVEL : ", this.nextLevelZone);
            this.poiInvolved = (PointOfInterestId)EditorGUILayout.EnumPopup("POI : ", this.poiInvolved);
            base.ActionGUI();
        }
#endif
    }

}
