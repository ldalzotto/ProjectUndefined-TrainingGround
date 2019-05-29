using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Tests
{
    [CustomEditor(typeof(TestTimelineNodeEditorProfile), true)]
    public class TestTimelineEditorCustomInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("OPEN IN EDITOR"))
            {
                TestTimelineNodeEditor.Init((TestTimelineNodeEditorProfile)this.target, typeof(TestTimelineNodeEditor));
            }
            base.OnInspectorGUI();
        }
    }

}
