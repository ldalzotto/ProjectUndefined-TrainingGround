using UnityEngine;
using System.Collections;
using NodeGraph;
using System;
using System.Collections.Generic;

namespace Editor_LevelAvailabilityNodeEditor
{
    [System.Serializable]
    public class WorkflowActionToNodeEdge : NodeEdgeProfile
    {
        public override List<Type> AllowedConnectedNodeEdges => new List<Type>() {
            typeof(WorkflowActionToNodeEdge)
        };

        protected override Color EdgeColor()
        {
            return Color.green;
        }
    }
}

