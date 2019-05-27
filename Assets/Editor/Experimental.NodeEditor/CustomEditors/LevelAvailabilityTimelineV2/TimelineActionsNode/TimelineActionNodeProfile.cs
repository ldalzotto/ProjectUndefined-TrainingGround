using UnityEngine;
using System.Collections;
using NodeGraph;
using System.Collections.Generic;
using System;
using UnityEditor;

namespace Editor_LevelAvailabilityNodeEditor
{
    [System.Serializable]
    public class TimelineActionNodeProfile : NodeProfile
    {

        public LevelCompletedTimelineActionEdgeV2 TimelineAction;
        public TimelineActionToNodeEdgeV2 TimelineActionConnection;

        public override List<NodeEdgeProfile> InitInputEdges()
        {
            this.TimelineAction = NodeEdgeProfile.CreateNodeEdge<LevelCompletedTimelineActionEdgeV2>(this, NodeEdgeType.SINGLE_INPUT);
            return new List<NodeEdgeProfile>()
            {
                 this.TimelineAction
            };
        }

        public override List<NodeEdgeProfile> InitOutputEdges()
        {
            this.TimelineActionConnection = NodeEdgeProfile.CreateNodeEdge<TimelineActionToNodeEdgeV2>(this, NodeEdgeType.SINGLE_INPUT);
            return new List<NodeEdgeProfile>()
            {
                 this.TimelineActionConnection
            };
        }

        protected override Vector2 Size()
        {
            return new Vector2(180, 150);
        }

    }

}
