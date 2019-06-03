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
    }
}