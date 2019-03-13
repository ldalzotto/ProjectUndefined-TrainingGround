#if UNITY_EDITOR
using ExpressionEvaluation;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ConfigurationEditor
{
    [System.Serializable]
    public class NativeTypeDictionaryEnumFilter : AbstractDictionaryEnumGUIFilter
    {
        [SerializeField]
        private ExpressionFilterOperation filterOperation;

        public NativeTypeDictionaryEnumFilter()
        {
        }

        public NativeTypeDictionaryEnumFilter(PropertyInfo propInfo) : base(propInfo)
        {
            this.filterOperation = new ExpressionFilterOperation();
        }

        protected override void GUITick()
        {
            EditorGUILayout.BeginHorizontal();
            this.filterOperation.GUITick(this.fieldName);
            EditorGUILayout.EndHorizontal();
        }

        protected override bool ComputeFieldFilters(object fieldValue)
        {
            return this.filterOperation.ComputeOperation(fieldValue);
        }

        protected override void OnFilterDisabled()
        {
            this.filterOperation.ClearGUIComponent();
        }
    }
}


#endif //UNITY_EDTIRO