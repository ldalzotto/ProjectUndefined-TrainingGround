using CoreGame;
using NodeGraph;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace Experimental.Editor_NodeEditor
{
    public class TutorialNodeEditor : NodeEditor
    {
        protected override Type NodeEditorProfileType => typeof(TutorialGraph);

        protected override Dictionary<string, Type> NodePickerConfiguration => new Dictionary<string, Type>() {
            {typeof(CutsceneStartNode).Name, typeof(CutsceneStartNode) },
            {typeof(TutorialTextNode).Name, typeof(TutorialTextNode)},
        };

        public static void Init(NodeEditorProfile nodeEditorProfile)
        {
            TutorialNodeEditor window = (TutorialNodeEditor)EditorWindow.GetWindow(typeof(TutorialNodeEditor));
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