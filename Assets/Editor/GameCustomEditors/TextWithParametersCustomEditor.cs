using ConfigurationEditor;
using CoreGame;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor_GameCustomEditors
{
    [CustomEditor(typeof(DiscussionTextInherentData))]
    public class TextWithParametersCustomEditor : Editor
    {
        private FoldableReordableList<InputParameter> parameterList;
        private Dictionary<int, float> parameterListElementHeight;

        private void OnEnable()
        {
            DiscussionTextInherentData TextWithParametersTarget = (DiscussionTextInherentData)target;

            if (TextWithParametersTarget.InputParameters == null) { TextWithParametersTarget.InputParameters = new List<InputParameter>(); };
            this.parameterListElementHeight = new Dictionary<int, float>();
            this.parameterList = new FoldableReordableList<InputParameter>(TextWithParametersTarget.InputParameters, true, true, true, true, nameof(TextWithParametersTarget.InputParameters), 1f, this.InputParameterGUI,
                (int index) => { if (this.parameterListElementHeight.ContainsKey(index) && this.parameterList.Displayed) { return this.parameterListElementHeight[index]; } else { return 0f; } });
        }

        private void InputParameterGUI(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.x += 20;
            DiscussionTextInherentData TextWithParametersTarget = (DiscussionTextInherentData)target;
            EditorGUI.PropertyField(rect, this.serializedObject.FindProperty(nameof(TextWithParametersTarget.InputParameters)).GetArrayElementAtIndex(index), true);
            this.parameterListElementHeight[index] = EditorGUI.GetPropertyHeight(this.serializedObject.FindProperty(nameof(TextWithParametersTarget.InputParameters)).GetArrayElementAtIndex(index), true);
        }


        public override void OnInspectorGUI()
        {
            DiscussionTextInherentData TextWithParametersTarget = (DiscussionTextInherentData)target;

            EditorGUI.BeginChangeCheck();
            TextWithParametersTarget.Text = EditorGUILayout.TextArea(TextWithParametersTarget.Text);
            this.parameterList.DoLayoutList();

            this.serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(this.target);
            }
        }
    }
}