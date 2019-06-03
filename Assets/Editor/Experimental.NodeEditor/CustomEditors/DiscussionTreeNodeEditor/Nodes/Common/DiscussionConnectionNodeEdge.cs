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
    }
}