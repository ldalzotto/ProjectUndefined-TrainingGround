using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using RTPuzzle;
using NodeGraph;

namespace Experimental.Editor_NodeEditor
{
    public class ConditionNodeEditor : NodeEditor
    {
        protected override Dictionary<string, Type> NodePickerConfiguration => new Dictionary<string, Type>() {
            { typeof(AndNode).Name, typeof(AndNode) },
            { typeof(OrNode).Name, typeof(OrNode) },
            {typeof(OutputResultNode).Name, typeof(OutputResultNode) },
            {typeof(AITargetConditionNode).Name, typeof(AITargetConditionNode) }
        };

        protected override Type NodeEditorProfileType => typeof(ConditionGraphEditorProfile);

        public static void Init(NodeEditorProfile nodeEditorProfile)
        {
            ConditionNodeEditor window = (ConditionNodeEditor)EditorWindow.GetWindow(typeof(ConditionNodeEditor));
            nodeEditorProfile.Init();
            window.NodeEditorProfile = nodeEditorProfile;
            window.Show();
        }

        protected override void OnEnable_Impl()
        {
        }

        protected override void OnGUI_Impl()
        {
        }
    }

}
