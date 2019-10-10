using AdventureGame;
using CoreGame;
using NodeGraph;
using RTPuzzle;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace Experimental.Editor_NodeEditor
{
    public class PuzzleCutsceneNodeEditor : NodeEditor
    {
        protected override Type NodeEditorProfileType => typeof(CutsceneGraph);

        protected override Dictionary<string, Type> NodePickerConfiguration => new Dictionary<string, Type>() {
            {typeof(CutsceneStartNode).Name,  typeof(CutsceneStartNode)},
            {typeof(CutsceneWorkflowWaitForSecondsNode).Name, typeof(CutsceneWorkflowWaitForSecondsNode) },
            {typeof(CutsceneWorkflowAbortNode).Name,  typeof(CutsceneWorkflowAbortNode)},
            {typeof(BranchInfiniteLoopNode).Name, typeof(BranchInfiniteLoopNode) },
            {typeof(DummyCutsceneNode).Name, typeof(DummyCutsceneNode) }
        };

        public static void Init(NodeEditorProfile nodeEditorProfile)
        {
            PuzzleCutsceneNodeEditor window = (PuzzleCutsceneNodeEditor)EditorWindow.GetWindow(typeof(PuzzleCutsceneNodeEditor));
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