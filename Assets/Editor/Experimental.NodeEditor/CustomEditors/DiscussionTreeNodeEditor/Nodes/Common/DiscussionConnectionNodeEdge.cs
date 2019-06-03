using AdventureGame;
using NodeGraph;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor_DiscussionTreeNodeEditor
{
    [System.Serializable]
    public class DiscussionConnectionNodeEdge : NodeEdgeProfile
    {
        public override List<Type> AllowedConnectedNodeEdges => new List<Type>() {
            typeof(DiscussionTextOnlyNodeEdge),
            typeof(DiscussionChoiceInputEdge)
        };

        protected override Color EdgeColor()
        {
            return Color.yellow;
        }

        public DiscussionNodeId GetConnectedNodeEdgeDiscussionNodeID()
        {
            if (this.ConnectedNodeEdges != null && this.ConnectedNodeEdges.Count > 0)
            {
                var connectedEdge = this.ConnectedNodeEdges[0];
                if (connectedEdge.GetType() == typeof(DiscussionTextOnlyNodeEdge))
                {
                    return ((DiscussionTextOnlyNodeEdge)connectedEdge).DiscussionNodeId;
                }
                else if (connectedEdge.GetType() == typeof(DiscussionChoiceInputEdge))
                {
                    return ((DiscussionChoiceInputEdge)connectedEdge).DiscussionNodeId;
                }
            }
            return DiscussionNodeId.NONE;
        }
    }
}