using AdventureGame;
using NodeGraph;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace Experimental.Editor_NodeEditor
{
    public class CutsceneNodeEditor : NodeEditor
    {
        protected override Type NodeEditorProfileType => typeof(CutsceneGraph);

        protected override Dictionary<string, Type> NodePickerConfiguration => new Dictionary<string, Type>() {
            {typeof(CutsceneStartNode).Name,  typeof(CutsceneStartNode)},
            {typeof(CutsceneWarpNode).Name, typeof(CutsceneWarpNode)},
            {typeof(CutsceneMoveNode).Name, typeof(CutsceneMoveNode)},
            {typeof(CutsceneAnimationNode).Name, typeof(CutsceneAnimationNode)},
            {typeof(CutsceneDiscussionNode).Name, typeof(CutsceneDiscussionNode)},
        };

        public static void Init(NodeEditorProfile nodeEditorProfile)
        {
            CutsceneNodeEditor window = (CutsceneNodeEditor)EditorWindow.GetWindow(typeof(CutsceneNodeEditor));
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