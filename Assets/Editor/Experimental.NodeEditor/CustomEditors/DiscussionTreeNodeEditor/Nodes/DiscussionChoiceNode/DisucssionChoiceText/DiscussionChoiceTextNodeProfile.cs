using NodeGraph;
using System.Collections.Generic;
using UnityEngine;

namespace Editor_DiscussionTreeNodeEditor
{
    [System.Serializable]
    public class DiscussionChoiceTextNodeProfile : NodeProfile
    {
        public DiscussionChoiceTextInputEdge DiscussionChoiceTextInputEdge;
        public DiscussionConnectionNodeEdge DiscussionConnectionNodeEdge;

        public override List<NodeEdgeProfile> InitInputEdges()
        {
            this.DiscussionChoiceTextInputEdge = DiscussionChoiceTextInputEdge.CreateNodeEdge<DiscussionChoiceTextInputEdge>(this, NodeEdgeType.SINGLE_INPUT);
            return new List<NodeEdgeProfile>() { this.DiscussionChoiceTextInputEdge };
        }

        public override List<NodeEdgeProfile> InitOutputEdges()
        {
            this.DiscussionConnectionNodeEdge = DiscussionConnectionNodeEdge.CreateNodeEdge<DiscussionConnectionNodeEdge>(this, NodeEdgeType.SINGLE_INPUT);
            return new List<NodeEdgeProfile>() { this.DiscussionConnectionNodeEdge };
        }

        protected override Color NodeColor()
        {
            return MyColors.Coral;
        }

        protected override Vector2 Size()
        {
            return new Vector2(200,100);
        }
    }
}