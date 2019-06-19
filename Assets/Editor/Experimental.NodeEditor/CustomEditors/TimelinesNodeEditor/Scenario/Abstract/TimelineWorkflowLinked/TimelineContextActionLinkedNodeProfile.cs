using AdventureGame;
using CoreGame;
using Editor_LevelAvailabilityNodeEditor;
using NodeGraph;
using System.Collections.Generic;
using UnityEngine;

namespace Editor_ScenarioNodeEditor
{
    [System.Serializable]
    public abstract class TimelineContextActionLinkedNodeProfile<T, A> : TimelineWorklowActionTemplatedNodeProfile<T, A> where A : TimelineNodeWorkflowActionV2Drawable where T : TimelineWorklowActionEdgeV2<A>
    {
        private TimelineContextActionLinkedEdge AddContextActionLinkedEdge;

        public override List<NodeEdgeProfile> InitInputEdges()
        {
            var baseInputEdges = base.InitInputEdges();
            this.AddContextActionLinkedEdge = NodeEdgeProfile.CreateNodeEdge<TimelineContextActionLinkedEdge>(this, NodeEdgeType.SINGLE_INPUT);
            baseInputEdges.Add(this.AddContextActionLinkedEdge);
            return baseInputEdges;
        }

        public override TimelineNodeWorkflowActionV2Drawable GetWorkflowAction()
        {
            this.OnGeneration();
            return base.GetWorkflowAction();
        }

        private void OnGeneration()
        {
            var contextAction = this.AddContextActionLinkedEdge.BuildContextAction();
            var actionSettable = this.WorkflowActionEdge as IContextActionSettable;
            if (actionSettable != null)
            {
                actionSettable.SetContextAction(contextAction);
            }
        }

        protected override Vector2 Size()
        {
            return new Vector3(300, 200);
        }
    }

    public interface IContextActionSettable
    {
        void SetContextAction(AContextAction aContextAction);
    }
}