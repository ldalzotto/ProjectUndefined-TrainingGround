﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using NodeGraph;
using RTPuzzle;
using Editor_LevelAvailabilityNodeEditor;
using Editor_ScenarioNodeEditor;
using Editor_DiscussionTreeNodeEditor;

namespace Experimental.Editor_NodeEditor
{
    [CustomEditor(typeof(NodeEditorProfile), true)]
    public class NodeEditorCustomInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("OPEN IN EDITOR"))
            {

                if (this.target.GetType() == typeof(ConditionGraphEditorProfile))
                {
                    ConditionNodeEditor.Init((NodeEditorProfile)this.target);
                }
                else if (this.target.GetType() == typeof(LevelAvailabilityNodeEditorProfile))
                {
                    LevelAvailabilityNodeEditor.Init((NodeEditorProfile)this.target, typeof(LevelAvailabilityNodeEditor));
                }
                else if (this.target.GetType() == typeof(ScenarioNodeEditorProfile))
                {
                    ScenarioNodeEditor.Init((NodeEditorProfile)this.target, typeof(ScenarioNodeEditor));
                }
                else if (this.target.GetType() == typeof(DiscussionTreeNodeEditorProfile))
                {
                    DiscussionTreeNodeEditorProfile.Init((NodeEditorProfile)this.target);
                }
                else
                {
                    Debug.LogError("NodeEditor not defined.");
                }
            }
            base.OnInspectorGUI();
        }
    }
}
