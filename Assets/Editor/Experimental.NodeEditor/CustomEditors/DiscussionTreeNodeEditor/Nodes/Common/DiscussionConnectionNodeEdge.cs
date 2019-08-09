using AdventureGame;
using GameConfigurationID;
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
            typeof(AdventureDiscussionTextOnlyNodeEdge),
            typeof(AdventureDiscussionChoiceInputEdge)
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
                if (connectedEdge.GetType() == typeof(AdventureDiscussionTextOnlyNodeEdge))
                {
                    return ((AdventureDiscussionTextOnlyNodeEdge)connectedEdge).DiscussionNodeId;
                }
                else if (connectedEdge.GetType() == typeof(AdventureDiscussionChoiceInputEdge))
                {
                    return ((AdventureDiscussionChoiceInputEdge)connectedEdge).DiscussionNodeId;
                }
            }
            return DiscussionNodeId.NONE;
        }
    }
}