using CoreGame;
using Editor_LevelAvailabilityNodeEditor;
using NodeGraph;
using UnityEditor;
using UnityEngine;

namespace Experimental.Editor_NodeEditor
{
    [CustomEditor(typeof(NodeEditorProfile), true)]
    public class NodeEditorCustomInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("OPEN IN EDITOR"))
            {
                if (target.GetType() == typeof(LevelAvailabilityNodeEditorProfile))
                    LevelAvailabilityNodeEditor.Init((NodeEditorProfile) target, typeof(LevelAvailabilityNodeEditor));
                else if (target.GetType() == typeof(TutorialGraph))
                    TutorialNodeEditor.Init((NodeEditorProfile) target);
                else
                    Debug.LogError("NodeEditor not defined.");
            }

            base.OnInspectorGUI();
        }
    }
}