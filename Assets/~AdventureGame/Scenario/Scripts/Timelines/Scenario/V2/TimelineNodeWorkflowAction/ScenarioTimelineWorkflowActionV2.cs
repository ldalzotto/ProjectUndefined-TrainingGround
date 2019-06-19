using UnityEngine;
using CoreGame;
using GameConfigurationID;
#if UNITY_EDITOR
using UnityEditor;
using NodeGraph_Editor;
#endif

namespace AdventureGame
{
    [System.Serializable]
    public class AddGrabbableItemV2 : TimelineNodeWorkflowActionV2<GhostsPOIManager, ScenarioTimelineNodeID>
    {
        [SerializeField]
        private PointOfInterestId poiInvolved;
        [SerializeField]
        private ItemID itemInvolved;
        [SerializeField]
        private bool destroyPOIAtEnd;
        [SerializeField]
        private PlayerAnimatioNamesEnum playerAnimation = PlayerAnimatioNamesEnum.PLAYER_ACTION_GRAB_DOWN;
        [SerializeField]
        private CutsceneId cutsceneId;

        #region Editor related
        [SerializeField]
        private bool isAnimation;
        [SerializeField]
        private bool isCutscene;
        #endregion

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNodeV2<GhostsPOIManager, ScenarioTimelineNodeID> timelineNodeRefence)
        {
            var foundedPoi = GhostsPOIManager.GetGhostPOI(poiInvolved);
            if (foundedPoi != null)
            {
                AContextAction action = null;
                if (this.isAnimation)
                {
                    action = new AnimatorAction(this.playerAnimation, null);
                }
                else if (this.isCutscene)
                {
                    action = new CutsceneTimelineAction(this.cutsceneId, null, false);
                }
                action.SetNextContextAction(new GrabAction(this.itemInvolved, this.destroyPOIAtEnd, null));
                action.ContextActionWheelNodeConfigurationId = SelectionWheelNodeConfigurationId.GRAB_CONTEXT_ACTION_WHEEL_CONFIG;
                foundedPoi.OnItemRelatedContextActionAdd(itemInvolved, action);
            }
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.poiInvolved = (PointOfInterestId)NodeEditorGUILayout.EnumField("to POI : ", string.Empty, this.poiInvolved);
            this.itemInvolved = (ItemID)NodeEditorGUILayout.EnumField("add ITEM : ", string.Empty, this.itemInvolved);
            this.destroyPOIAtEnd = NodeEditorGUILayout.BoolField("destroy at end : ", string.Empty, this.destroyPOIAtEnd);

            EditorGUILayout.BeginHorizontal();
            this.isAnimation = GUILayout.Toggle(this.isAnimation, "A", EditorStyles.miniButtonLeft);
            this.isCutscene = GUILayout.Toggle(this.isCutscene, "C", EditorStyles.miniButtonRight);
            if (this.isAnimation) { this.isCutscene = false; }
            if (this.isCutscene) { this.isAnimation = false; }
            EditorGUILayout.EndHorizontal();

            if (this.isAnimation)
            {
                this.playerAnimation = (PlayerAnimatioNamesEnum)NodeEditorGUILayout.EnumField("with ANIMATION : ", string.Empty, this.playerAnimation);
            }

            if (this.isCutscene)
            {
                this.cutsceneId = (CutsceneId)NodeEditorGUILayout.EnumField("with CUTESCENE : ", string.Empty, this.cutsceneId);
            }
        }
#endif
    }

    [System.Serializable]
    public class RemoveGrabbableItemV2 : TimelineNodeWorkflowActionV2<GhostsPOIManager, ScenarioTimelineNodeID>
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
            this.poiInvolved = (PointOfInterestId)NodeEditorGUILayout.EnumField("to POI : ", string.Empty, this.poiInvolved);
            this.itemInvolved = (ItemID)NodeEditorGUILayout.EnumField("remove ITEM : ", string.Empty, this.itemInvolved);
        }
#endif
    }

    [System.Serializable]
    public class AddItemGiveV2 : TimelineNodeWorkflowActionV2<GhostsPOIManager, ScenarioTimelineNodeID>
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
                foundedPoi.OnContextActionAdd(new GiveAction(this.itemInvolved, null));
            }
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.poiInvolved = (PointOfInterestId)NodeEditorGUILayout.EnumField("from POI : ", string.Empty, this.poiInvolved);
            this.itemInvolved = (ItemID)NodeEditorGUILayout.EnumField("given ITEM : ", string.Empty, this.itemInvolved);
        }
#endif
    }

    [System.Serializable]
    public class AddReceivableItemV2 : TimelineNodeWorkflowActionV2<GhostsPOIManager, ScenarioTimelineNodeID>
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
            this.poiInvolved = (PointOfInterestId)NodeEditorGUILayout.EnumField("to POI : ", string.Empty, this.poiInvolved);
            this.itemInvolved = (ItemID)NodeEditorGUILayout.EnumField("received ITEM : ", string.Empty, this.itemInvolved);
        }
#endif
    }

    [System.Serializable]
    public class RemoveReceivableItemV2 : TimelineNodeWorkflowActionV2<GhostsPOIManager, ScenarioTimelineNodeID>
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
            this.poiInvolved = (PointOfInterestId)NodeEditorGUILayout.EnumField("to POI : ", string.Empty, this.poiInvolved);
            this.itemInvolved = (ItemID)NodeEditorGUILayout.EnumField("received ITEM : ", string.Empty, this.itemInvolved);
        }
#endif
    }

    [System.Serializable]
    public class AddPOItoItemInteractionV2 : TimelineNodeWorkflowActionV2<GhostsPOIManager, ScenarioTimelineNodeID>
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
            this.poiInvolved = (PointOfInterestId)NodeEditorGUILayout.EnumField("to POI : ", string.Empty, this.poiInvolved);
            this.itemInvolved = (ItemID)NodeEditorGUILayout.EnumField("add ITEM : ", string.Empty, this.itemInvolved);
        }
#endif
    }

    [System.Serializable]
    public class RemovePOItoItemInteractionV2 : TimelineNodeWorkflowActionV2<GhostsPOIManager, ScenarioTimelineNodeID>
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
            this.poiInvolved = (PointOfInterestId)NodeEditorGUILayout.EnumField("to POI : ", string.Empty, this.poiInvolved);
            this.itemInvolved = (ItemID)NodeEditorGUILayout.EnumField("remove ITEM : ", string.Empty, this.itemInvolved);
        }
#endif
    }

    [System.Serializable]
    public class AddItemInteractionActionV2 : TimelineNodeWorkflowActionV2<GhostsPOIManager, ScenarioTimelineNodeID>
    {
        [SerializeField]
        private ItemID itemInvolved;
        [SerializeField]
        private PointOfInterestId poiInvolved;
        [SerializeField]
        private CutsceneId cutsceneId;
        [SerializeField]
        private bool destroyPOIAtEnd;

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNodeV2<GhostsPOIManager, ScenarioTimelineNodeID> timelineNodeRefence)
        {
            var foundedPoi = GhostsPOIManager.GetGhostPOI(poiInvolved);
            if (foundedPoi != null)
            {
                var action = new ItemInteractAction(this.itemInvolved,
                    new CutsceneTimelineAction(this.cutsceneId, null, this.destroyPOIAtEnd));
                foundedPoi.OnContextActionAdd(action);
            }
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.poiInvolved = (PointOfInterestId)NodeEditorGUILayout.EnumField("to POI : ", string.Empty, this.poiInvolved);
            this.itemInvolved = (ItemID)NodeEditorGUILayout.EnumField("with ITEM : ", string.Empty, this.itemInvolved);
            this.cutsceneId = (CutsceneId)NodeEditorGUILayout.EnumField("with CUTESCENE : ", string.Empty, this.cutsceneId);
            this.destroyPOIAtEnd = NodeEditorGUILayout.BoolField("destroy at end : ", string.Empty, this.destroyPOIAtEnd);
        }
#endif
    }

    [System.Serializable]
    public class AddTransitionLevelV2 : TimelineNodeWorkflowActionV2<GhostsPOIManager, ScenarioTimelineNodeID>
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
                foundedPoi.OnLevelZoneTransitionAdd(nextLevelZone, new LevelZoneTransitionAction(this.nextLevelZone));
            }
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.nextLevelZone = (LevelZonesID)NodeEditorGUILayout.EnumField("LEVEL : ", string.Empty, this.nextLevelZone);
            this.poiInvolved = (PointOfInterestId)NodeEditorGUILayout.EnumField("POI : ", string.Empty, this.poiInvolved);
        }
#endif
    }

    [System.Serializable]
    public class DisablePOI : TimelineNodeWorkflowActionV2<GhostsPOIManager, ScenarioTimelineNodeID>
    {
        [SerializeField]
        private PointOfInterestId poiInvolved;

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNodeV2<GhostsPOIManager, ScenarioTimelineNodeID> timelineNodeRefence)
        {
            var foundedPoi = GhostsPOIManager.GetGhostPOI(poiInvolved);
            if (foundedPoi != null)
            {
                foundedPoi.OnDisablePOI();
            }
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.poiInvolved = (PointOfInterestId)NodeEditorGUILayout.EnumField("POI : ", string.Empty, this.poiInvolved);
        }
#endif
    }

    [System.Serializable]
    public class EnablePOI : TimelineNodeWorkflowActionV2<GhostsPOIManager, ScenarioTimelineNodeID>
    {
        [SerializeField]
        private PointOfInterestId poiInvolved;

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNodeV2<GhostsPOIManager, ScenarioTimelineNodeID> timelineNodeRefence)
        {
            var foundedPoi = GhostsPOIManager.GetGhostPOI(poiInvolved);
            if (foundedPoi != null)
            {
                foundedPoi.OnEnablePOI();
            }
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.poiInvolved = (PointOfInterestId)NodeEditorGUILayout.EnumField("POI : ", string.Empty, this.poiInvolved);
        }
#endif
    }

    #region Non templated actions
    [System.Serializable]
    public class AddItemInteractionActionV3 : TimelineNodeWorkflowActionV2<GhostsPOIManager, ScenarioTimelineNodeID>
    {
        [SerializeField]
        private PointOfInterestId poiInvolved;
        [SerializeField]
        private ItemID itemInvolved;
        [SerializeField]
        private SelectionWheelNodeConfigurationId contextActionWheelNodeConfigurationId;
        [SerializeField]
        public AContextAction ContextAction;

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNodeV2<GhostsPOIManager, ScenarioTimelineNodeID> timelineNodeRefence)
        {
            var foundedPoi = GhostsPOIManager.GetGhostPOI(poiInvolved);
            if (foundedPoi != null)
            {
                this.ContextAction.ContextActionWheelNodeConfigurationId = contextActionWheelNodeConfigurationId;
                foundedPoi.OnItemRelatedContextActionAdd(this.itemInvolved, this.ContextAction);
            }
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.poiInvolved = (PointOfInterestId)NodeEditorGUILayout.EnumField("to POI : ", string.Empty, this.poiInvolved);
            this.itemInvolved = (ItemID)NodeEditorGUILayout.EnumField("with ITEM : ", string.Empty, this.itemInvolved);
            this.contextActionWheelNodeConfigurationId = (SelectionWheelNodeConfigurationId)NodeEditorGUILayout.EnumField("wheel icon : ", string.Empty, this.contextActionWheelNodeConfigurationId);
        }
#endif

    }

    [System.Serializable]
    public class AddContextActionV3 : TimelineNodeWorkflowActionV2<GhostsPOIManager, ScenarioTimelineNodeID>
    {
        [SerializeField]
        private PointOfInterestId poiInvolved;
        [SerializeField]
        private SelectionWheelNodeConfigurationId contextActionWheelNodeConfigurationId;
        [SerializeField]
        public AContextAction ContextAction;

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNodeV2<GhostsPOIManager, ScenarioTimelineNodeID> timelineNodeRefence)
        {
            var foundedPoi = GhostsPOIManager.GetGhostPOI(poiInvolved);
            if (foundedPoi != null)
            {
                this.ContextAction.ContextActionWheelNodeConfigurationId = contextActionWheelNodeConfigurationId;
                foundedPoi.OnContextActionAdd(this.ContextAction);
            }
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.poiInvolved = (PointOfInterestId)NodeEditorGUILayout.EnumField("to POI : ", string.Empty, this.poiInvolved);
            this.contextActionWheelNodeConfigurationId = (SelectionWheelNodeConfigurationId)NodeEditorGUILayout.EnumField("wheel icon : ", string.Empty, this.contextActionWheelNodeConfigurationId);
        }
#endif

    }

    [System.Serializable]
    public class AddDiscussionActionV3 : TimelineNodeWorkflowActionV2<GhostsPOIManager, ScenarioTimelineNodeID>
    {
        [SerializeField]
        private PointOfInterestId poiInvolved;
        [SerializeField]
        private SelectionWheelNodeConfigurationId contextActionWheelNodeConfigurationId;
        [SerializeField]
        private DiscussionTreeId discussionTreeId;
        [SerializeField]
        public AContextAction ContextAction;

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNodeV2<GhostsPOIManager, ScenarioTimelineNodeID> timelineNodeRefence)
        {
            var foundedPoi = GhostsPOIManager.GetGhostPOI(poiInvolved);
            if (foundedPoi != null)
            {
                this.ContextAction.ContextActionWheelNodeConfigurationId = contextActionWheelNodeConfigurationId;
                foundedPoi.OnDiscussionTreeAdd(this.discussionTreeId, this.ContextAction);
            }
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.poiInvolved = (PointOfInterestId)NodeEditorGUILayout.EnumField("to POI : ", string.Empty, this.poiInvolved);
            this.discussionTreeId = (DiscussionTreeId)NodeEditorGUILayout.EnumField("with Discussion tree : ", string.Empty, this.discussionTreeId);
            this.contextActionWheelNodeConfigurationId = (SelectionWheelNodeConfigurationId)NodeEditorGUILayout.EnumField("wheel icon : ", string.Empty, this.contextActionWheelNodeConfigurationId);
        }
#endif
    }

    [System.Serializable]
    public class RemoveDiscussionActionV3 : TimelineNodeWorkflowActionV2<GhostsPOIManager, ScenarioTimelineNodeID>
    {
        [SerializeField]
        private PointOfInterestId poiInvolved;
        [SerializeField]
        private DiscussionTreeId discussionTreeId;

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNodeV2<GhostsPOIManager, ScenarioTimelineNodeID> timelineNodeRefence)
        {
            var foundedPoi = GhostsPOIManager.GetGhostPOI(poiInvolved);
            if (foundedPoi != null)
            {
                foundedPoi.OnDiscussionTreeRemove(this.discussionTreeId);
            }
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.poiInvolved = (PointOfInterestId)NodeEditorGUILayout.EnumField("to POI : ", string.Empty, this.poiInvolved);
            this.discussionTreeId = (DiscussionTreeId)NodeEditorGUILayout.EnumField("with Discussion tree : ", string.Empty, this.discussionTreeId);
        }
#endif
    }
    #endregion
}
