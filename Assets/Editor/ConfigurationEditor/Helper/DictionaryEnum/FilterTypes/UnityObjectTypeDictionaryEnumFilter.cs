#if UNITY_EDITOR

using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ConfigurationEditor
{
    [System.Serializable]
    public class UnityObjectTypeDictionaryEnumFilter : AbstractDictionaryEnumGUIFilter
    {
        [SerializeField]
        private bool isNotNull;
        [SerializeField]
        private ExpressionFilterOperation nameExpressionFilter;

        public UnityObjectTypeDictionaryEnumFilter() { }
        public UnityObjectTypeDictionaryEnumFilter(PropertyInfo propInfo) : base(propInfo)
        {
            this.nameExpressionFilter = new ExpressionFilterOperation();
        }

        protected override bool ComputeFieldFilters(object fieldValue)
        {
            bool isFiltered = EvaluateNullFilter(fieldValue);
            if (!isFiltered)
            {
                var unityObj = (Object)fieldValue;
                if (unityObj != null)
                {
                    isFiltered = this.nameExpressionFilter.ComputeOperation(unityObj.name);
                }

            }
            return isFiltered;
        }

        private bool EvaluateNullFilter(object fieldValue)
        {
            if (this.isNotNull)
            {
                if (fieldValue != null && fieldValue.ToString() != "null")
                {
                    return true;
                }
            }
            else
            {
                if (fieldValue == null || fieldValue.ToString() == "null")
                {
                    return true;
                }
            }

            return false;
        }

        protected override void GUITick()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField(this.fieldName + " not null : ", EditorStyles.label, GUILayout.ExpandWidth(false));
            this.isNotNull = GUILayout.Toggle(this.isNotNull, "O", EditorStyles.miniButton, GUILayout.Width(25));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            this.nameExpressionFilter.GUITick(this.fieldName);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        protected override void OnFilterDisabled()
        {
            this.nameExpressionFilter.ClearGUIComponent();
        }
    }
}

#endif //UNITY_EDITOR