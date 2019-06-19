using CoreGame;
using NodeGraph;
using System.Collections.Generic;
using UnityEngine;

namespace Editor_LevelAvailabilityNodeEditor
{
    public interface TimelineWorklowActionNodeProfileDataRetrieval
    {
        TimelineNodeWorkflowActionV2Drawable GetWorkflowAction();
    }

    [System.Serializable]
    public abstract class TimelineWorklowActionTemplatedNodeProfile<T, A> : NodeProfile, TimelineWorklowActionNodeProfileDataRetrieval where A : TimelineNodeWorkflowActionV2Drawable where T : TimelineWorklowActionEdgeV2<A>
    {
        public T WorkflowActionEdge;
        public WorkflowActionToNodeEdge WorkflowActionToNodeEdge;

        public virtual TimelineNodeWorkflowActionV2Drawable GetWorkflowAction()
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

        protected override string NodeTitle()
        {
            return typeof(A).Name;
        }

        protected override Vector2 Size()
        {
            return new Vector2(300, 200);
        }
    }
}