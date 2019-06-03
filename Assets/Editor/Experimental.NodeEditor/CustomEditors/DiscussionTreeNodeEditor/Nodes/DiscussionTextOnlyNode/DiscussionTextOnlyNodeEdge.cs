using AdventureGame;
using NodeGraph;
using NodeGraph_Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor_DiscussionTreeNodeEditor
{
    [System.Serializable]
    public class DiscussionTextOnlyNodeEdge : NodeEdgeProfile
    {
        public override List<Type> AllowedConnectedNodeEdges => new List<Type>() {
            typeof(DiscussionConnectionNodeEdge)
        };

        public DiscussionNodeId DiscussionNodeId;
        public DisucssionSentenceTextId DisplayedText;
        public PointOfInterestId Talker;

        protected override void GUI_Impl(Rect rect)
        {
            EditorGUILayout.BeginVertical();
            this.DiscussionNodeId = (DiscussionNodeId)NodeEditorGUILayout.EnumField("Node : ", string.Empty, this.DiscussionNodeId);
            this.DisplayedText = (DisucssionSentenceTextId)NodeEditorGUILayout.EnumField("Text : ", string.Empty, this.DisplayedText);
            this.Talker = (PointOfInterestId)NodeEditorGUILayout.EnumField("POI : ", string.Empty, this.Talker);
            EditorGUILayout.EndVertical();
        }

        protected override Color EdgeColor()
        {
            return MyColors.PaleBlue;
        }
    }
}