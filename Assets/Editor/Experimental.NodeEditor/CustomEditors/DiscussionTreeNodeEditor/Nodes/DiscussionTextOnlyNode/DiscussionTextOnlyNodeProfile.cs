﻿using AdventureGame;
using NodeGraph;
using System.Collections.Generic;
using UnityEngine;

namespace Editor_DiscussionTreeNodeEditor
{
    [System.Serializable]
    public class DiscussionTextOnlyNodeProfile : NodeProfile
    {
        public DiscussionTextOnlyNodeEdge DiscussionNodeEdge;
        public DiscussionConnectionNodeEdge ConnectionEdge;

        public override List<NodeEdgeProfile> InitInputEdges()
        {
            this.DiscussionNodeEdge = DiscussionTextOnlyNodeEdge.CreateNodeEdge<DiscussionTextOnlyNodeEdge>(this, NodeEdgeType.SINGLE_INPUT);
            return new List<NodeEdgeProfile>() { this.DiscussionNodeEdge };
        }

        public override List<NodeEdgeProfile> InitOutputEdges()
        {
            this.ConnectionEdge = DiscussionConnectionNodeEdge.CreateNodeEdge<DiscussionConnectionNodeEdge>(this, NodeEdgeType.SINGLE_INPUT);
            return new List<NodeEdgeProfile>() { this.ConnectionEdge };
        }

        protected override Color NodeColor()
        {
            return MyColors.PaleBlue;
        }

        protected override Vector2 Size()
        {
            return new Vector2(200, 100);
        }
        
        protected override string NodeTitle()
        {
            return this.DiscussionNodeEdge.DiscussionNodeId.ToString();
        }
        
        
    }
}