using UnityEngine;
using System.Collections;
using NodeGraph;
using System.Collections.Generic;

namespace Editor_LevelAvailabilityNodeEditor
{
    [System.Serializable]
    public class TimelineStartNodeProfile : NodeProfile
    {
        public TimelineNodeEdgeProfile StartNodeEdge;
        public override List<NodeEdgeProfile> InitInputEdges()
        {
            return new List<NodeEdgeProfile>() { };
        }

        public override List<NodeEdgeProfile> InitOutputEdges()
        {
            StartNodeEdge = TimelineNodeEdgeProfile.CreateNodeEdge<TimelineNodeEdgeProfile>(this, NodeEdgeType.SINGLE_INPUT);
            return new List<NodeEdgeProfile>() { StartNodeEdge };
        }

        protected override Color NodeColor()
        {
            return Color.green;
        }
    }

}
