using AdventureGame;
using NodeGraph;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor_ScenarioNodeEditor
{
    [System.Serializable]
    public class TimelineContextActionLinkedEdge : NodeEdgeProfile
    {
        public override List<Type> AllowedConnectedNodeEdges => new List<Type>();

        public AContextAction BuildContextAction()
        {
            if (this.BackwardConnectedNodeEdges != null && this.BackwardConnectedNodeEdges.Count > 0)
            {
                var connectedEdge = this.BackwardConnectedNodeEdges[0];
                if (connectedEdge.GetType() == typeof(ContextActionOutputEdge))
                {
                    return ((ContextActionOutputEdge)connectedEdge).GetContextAction();
                }
            }
            return null;
        }

        protected override Color EdgeColor()
        {
            return MyColors.HotPink;
        }
    }
    
}