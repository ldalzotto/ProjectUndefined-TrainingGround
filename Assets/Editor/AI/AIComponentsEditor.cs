using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace RTPuzzle
{
    [CustomEditor(typeof(GenericPuzzleAIComponents))]
    public class AIComponentsEditor : Editor
    {

        private Dictionary<FieldInfo, Editor> cachedEditors = new Dictionary<FieldInfo, Editor>();

        public override void OnInspectorGUI()
        {
            EditorGUILayout.ObjectField("Script:", MonoScript.FromScriptableObject((ScriptableObject)target), typeof(MonoScript), false);
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
                            prop.objectReferenceValue = EditorGUILayout.ObjectField(propertyFieldInfo.Name, prop.objectReferenceValue, propertyType, false);
                            if (prop.objectReferenceValue != null)
                            {
                                this.DisplayNestedEditor(propertyFieldInfo, prop.objectReferenceValue).OnInspectorGUI();
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

        private Editor DisplayNestedEditor(FieldInfo windowType, UnityEngine.Object obj)
        {
            if (!this.cachedEditors.ContainsKey(windowType) || this.cachedEditors[windowType] == null)
            {
                this.cachedEditors[windowType] = Editor.CreateEditor(obj);
            }
            return this.cachedEditors[windowType];
        }
    }



}
