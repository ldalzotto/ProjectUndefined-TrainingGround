#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace NodeGraph_Editor
{
    public static class NodeEditorGUILayout
    {
        public static GUIContent CommonGUIContent = new GUIContent();

        public static Enum EnumField(string label, string tooltip, Enum enumField)
        {
            CommonGUIContent.text = label;
            CommonGUIContent.tooltip = tooltip;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(CommonGUIContent, GUILayout.Width(label.Length * 7f));
            var retrunEnum = EditorGUILayout.EnumPopup(enumField, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
            return retrunEnum;
        }

        public static bool BoolField(string label, string tooltip, bool boolField)
        {
            CommonGUIContent.text = label;
            CommonGUIContent.tooltip = tooltip;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(CommonGUIContent, GUILayout.Width(label.Length * 7f));
            var retrunEnum = EditorGUILayout.Toggle(boolField, EditorStyles.toggle, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
            return retrunEnum;
        }

    }

}

#endif