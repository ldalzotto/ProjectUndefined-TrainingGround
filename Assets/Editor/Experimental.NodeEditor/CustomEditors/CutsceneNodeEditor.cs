using AdventureGame;
using CoreGame;
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
            {typeof(CutsceneWarpNode).Name, typeof(CutsceneWarpNode)},
            {typeof(CutsceneMoveNode).Name, typeof(CutsceneMoveNode)},
            {typeof(CutsceneAnimationNode).Name, typeof(CutsceneAnimationNode)},
            {typeof(CutsceneDiscussionNode).Name, typeof(CutsceneDiscussionNode)},
            {typeof(CutsceneStartNode).Name,  typeof(CutsceneStartNode)},
            {typeof(CutsceneWorkflowAbortNode).Name,  typeof(CutsceneWorkflowAbortNode)},
            {typeof(CutsceneSpawnPOINode).Name,  typeof(CutsceneSpawnPOINode)},
            {typeof(CutsceneCameraFollowNode).Name,  typeof(CutsceneCameraFollowNode)},
            {typeof(CutsceneCameraRotationNode).Name,  typeof(CutsceneCameraRotationNode)},
            {typeof(CutsceneDestroyPOINode).Name,  typeof(CutsceneDestroyPOINode)},
            {typeof(CutsceneWorkflowWaitForSecondsNode).Name,  typeof(CutsceneWorkflowWaitForSecondsNode)},
            {typeof(CutscenePersistPOINode).Name,  typeof(CutscenePersistPOINode)},
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