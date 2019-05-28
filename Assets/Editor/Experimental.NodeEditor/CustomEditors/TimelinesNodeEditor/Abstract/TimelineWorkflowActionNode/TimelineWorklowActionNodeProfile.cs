using UnityEngine;
using System.Collections;
using NodeGraph;
using System.Collections.Generic;
using CoreGame;

namespace Editor_LevelAvailabilityNodeEditor
{
    public interface TimelineWorklowActionNodeProfileDataRetrieval
    {
        TimelineNodeWorkflowActionV2Drawable GetWorkflowAction();
    }

    [System.Serializable]
    public abstract class TimelineWorklowActionNodeProfile<T, A> : NodeProfile, TimelineWorklowActionNodeProfileDataRetrieval where A : TimelineNodeWorkflowActionV2Drawable where T : TimelineWorklowActionEdgeV2<A>
    {
        public T WorkflowActionEdge;
        public WorkflowActionToNodeEdge WorkflowActionToNodeEdge;

        public TimelineNodeWorkflowActionV2Drawable GetWorkflowAction()
        {
            return this.WorkflowActionEdge.WorkflowAction;
        }

        public override List<NodeEdgeProfile> InitInputEdges()
        {
            this.WorkflowActionEdge = NodeEdgeProfile.CreateNodeEdge<T>(this, NodeEdgeType.SINGLE_INPUT);
            return new List<NodeEdgeProfile>() { this.WorkflowActionEdge };
        }

        public override List<NodeEdgeProfile> InitOutputEdges()
        {
            this.WorkflowActionToNodeEdge = NodeEdgeProfile.CreateNodeEdge<WorkflowActionToNodeEdge>(this, NodeEdgeType.SINGLE_INPUT);
            return new List<NodeEdgeProfile>() { this.WorkflowActionToNodeEdge };
        }
    }
}