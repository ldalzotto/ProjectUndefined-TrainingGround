#if UNITY_EDITOR

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ConfigurationEditor
{
    [System.Serializable]
    public abstract class AbstractDictionaryEnumGUIFilter
    {
        [SerializeField]
        protected string fieldName;
        [SerializeField]
        private Type fieldType;

        [SerializeField]
        private bool isFilterEnabled;
        [SerializeField]
        private bool filterFoldout;

        private GUIStyle filterContainerStyle;
        private GUIStyle filterFoldoutStyle;
        private GUIStyle filterEnabledToggleStyle;

        public AbstractDictionaryEnumGUIFilter() { }

        public AbstractDictionaryEnumGUIFilter(PropertyInfo propInfo)
        {
            this.fieldName = propInfo.Name;
            this.fieldType = propInfo.PropertyType;
            this.Init();
        }

        private void Init()
        {
            if (this.filterContainerStyle == null)
            {
                this.filterContainerStyle = new GUIStyle();
                this.filterContainerStyle.margin = new RectOffset(5, 0, 0, 0);
            }
            if (this.filterFoldoutStyle == null)
            {
                this.filterFoldoutStyle = new GUIStyle(EditorStyles.foldout);
                this.filterFoldoutStyle.margin = new RectOffset(0, 0, 0, 5);
            }

            if (this.filterEnabledToggleStyle == null)
            {
                this.filterEnabledToggleStyle = new GUIStyle(EditorStyles.miniButton);
                this.filterEnabledToggleStyle.alignment = TextAnchor.MiddleCenter;
            }

        }

        public void AbstractGUITick()
        {
            this.Init();
            EditorGUILayout.BeginVertical(this.filterContainerStyle);
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            EditorGUI.BeginChangeCheck();
            var isFilterEnabled = EditorGUILayout.Toggle(this.isFilterEnabled, this.filterEnabledToggleStyle, GUILayout.Width(10), GUILayout.Height(10));
            if (EditorGUI.EndChangeCheck())
            {
                ConfigurationEditorUndoHelper.RecordUndo();
                this.SetIsFilterEnabled(isFilterEnabled);
            }

            EditorGUI.BeginChangeCheck();
            var foldoutHeader = this.fieldName + " -- type : ";
            if (fieldType.IsEnum)
            {
                foldoutHeader += "(enum)";
            }
            foldoutHeader += this.fieldType.Name;
            var filterFoldout = EditorGUILayout.Foldout(this.filterFoldout, foldoutHeader, true, this.filterFoldoutStyle);
            if (EditorGUI.EndChangeCheck())
            {
                ConfigurationEditorUndoHelper.RecordUndo();
                this.filterFoldout = filterFoldout;
            }
            EditorGUILayout.EndHorizontal();
            if (this.filterFoldout)
            {
                EditorGUI.indentLevel += 1;
                this.GUITick();
                EditorGUI.indentLevel -= 1;
            }
            EditorGUILayout.EndVertical();

        }
        protected abstract void GUITick();

        public bool IsFilterEnabled()
        {
            return this.isFilterEnabled;
        }

        public bool ComputeObjectFilters(object objectValue)
        {
            var retrievedValue = objectValue.GetType().GetProperty(this.fieldName).GetValue(objectValue);
            if (retrievedValue != null)
            {
                return this.ComputeFieldFilters(retrievedValue);
            }
            return false;
        }

        protected abstract bool ComputeFieldFilters(object fieldValue);

        public void DeactivateFilter()
        {
            this.SetIsFilterEnabled(false);
        }

        private void SetIsFilterEnabled(bool newValue)
        {
            if (!newValue)
            {
                this.OnFilterDisabled();
            }
            this.isFilterEnabled = newValue;
        }

        protected abstract void OnFilterDisabled();

        internal void ColapseFilter()
        {
            this.filterFoldout = false;
        }
    }

}

#endif //UNITY_EDITOR