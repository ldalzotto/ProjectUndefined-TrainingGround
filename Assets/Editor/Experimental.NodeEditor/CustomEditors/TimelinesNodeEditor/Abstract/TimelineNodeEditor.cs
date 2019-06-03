using CoreGame;
using Experimental.Editor_NodeEditor;
using NodeGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor_LevelAvailabilityNodeEditor
{
    public abstract class TimelineNodeEditor<TIMELINE_INITIALIZER, TIMELINE_CONTEXT, NODE_KEY> : NodeEditor
                    where NODE_KEY : Enum
                    where TIMELINE_INITIALIZER : TimelineInitializerV2<TIMELINE_CONTEXT, NODE_KEY>
    {

        public static void Init(NodeEditorProfile nodeEditorProfile, Type timelineNodeEditorType)
        {
            TimelineNodeEditor<TIMELINE_INITIALIZER, TIMELINE_CONTEXT, NODE_KEY> window = (TimelineNodeEditor<TIMELINE_INITIALIZER, TIMELINE_CONTEXT, NODE_KEY>)EditorWindow.GetWindow(timelineNodeEditorType);
            nodeEditorProfile.Init();
            window.NodeEditorProfile = nodeEditorProfile;
            window.Show();
        }



        protected override void OnEnable_Impl()
        {
        }

        protected override void OnGUI_Impl()
        {
            if (GUILayout.Button("GENERATE", EditorStyles.miniButton, GUILayout.Width(100f)))
            {
                this.Generate();
            }
            if (GUILayout.Button("SEARCH", EditorStyles.miniButton, GUILayout.Width(75f)))
            {
                new TimelineNodeSearchTreePicker().OnSearch(ref this.nodeEditorProfile, (int selectedNodeKey) => {
                    this.OnManuallyNodeSelected(selectedNodeKey);
                    this.Repaint();
                });
            }
        }

        private void Generate()
        {
            var LevelAvailabilityTimelineInitializerV2 = (TIMELINE_INITIALIZER)ScriptableObject.CreateInstance(typeof(TIMELINE_INITIALIZER));

            var nodes = new Dictionary<NODE_KEY, TimelineNodeV2<TIMELINE_CONTEXT, NODE_KEY>>();
            var initialNodes = new List<NODE_KEY>();

            foreach (var node in this.nodeEditorProfile.Nodes.Values)
            {
                var timelineNode = node as TimelineNodeProfile<NODE_KEY>;
                if (timelineNode != null)
                {
                    var Transitions = new Dictionary<TimeLineAction, List<NODE_KEY>>();

                    for (var i = 0; i < timelineNode.TransitionTimelineActionInputEdges.Count; i++)
                    {
                        var timelineAction = (timelineNode.TransitionTimelineActionInputEdges[i].BackwardConnectedNodeEdges[0].NodeProfileRef as TimelineActionNodeProfileDataRetrieval).GetTimelineAction();
                        var transitioningNodeIds = timelineNode.TransitionTimelineActionOutputEdges[i].ConnectedNodeEdges.ConvertAll(e => (e.NodeProfileRef as TimelineNodeProfileDataRetrieval).GetTimelineNodeId<NODE_KEY>());
                        Transitions.Add(timelineAction, transitioningNodeIds);
                    }

                    var OnStartWorkflowAction =
                            timelineNode.OnStartWorkflowActionEdges
                              .ConvertAll(e => (TimelineNodeWorkflowActionV2<TIMELINE_CONTEXT, NODE_KEY>)(e.BackwardConnectedNodeEdges[0].NodeProfileRef as TimelineWorklowActionNodeProfileDataRetrieval).GetWorkflowAction());
                    var OnExitWorkflowAction =
                           timelineNode.OnExitWorkflowActionEdges
                             .ConvertAll(e => (TimelineNodeWorkflowActionV2<TIMELINE_CONTEXT, NODE_KEY>)(e.BackwardConnectedNodeEdges[0].NodeProfileRef as TimelineWorklowActionNodeProfileDataRetrieval).GetWorkflowAction());

                    TimelineNodeV2<TIMELINE_CONTEXT, NODE_KEY> createdNode = new TimelineNodeV2<TIMELINE_CONTEXT, NODE_KEY>(Transitions,
                       OnStartWorkflowAction,
                        OnExitWorkflowAction);

                    nodes.Add(timelineNode.TimelineNodeId, createdNode);
                }
                else
                {
                    var startTimelineNode = node as TimelineStartNodeProfile;
                    if (startTimelineNode != null)
                    {
                        initialNodes.AddRange(startTimelineNode.StartNodeEdge.ConnectedNodeEdges.ConvertAll(e => (e.NodeProfileRef as TimelineNodeProfile<NODE_KEY>).TimelineNodeId));
                    }
                }
            }

            LevelAvailabilityTimelineInitializerV2.InitialNodes = initialNodes;
            LevelAvailabilityTimelineInitializerV2.Nodes = nodes;
            Debug.Assert(LevelAvailabilityTimelineInitializerV2.InitialNodes.Count > 0);

            var generationPath = MyFileUtil.GetAssetDirectoryPath(this.nodeEditorProfile) + LevelAvailabilityTimelineInitializerV2.GetType().Name + ".asset";
            AssetDatabase.CreateAsset(LevelAvailabilityTimelineInitializerV2, generationPath);
            //update timeline game configuration
            var timelineConfiguration = AssetFinder.SafeSingleAssetFind<TimelineConfiguration>("t:" + typeof(TimelineConfiguration).Name);
            timelineConfiguration.SetEntry(((TimelineNodeEditorProfile)this.nodeEditorProfile).TimelineID,
                (TimelineInitializerScriptableObject)AssetDatabase.LoadAssetAtPath(generationPath, typeof(TimelineInitializerScriptableObject)));
        }


        class TimelineNodeSearchTreePicker
        {
            private TreePickerPopup nodeSearchTreePicker;
            private Dictionary<NODE_KEY, int> temporaryNodeLookupTable;

            public void OnSearch(ref NodeEditorProfile nodeEditorProfile, Action<int> onSelectionChange)
            {
                this.temporaryNodeLookupTable = new Dictionary<NODE_KEY, int>();
                foreach (var node in nodeEditorProfile.Nodes.Values)
                {
                    var timelineNode = node as TimelineNodeProfile<NODE_KEY>;
                    if (timelineNode != null)
                    {
                        this.temporaryNodeLookupTable.Add(timelineNode.TimelineNodeId, timelineNode.Id);
                    }
                }
                this.nodeSearchTreePicker = new TreePickerPopup(this.temporaryNodeLookupTable.Keys.ToList().ConvertAll(e => e.ToString()), () =>
                {
                    onSelectionChange.Invoke(this.temporaryNodeLookupTable[(NODE_KEY)Enum.Parse(typeof(NODE_KEY), this.nodeSearchTreePicker.SelectedKey)]);
                }, string.Empty);

                var popupRect = new Rect(Event.current.mousePosition, Vector2.zero);
                this.nodeSearchTreePicker.WindowDimensions = new Vector2(200, 450);
                this.nodeSearchTreePicker.RepaintAction = () => { GUI.changed = true; };
                PopupWindow.Show(popupRect, this.nodeSearchTreePicker);
            }
        }

    }

}
