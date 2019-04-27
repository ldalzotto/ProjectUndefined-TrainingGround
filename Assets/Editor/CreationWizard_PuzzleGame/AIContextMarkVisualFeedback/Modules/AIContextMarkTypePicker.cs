using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Editor_AIContextMarkVisualFeedbackCreationWizardEditor
{
    public class AIContextMarkTypePicker : CreationModuleComponent
    {
        public bool SingleMark;
        public bool AlternanceMark;

        public AIContextMarkTypePicker(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        protected override string headerDescriptionLabel => "Choose either to create a single AI mark (a single object), or an alternance AI mark (alternance between two mark at a fixed rate).";

        public override void ResetEditor()
        {
        }

        protected override void OnInspectorGUIImpl(SerializedObject serializedObject)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            this.SingleMark = GUILayout.Toggle(this.SingleMark, "SingleMark", EditorStyles.miniButtonLeft);
            if (EditorGUI.EndChangeCheck())
            {
                if (this.SingleMark)
                {
                    this.AlternanceMark = false;
                }
            }

            EditorGUI.BeginChangeCheck();
            this.AlternanceMark = GUILayout.Toggle(this.AlternanceMark, "AlternanceMark", EditorStyles.miniButtonRight);
            if (EditorGUI.EndChangeCheck())
            {
                if (this.AlternanceMark)
                {
                    this.SingleMark = false;
                }
            }


            EditorGUILayout.EndHorizontal();
        }
    }

}
