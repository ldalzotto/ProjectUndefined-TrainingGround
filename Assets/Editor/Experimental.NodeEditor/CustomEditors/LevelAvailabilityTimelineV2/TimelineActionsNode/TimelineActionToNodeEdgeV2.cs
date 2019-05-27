using UnityEngine;
using System.Collections;
using NodeGraph;
using System;
using System.Collections.Generic;

namespace Editor_LevelAvailabilityNodeEditor
{
    [System.Serializable]
    public class TimelineActionToNodeEdgeV2 : NodeEdgeProfile
    {

        public override List<Type> AllowedConnectedNodeEdges => new List<Type>()
        {
            typeof(TimelineActionToNodeEdgeV2)
        };

        protected override Color EdgeColor()
        {
            return Color.yellow;
        }
    }

}
