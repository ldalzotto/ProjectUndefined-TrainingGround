using AdventureGame;
using GameConfigurationID;
using NodeGraph;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor_DiscussionTreeNodeEditor
{
    [System.Serializable]
    public class DiscussionChoiceToTextEdge : NodeEdgeProfile
    {
        public override List<Type> AllowedConnectedNodeEdges => new List<Type>()
        {
            typeof(DiscussionChoiceTextInputEdge)
        };

        protected override Color EdgeColor()
        {
            return MyColors.Coral;
        }

        public DiscussionNodeId GetConnectedDiscussionNodeId()
        {
            if(this.ConnectedNodeEdges != null && this.ConnectedNodeEdges.Count > 0)
            {
                return ((DiscussionChoiceTextInputEdge)this.ConnectedNodeEdges[0]).DiscussionNodeId;
            }
            return DiscussionNodeId.NONE;
        }
    }
}