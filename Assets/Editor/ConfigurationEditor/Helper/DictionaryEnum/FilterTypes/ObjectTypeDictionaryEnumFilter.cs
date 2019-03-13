#if UNITY_EDITOR

using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ConfigurationEditor
{
    [System.Serializable]
    public class ObjectTypeDictionaryEnumFilter : AbstractDictionaryEnumGUIFilter
    {
        [SerializeField]
        private bool isNotNull;
        public ObjectTypeDictionaryEnumFilter() { }
        public ObjectTypeDictionaryEnumFilter(PropertyInfo propInfo) : base(propInfo)
        { }

        protected override bool ComputeFieldFilters(object fieldValue)
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
            EditorGUILayout.EndVertical();
        }

        protected override void OnFilterDisabled()
        {

        }
    }
}

#endif //UNITY_EDITOR