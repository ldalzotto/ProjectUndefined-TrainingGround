#if UNITY_EDITOR
using ExpressionEvaluation;
using System;
using UnityEditor;
using UnityEngine;

namespace ConfigurationEditor
{
    [System.Serializable]
    public class ExpressionFilterOperation
    {
        private ExpressionEval expressionEvaluation;

        [SerializeField]
        private string compareExpression;
        [SerializeField]
        private string fieldName;

        private GUIStyle errorMessageStyle;
        private string errorMessage = String.Empty;

        public ExpressionFilterOperation()
        {
            this.errorMessage = String.Empty;
        }

        private void Init()
        {
            if (this.errorMessageStyle == null)
            {
                this.errorMessageStyle = new GUIStyle(EditorStyles.label);
                this.errorMessageStyle.normal.textColor = Color.red;
            }
            if (this.expressionEvaluation == null && this.compareExpression != null)
            {
                this.expressionEvaluation = new ExpressionEval(this.compareExpression);
            }
        }

        public bool ComputeOperation(object value)
        {
            if (this.expressionEvaluation != null)
            {
                if (value.GetType().IsEnum || value.GetType() == typeof(String) || value.GetType() == typeof(string))
                {
                    this.expressionEvaluation.SetVariable(fieldName, value.ToString());
                }
                else
                {
                    this.expressionEvaluation.SetVariable(fieldName, value);
                }
                this.errorMessage = String.Empty;

                try
                {
                    bool evaluation = this.expressionEvaluation.EvaluateBool();
                    return evaluation;
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                    this.errorMessage = e.Message;
                }
            }

            return false;
        }

        public void GUITick(string fieldName)
        {
            this.Init();
            this.fieldName = fieldName;
            EditorGUILayout.BeginVertical();
            EditorGUI.BeginChangeCheck();
            var nextFrameCompareExpression = EditorGUILayout.TextField(this.compareExpression, EditorStyles.textField);
            if (EditorGUI.EndChangeCheck())
            {
                if (nextFrameCompareExpression != this.compareExpression)
                {
                    ConfigurationEditorUndoHelper.RecordUndo();
                    this.compareExpression = nextFrameCompareExpression;
                    this.expressionEvaluation = new ExpressionEval(this.compareExpression);
                }
            }

            if (this.errorMessage != String.Empty)
            {
                EditorGUILayout.LabelField(this.errorMessage, this.errorMessageStyle);
            }

            EditorGUILayout.EndVertical();
        }

        public void ClearGUIComponent()
        {
            this.errorMessage = String.Empty;
        }

    }
}

#endif //UNITY_EDITOR