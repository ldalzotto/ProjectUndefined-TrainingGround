using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Inline))]
public class InlinePropertyDrawer : PropertyDrawer
{
    private bool folded;
    private Editor inlineEditor;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        try
        {
            Inline byEnumProperty = (Inline)attribute;

            EditorGUI.BeginProperty(position, label, property);
            EditorGUILayout.BeginVertical(EditorStyles.textArea);
            this.folded = EditorGUILayout.Foldout(this.folded, property.name, true);

            if (this.folded)
            {
                EditorGUI.indentLevel += 1;
                EditorGUILayout.PropertyField(property);
                EditorGUI.indentLevel -= 1;

                if (property.objectReferenceValue != null)
                {
                    if (this.inlineEditor == null)
                    {
                        inlineEditor = Editor.CreateEditor(property.objectReferenceValue);
                    }
                    if (this.inlineEditor != null)
                    {
                        EditorGUI.indentLevel += 1;
                        this.inlineEditor.OnInspectorGUI();
                        EditorGUI.indentLevel -= 1;
                    }
                }
                else
                {
                    if (EditorUtility.IsPersistent(property.serializedObject.targetObject))
                    {
                        if (byEnumProperty.CreateSubIfAbsent)
                        {
                            GUILayout.Button(new GUIContent(""), EditorStyles.miniButton);
                            bool toCreate = true;
                            var ActualAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(property.serializedObject.targetObject)).ToList();

                            foreach (var ActualAsset in ActualAssets)
                            {
                                if (ActualAsset != null)
                                {
                                    if (ActualAsset.name == byEnumProperty.FileName)
                                    {
                                        toCreate = false;
                                        property.objectReferenceValue = ActualAsset;
                                        break;
                                    }
                                }
                            }


                            if (toCreate)
                            {
                                this.CreateByEnumSO(property, byEnumProperty);
                            }
                        }
                        else if (byEnumProperty.CreateAtSameLevelIfAbsent)
                        {
                            if (GUILayout.Button(new GUIContent("CREATE")))
                            {
                                var createdAsset = AssetHelper.CreateAssetAtSameDirectoryLevel((ScriptableObject)property.serializedObject.targetObject, property.type.Replace("PPtr<$", "").Replace(">", ""), property.name);

                                property.objectReferenceValue = (UnityEngine.Object)createdAsset;
                            }
                        }

                    }
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUI.EndProperty();
        }
        catch (Exception e) { Debug.LogError(e); }

    }

    private void CreateByEnumSO(SerializedProperty property, Inline inlineAttribute)
    {
        this.ClearObject(property);
        var sanitizedType = property.type.Replace("PPtr<$", "").Replace(">", "");
        var instanciatedProperty = ScriptableObject.CreateInstance(sanitizedType);
        instanciatedProperty.name = inlineAttribute.FileName;
        AssetDatabase.AddObjectToAsset(instanciatedProperty, property.serializedObject.targetObject);
        property.objectReferenceValue = instanciatedProperty;
    }

    private void ClearObject(SerializedProperty property)
    {
        List<object> ObjectsByEnum = new List<object>();
        foreach (var field in property.serializedObject.targetObject.GetType().GetFields())
        {
            var inlinesAttribute = field.GetCustomAttributes(typeof(Inline), true).Cast<Inline>().ToArray();
            if (inlinesAttribute != null && inlinesAttribute.Length > 0)
            {
                ObjectsByEnum.Add(field.GetValue(property.serializedObject.targetObject));
            }
        }
        var ActualAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(property.serializedObject.targetObject)).ToList();

        ActualAssets.Remove(property.serializedObject.targetObject);

        foreach (var ActualAsset in ActualAssets)
        {
            if (ActualAsset != null)
            {
                if (!ObjectsByEnum.Contains(ActualAsset))
                {
                    AssetDatabase.RemoveObjectFromAsset(ActualAsset);
                }
            }
        }

    }
}
