#if UNITY_EDITOR
using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace ConfigurationEditor
{
    [System.Serializable]
    public class RegexFilterOperation
    {

        [SerializeField]
        private string regexString;
        private Regex regex;

        private string errorMessage = String.Empty;
        private GUIStyle errorMessageStyle;

        public void Init()
        {
            if (this.errorMessageStyle == null)
            {
                this.errorMessageStyle = new GUIStyle(EditorStyles.label);
                this.errorMessageStyle.normal.textColor = Color.red;
            }
        }

        public void GUiTick()
        {
            this.Init();
            EditorGUILayout.BeginVertical();
            EditorGUI.BeginChangeCheck();
            try
            {
                var nextRegexString = EditorGUILayout.TextField(this.regexString, EditorStyles.textField);
                if (EditorGUI.EndChangeCheck())
                {
                    ConfigurationEditorUndoHelper.RecordUndo();
                    this.regexString = nextRegexString;
                    this.regex = new Regex(this.regexString);
                }
            }
            catch (Exception e)
            {
                this.errorMessage = e.Message;
                Debug.LogWarning(e);
            }


            if (this.errorMessage != String.Empty)
            {
                EditorGUILayout.LabelField(this.errorMessage, this.errorMessageStyle);
            }

            EditorGUILayout.EndVertical();
        }

        internal void ClearGUIComponent()
        {
            this.errorMessage = String.Empty;
        }

        public bool ComputeOperation(string value)
        {

            try
            {
                this.errorMessage = String.Empty;
                if (this.regex == null)
                {
                    this.regex = new Regex(this.regexString);
                }
                MatchCollection matches = this.regex.Matches(value);
                return (matches != null && matches.Count > 0);
            }
            catch (Exception e)
            {
                this.errorMessage = e.Message;
                Debug.LogWarning(e);
            }
            return false;
        }
    }
}

#endif //UNITY_EDITOR