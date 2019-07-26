using Editor_GameDesigner;
using UnityEditor;
using UnityEngine;

namespace RTPuzzle
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(InteractiveObjectType))]
    public class InteractiveObjectTypeCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("EDIT MODULES"))
            {
                GameDesignerEditor.InitWithSelectedKey(typeof(InteractiveObjectModuleWizard));
            }
            base.OnInspectorGUI();
        }
    }
}
