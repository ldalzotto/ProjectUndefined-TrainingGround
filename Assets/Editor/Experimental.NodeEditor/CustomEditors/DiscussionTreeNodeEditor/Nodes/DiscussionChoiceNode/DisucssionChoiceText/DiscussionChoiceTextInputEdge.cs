using AdventureGame;
using NodeGraph;
using NodeGraph_Editor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor_DiscussionTreeNodeEditor
{
    [System.Serializable]
    public class DiscussionChoiceTextInputEdge : NodeEdgeProfile
    {
        public override List<Type> AllowedConnectedNodeEdges => new List<Type>();

        public DisucssionSentenceTextId DisplayedText;
        public DiscussionNodeId DiscussionNodeId;

        protected override void GUI_Impl(Rect rect)
        {
            this.DiscussionNodeId = (DiscussionNodeId)NodeEditorGUILayout.EnumField("Node : ", string.Empty, this.DiscussionNodeId);
            this.DisplayedText = (DisucssionSentenceTextId)NodeEditorGUILayout.EnumField("Text : ", string.Empty, this.DisplayedText);
        }

        protected override Color EdgeColor()
        {
            return MyColors.Coral;
        }
    }
}