using NodeGraph;
using System;
using System.Collections.Generic;

namespace Editor_DiscussionTreeNodeEditor
{
    [System.Serializable]
    public class DiscussionStartEdge : NodeEdgeProfile
    {
        public override List<Type> AllowedConnectedNodeEdges => new List<Type>()
        {
            typeof(AdventureDiscussionTextOnlyNodeEdge),
            typeof(FixedScreenPositionTextOnlyNodeEdge),
            typeof(PuzzleDiscussionTextOnlyNodeEdge)
        };

        public AbstractTextOnlyNodeEdge GetLinkedEdge()
        {
            return this.ConnectedNodeEdges[0] as AbstractTextOnlyNodeEdge;
        }
    }
}