using UnityEngine;
using System.Collections;
using NodeGraph;
using System;
using System.Collections.Generic;

namespace Editor_LevelAvailabilityNodeEditor
{
    [System.Serializable]
    public class TimelineNodeEdgeProfile : NodeEdgeProfile
    {
        public override List<Type> AllowedConnectedNodeEdges => new List<Type>()
        {
            typeof(TimelineNodeEdgeProfile)
        };

        protected override Color EdgeColor()
        {
            return Color.blue;
        }
    }

}
