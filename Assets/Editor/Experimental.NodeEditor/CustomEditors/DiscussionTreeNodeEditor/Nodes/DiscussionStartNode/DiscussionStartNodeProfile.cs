using AdventureGame;
using NodeGraph;
using NodeGraph_Editor;
using System.Collections.Generic;
using UnityEngine;

namespace Editor_DiscussionTreeNodeEditor
{
    public class DiscussionStartNodeProfile : NodeProfile
    {
        public override List<NodeEdgeProfile> InitInputEdges()
        {
            return new List<NodeEdgeProfile>() { };
        }

        public DiscussionStartEdge DiscussionStartEdge;
        public DiscussionTreeId DiscussionTreeId;

        public override List<NodeEdgeProfile> InitOutputEdges()
        {
            this.DiscussionStartEdge = DiscussionStartEdge.CreateNodeEdge<DiscussionStartEdge>(this, NodeEdgeType.SINGLE_INPUT);
            return new List<NodeEdgeProfile>() { this.DiscussionStartEdge };
        }

        protected override void NodeGUI(ref NodeEditorProfile nodeEditorProfileRef)
        {
            base.NodeGUI(ref nodeEditorProfileRef);
            this.DiscussionTreeId = (DiscussionTreeId)NodeEditorGUILayout.EnumField("Tree ID : ", string.Empty, this.DiscussionTreeId);
        }

        protected override Color NodeColor()
        {
            return Color.green;
        }

        protected override Vector2 Size()
        {
            return new Vector2(200, 100);
        }
    }
}