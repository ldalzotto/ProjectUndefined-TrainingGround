
using AdventureGame;
using System.Collections.Generic;

namespace timeline.serialized
{
    public class RootTimelinesSerialized
    {
        public List<TimelineSerialized> timelines { get; }

        public RootTimelinesSerialized(List<TimelineSerialized> timelines)
        {
            this.timelines = timelines;
        }
    }

    public class TimelineSerialized
    {
        public string TimelineID { get; }
        public List<TimelineNodeSerialized> Nodes { get; }

        public TimelineSerialized(string timelineID, List<TimelineNodeSerialized> nodes)
        {
            TimelineID = timelineID;
            Nodes = nodes;
        }
    }

    public class TimelineNodeSerialized
    {
        public string TimelineNodeId { get; }
        public List<TimelineNodeTransitionSerialized> TimelineNodeTransitionsSerialized { get; }

        public TimelineNodeSerialized(List<TimelineNodeTransitionSerialized> imelineNodeTransitionsSerialized, string timelineNodeId)
        {
            TimelineNodeId = timelineNodeId;
            TimelineNodeTransitionsSerialized = imelineNodeTransitionsSerialized;
        }
    }

    public class TimelineNodeTransitionSerialized
    {
        public string ScenarioActionId { get; }
        public ScenarioAction ScenarioAction { get; }
        public TimelineNodeSerialized NextNode { get; }

        public TimelineNodeTransitionSerialized(ScenarioAction scenarioAction, TimelineNodeSerialized nextNode, string scenarioActionId)
        {
            ScenarioAction = scenarioAction;
            NextNode = nextNode;
            ScenarioActionId = scenarioActionId;
        }
    }
}