﻿using UnityEngine;
using CoreGame;
#if UNITY_EDITOR
using UnityEditor;
using NodeGraph_Editor;
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
                foundedPoi.OnContextActionAdd(itemInvolved, action);
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
            this.poiInvolved = (PointOfInterestId)NodeEditorGUILayout.EnumField("to POI : ", string.Empty, this.poiInvolved);
            this.itemInvolved = (ItemID)NodeEditorGUILayout.EnumField("remove ITEM : ", string.Empty, this.itemInvolved);
        }
#endif
    }

    [System.Serializable]
    public class AddItemGiveV2 : ScenarioTimelineBaseWorkflowAction
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
                foundedPoi.OnContextActionAdd(this.itemInvolved, new GiveAction(this.itemInvolved, null));
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
            this.poiInvolved = (PointOfInterestId)NodeEditorGUILayout.EnumField("to POI : ", string.Empty, this.poiInvolved);
            this.itemInvolved = (ItemID)NodeEditorGUILayout.EnumField("received ITEM : ", string.Empty, this.itemInvolved);
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
            this.poiInvolved = (PointOfInterestId)NodeEditorGUILayout.EnumField("to POI : ", string.Empty, this.poiInvolved);
            this.itemInvolved = (ItemID)NodeEditorGUILayout.EnumField("received ITEM : ", string.Empty, this.itemInvolved);
        }
#endif
    }

    [System.Serializable]
    public class AddPOItoItemInteractionV2 : ScenarioTimelineBaseWorkflowAction
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
    public class RemovePOItoItemInteractionV2 : ScenarioTimelineBaseWorkflowAction
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
    public class AddItemInteractionActionV2 : ScenarioTimelineBaseWorkflowAction
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
                var action = new InteractAction(this.itemInvolved,
                    new CutsceneTimelineAction(this.cutsceneId, null, this.destroyPOIAtEnd));
                foundedPoi.OnContextActionAdd(this.itemInvolved, action);
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
                foundedPoi.OnLevelZoneTransitionAdd(nextLevelZone, new LevelZoneTransitionAction(this.nextLevelZone));
            }
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.nextLevelZone = (LevelZonesID)EditorGUILayout.EnumPopup("LEVEL : ", this.nextLevelZone);
            this.poiInvolved = (PointOfInterestId)EditorGUILayout.EnumPopup("POI : ", this.poiInvolved);
        }
#endif
    }

}
