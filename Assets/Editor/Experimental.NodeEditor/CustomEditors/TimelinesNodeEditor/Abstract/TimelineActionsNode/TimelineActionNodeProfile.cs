using UnityEngine;
using System.Collections;
using NodeGraph;
using System.Collections.Generic;
using CoreGame;
using Timelines;

namespace Editor_LevelAvailabilityNodeEditor
{
    public interface TimelineActionNodeProfileDataRetrieval
    {
        TimeLineAction GetTimelineAction();
    }

    public abstract class TimelineActionNodeProfile<T, A> : NodeProfile, TimelineActionNodeProfileDataRetrieval where A : TimeLineAction where T : TimelineActionEdgeProfile<A>
    {
        public T TimelineActionEdge;
        public TimelineActionToNodeEdgeV2 TimelineActionConnection;

        public TimeLineAction GetTimelineAction()
        {
            return this.TimelineActionEdge.TimelineAction;
        }

        public override List<NodeEdgeProfile> InitInputEdges()
        {
            this.TimelineActionEdge = NodeEdgeProfile.CreateNodeEdge<T>(this, NodeEdgeType.SINGLE_INPUT);
            return new List<NodeEdgeProfile>()
            {
                this.TimelineActionEdge
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
            return new Vector2(300, 150);
        }

        protected override string NodeTitle()
        {
            return typeof(A).Name;
        }
    }
}