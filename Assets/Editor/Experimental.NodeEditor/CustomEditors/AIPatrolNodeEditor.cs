using CoreGame;
using NodeGraph;
using RTPuzzle;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace Experimental.Editor_NodeEditor
{
    public class AIPatrolNodeEditor : NodeEditor
    {
        protected override Type NodeEditorProfileType => typeof(AIPatrolGraph);

        protected override Dictionary<string, Type> NodePickerConfiguration => new Dictionary<string, Type>() {
            {typeof(CutsceneStartNode).Name,  typeof(CutsceneStartNode)},
            {typeof(AIMoveToNode).Name, typeof(AIMoveToNode) },
            {typeof(AIWarpNode).Name, typeof(AIWarpNode) },
            {typeof(CutsceneWorkflowWaitForSecondsNode).Name, typeof(CutsceneWorkflowWaitForSecondsNode) },
            {typeof(BranchInfiniteLoopNode).Name, typeof(BranchInfiniteLoopNode) },
            {typeof(DummyCutsceneNode).Name, typeof(DummyCutsceneNode) }
        };

        public static void Init(NodeEditorProfile nodeEditorProfile)
        {
            AIPatrolNodeEditor window = (AIPatrolNodeEditor)EditorWindow.GetWindow(typeof(AIPatrolNodeEditor));
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