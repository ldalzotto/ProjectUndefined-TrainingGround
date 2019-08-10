using AdventureGame;
using GameConfigurationID;
using NodeGraph;
using NodeGraph_Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor_DiscussionTreeNodeEditor
{
    [System.Serializable]
    public class FixedScreenPositionTextOnlyNodeEdge : AbstractTextOnlyNodeEdge
    {
        public DiscussionPositionMarkerID DiscussionPositionMarkerID;

        protected override void AdditionalGUI()
        {
            this.DiscussionPositionMarkerID = (DiscussionPositionMarkerID)NodeEditorGUILayout.EnumField("Position : ", string.Empty, this.DiscussionPositionMarkerID);
        }
    }
}