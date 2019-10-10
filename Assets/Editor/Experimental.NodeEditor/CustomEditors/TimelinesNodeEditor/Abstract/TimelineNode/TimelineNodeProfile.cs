using UnityEngine;
using System.Collections;
using NodeGraph;
using CoreGame;
using System.Collections.Generic;
using UnityEditor;
using System;

namespace Editor_LevelAvailabilityNodeEditor
{
    public interface TimelineNodeProfileDataRetrieval
    {
        ENUM GetTimelineNodeId<ENUM>();
    }

    [System.Serializable]
    public abstract class TimelineNodeProfile<NODE_KEY> : NodeProfile, TimelineNodeProfileDataRetrieval where NODE_KEY : Enum
    {
        [CustomEnum()]
        public NODE_KEY TimelineNodeId;
        public List<TimelineActionToNodeEdgeV2> TransitionTimelineActionInputEdges = new List<TimelineActionToNodeEdgeV2>();
        public List<TimelineNodeEdgeProfile> TransitionTimelineActionOutputEdges = new List<TimelineNodeEdgeProfile>();
        public TimelineNodeEdgeProfile InputNodeConnectionEdge;

        public List<WorkflowActionToNodeEdge> OnStartWorkflowActionEdges = new List<WorkflowActionToNodeEdge>();
        public List<WorkflowActionToNodeEdge> OnExitWorkflowActionEdges = new List<WorkflowActionToNodeEdge>();

        public override List<NodeEdgeProfile> InitInputEdges()
        {
            this.InputNodeConnectionEdge = NodeEdgeProfile.CreateNodeEdge<TimelineNodeEdgeProfile>(this, NodeEdgeType.MULTIPLE_INPUT);
            return new List<NodeEdgeProfile>() { this.InputNodeConnectionEdge };
        }

        public override List<NodeEdgeProfile> InitOutputEdges()
        {
            return new List<NodeEdgeProfile>();
        }

        private Color customNodeColor = new Color(1, 0.5f, 0);
        private Vector2 baseNodeSize = new Vector2(120,100);

        protected override Color NodeColor()
        {
            return this.customNodeColor;
        }

        protected override Vector2 Size()
        {
            return this.baseNodeSize;
        }

        protected override string NodeTitle()
        {
            return this.TimelineNodeId.ToString();
        }

        protected override void NodeGUI(ref NodeEditorProfile nodeEditorProfileRef)
        {
            EditorGUILayout.BeginHorizontal();
            this.InputNodeConnectionEdge.GUIEdgeRectangles(this.OffsettedBounds, ref nodeEditorProfileRef);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(20f)))
            {
                var createdTransitionInputEdge = NodeEdgeProfile.CreateNodeEdge<TimelineActionToNodeEdgeV2>(this, NodeEdgeType.SINGLE_INPUT);
                this.TransitionTimelineActionInputEdges.Add(createdTransitionInputEdge);
                this.AddInputEdge(createdTransitionInputEdge);

                var createdTransitionOutputEdge = NodeEdgeProfile.CreateNodeEdge<TimelineNodeEdgeProfile>(this, NodeEdgeType.SINGLE_INPUT);
                this.TransitionTimelineActionOutputEdges.Add(createdTransitionOutputEdge);
                this.AddOutputEdge(createdTransitionOutputEdge);
            }
            GUILayout.Label("Transitions");
            EditorGUILayout.EndHorizontal();
            for (var i = 0; i < TransitionTimelineActionInputEdges.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                this.TransitionTimelineActionInputEdges[i].GUIEdgeRectangles(this.OffsettedBounds, ref nodeEditorProfileRef);
                this.TransitionTimelineActionOutputEdges[i].GUIEdgeRectangles(this.OffsettedBounds, ref nodeEditorProfileRef);
                EditorGUILayout.EndHorizontal();
            }
            DrawWorkflowActionEdges(ref nodeEditorProfileRef, ref this.OnStartWorkflowActionEdges, "On Start");
            DrawWorkflowActionEdges(ref nodeEditorProfileRef, ref this.OnExitWorkflowActionEdges, "On Exit");
        }

        private void DrawWorkflowActionEdges(ref NodeEditorProfile nodeEditorProfileRef, ref List<WorkflowActionToNodeEdge> workflowActionEdges, string label)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(20f)))
            {
                var createdEdge = NodeEdgeProfile.CreateNodeEdge<WorkflowActionToNodeEdge>(this, NodeEdgeType.SINGLE_INPUT);
                workflowActionEdges.Add(createdEdge);
                this.AddInputEdge(createdEdge);
            }
            if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20f)))
            {
                foreach(var currentSelectedObject in nodeEditorProfileRef.NodeEdtitorSelectionProfile.CurrentSelectedObjects)
                {
                    if (currentSelectedObject != null
                   && currentSelectedObject.GetType() == typeof(WorkflowActionToNodeEdge)
                   && workflowActionEdges.Contains((WorkflowActionToNodeEdge)currentSelectedObject))
                    {
                        var edgeToDelete = (NodeEdgeProfile)currentSelectedObject;
                        this.DeleteEdge(edgeToDelete);
                        workflowActionEdges.Remove((WorkflowActionToNodeEdge)edgeToDelete);
                    }
                }
            }
            GUILayout.Label(label);
            EditorGUILayout.EndHorizontal();

            foreach (var OnStartWorkflowActionEdge in workflowActionEdges)
            {
                OnStartWorkflowActionEdge.GUIEdgeRectangles(this.OffsettedBounds, ref nodeEditorProfileRef);
            }
        }

        public ENUM GetTimelineNodeId<ENUM>()
        {
            return (ENUM)Enum.Parse(typeof(ENUM), this.TimelineNodeId.ToString());
        }
    }
}