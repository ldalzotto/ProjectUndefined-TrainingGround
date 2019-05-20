using UnityEngine;
using System.Collections;
using UnityEditor;
using NodeGraph;
using RTPuzzle;

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
                } else
                {
                    Debug.LogError("NodeEditor not defined.");
                }
            }
            base.OnInspectorGUI();
        }
    }
}
