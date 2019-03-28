using System;
using System.Collections.Generic;
using UnityEditor;

namespace RTPuzzle
{
    [CustomEditor(typeof(AIComponents))]
    public class AIComponentsEditor : Editor
    {

        private Dictionary<string, Editor> cachedEditors = new Dictionary<string, Editor>();

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            SerializedProperty prop = serializedObject.GetIterator();
            prop.NextVisible(true);
            do
            {
                if (prop != null)
                {
                    var propertyFieldInfo = SerializableObjectHelper.GetFieldInfo(prop);
                    if (propertyFieldInfo != null)
                    {
                        var propertyType = propertyFieldInfo.FieldType;
                        if (propertyType != null && propertyType.IsSubclassOf(typeof(AbstractAIComponent)))
                        {
                            prop.objectReferenceValue = EditorGUILayout.ObjectField(propertyType.Name, prop.objectReferenceValue, propertyType, false);
                            if (prop.objectReferenceValue != null)
                            {
                                this.DisplayNestedEditor(propertyType, prop.objectReferenceValue).OnInspectorGUI();
                            }
                            EditorGUILayout.Separator();
                            EditorGUILayout.Separator();
                        }
                    }

                }

            } while (prop.NextVisible(false));

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }

        private Editor DisplayNestedEditor(Type windowType, UnityEngine.Object obj)
        {
            if (!this.cachedEditors.ContainsKey(windowType.Name) || this.cachedEditors[windowType.Name] == null)
            {
                this.cachedEditors[windowType.Name] = Editor.CreateEditor(obj);
            }
            return this.cachedEditors[windowType.Name];
        }

    }

}
