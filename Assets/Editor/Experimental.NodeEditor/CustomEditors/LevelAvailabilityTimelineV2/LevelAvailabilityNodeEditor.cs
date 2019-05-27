using UnityEngine;
using System.Collections;
using Experimental.Editor_NodeEditor;
using System;
using System.Collections.Generic;
using UnityEditor;
using NodeGraph;
using CoreGame;

namespace Editor_LevelAvailabilityNodeEditor
{
    public class LevelAvailabilityNodeEditor : NodeEditor
    {
        protected override Type NodeEditorProfileType => typeof(LevelAvailabilityNodeProfile);

        public static void Init(NodeEditorProfile nodeEditorProfile)
        {
            LevelAvailabilityNodeEditor window = (LevelAvailabilityNodeEditor)EditorWindow.GetWindow(typeof(LevelAvailabilityNodeEditor));
            nodeEditorProfile.Init();
            window.NodeEditorProfile = nodeEditorProfile;
            window.Show();
        }

        protected override Dictionary<string, Type> NodePickerConfiguration => new Dictionary<string, Type>()
        {
            {typeof(TimelineActionNodeProfile).Name, typeof(TimelineActionNodeProfile) },
            {typeof(LevelAvailabilityNodeProfile).Name, typeof(LevelAvailabilityNodeProfile) },
            {typeof(LevelUnlockWorklowActionV2).Name, typeof(LevelUnlockWorklowActionV2) },
            {typeof(TimelineStartNodeV2).Name, typeof(TimelineStartNodeV2) }
        };

        protected override void OnEnable_Impl()
        {

        }

        protected override void OnGUI_Impl()
        {
            if (GUILayout.Button("GENERATE", EditorStyles.miniButton, GUILayout.Width(100f)))
            {
                this.Generate();
            }
        }

        private void Generate()
        {
            var LevelAvailabilityTimelineInitializerV2 = (LevelAvailabilityTimelineInitializerV2) ScriptableObject.CreateInstance(typeof(LevelAvailabilityTimelineInitializerV2));

            var nodes = new Dictionary<LevelAvailabilityTimelineNodeID, TimelineNodeV2<LevelAvailabilityManager, LevelAvailabilityTimelineNodeID>>();
            var initialNodes = new List<LevelAvailabilityTimelineNodeID>();

            foreach (var node in this.nodeEditorProfile.Nodes.Values)
            {          
                var timelineNode = node as LevelAvailabilityNodeProfile;
                if (timelineNode != null)
                {
                    var Transitions = new Dictionary<TimeLineAction, List<LevelAvailabilityTimelineNodeID>>();

                    for (var i = 0; i < timelineNode.TransitionTimelineActionInputEdges.Count; i++)
                    {
                        var timelineAction = (timelineNode.TransitionTimelineActionInputEdges[i].BackwardConnectedNodeEdges[0].NodeProfileRef as TimelineActionNodeProfile).TimelineAction.LevelCompletedTimelineAction;
                        var transitioningNodeIds = timelineNode.TransitionTimelineActionOutputEdges[i].ConnectedNodeEdges.ConvertAll(e => (e.NodeProfileRef as LevelAvailabilityNodeProfile).LevelAvailabilityTimelineNodeID);
                        Transitions.Add(timelineAction, transitioningNodeIds);
                    }

                    var OnStartWorkflowAction = timelineNode.OnStartWorkflowActionEdges
                        .ConvertAll(e => (TimelineNodeWorkflowActionV2<LevelAvailabilityManager, LevelAvailabilityTimelineNodeID>)((e.BackwardConnectedNodeEdges[0].NodeProfileRef as LevelUnlockWorklowActionV2).LevelUnlockWorklowActionEdgeV2.LevelUnlockWorkflowAction));
                    var OnExitWorkflowAction = timelineNode.OnExitWorkflowActionEdges
                        .ConvertAll(e => (TimelineNodeWorkflowActionV2<LevelAvailabilityManager, LevelAvailabilityTimelineNodeID>)((e.BackwardConnectedNodeEdges[0].NodeProfileRef as LevelUnlockWorklowActionV2).LevelUnlockWorklowActionEdgeV2.LevelUnlockWorkflowAction));

                    LevelUnlockNodeV2 createdNode = new LevelUnlockNodeV2(Transitions,
                       OnStartWorkflowAction,
                        OnExitWorkflowAction);

                    nodes.Add(timelineNode.LevelAvailabilityTimelineNodeID, createdNode);
                }
                else
                {
                    var startTimelineNode = node as TimelineStartNodeV2;
                    if (startTimelineNode != null)
                    {
                        initialNodes = startTimelineNode.StartNodeEdge.ConnectedNodeEdges.ConvertAll(e => (e.NodeProfileRef as LevelAvailabilityNodeProfile).LevelAvailabilityTimelineNodeID);
                    }
                }
            }

            LevelAvailabilityTimelineInitializerV2.InitialNodes = initialNodes;
            LevelAvailabilityTimelineInitializerV2.Nodes = nodes;
            Debug.Assert(LevelAvailabilityTimelineInitializerV2.InitialNodes.Count > 0);
            AssetDatabase.CreateAsset(LevelAvailabilityTimelineInitializerV2, "Assets/Editor/Experimental.NodeEditor/CustomEditors/LevelAvailabilityTimelineV2/LevelAvailabilityTimelineInitializerV2.asset");
        }
    }

}
