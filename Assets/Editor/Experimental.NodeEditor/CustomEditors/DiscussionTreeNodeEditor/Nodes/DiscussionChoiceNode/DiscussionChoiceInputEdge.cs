using System;
using System.Collections.Generic;
using AdventureGame;
using GameConfigurationID;
using NodeGraph;
using NodeGraph_Editor;
using UnityEngine;

namespace Editor_DiscussionTreeNodeEditor
{
    [System.Serializable]
    public class DiscussionChoiceInputEdge : NodeEdgeProfile
    {
        public override List<Type> AllowedConnectedNodeEdges => new List<Type>() { };

        public DiscussionNodeId DiscussionNodeId;
        public PointOfInterestId Talker;

        protected override void GUI_Impl(Rect rect)
        {
            this.DiscussionNodeId = (DiscussionNodeId)NodeEditorGUILayout.EnumField("Node : ", string.Empty, this.DiscussionNodeId);
            this.Talker = (PointOfInterestId)NodeEditorGUILayout.EnumField("POI : ", string.Empty, this.Talker);
        }

        protected override Color EdgeColor()
        {
            return Color.yellow;
        }
    }
}