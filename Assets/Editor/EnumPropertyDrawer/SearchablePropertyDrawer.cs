﻿using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SearchableEnum))]
public class SearchablePropertyDrawer : PropertyDrawer
{

    private Rect enumPopupRect;
    private EnumSearchGUIWindow windowInstance;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SearchableEnum searchableEnum = (SearchableEnum)attribute;
        if (property.propertyType == SerializedPropertyType.Enum)
        {
            var fullRect = new Rect(position);
            var targetEnum = GetBaseProperty<Enum>(property);

            var labelFieldRect = new Rect(position.x, position.y, position.width / 2, position.height);
            EditorGUI.LabelField(labelFieldRect, label);
            var enumPopupRect = new Rect(position.x + position.width / 2, position.y, position.width / 2, position.height);
            if (EditorGUI.DropdownButton(enumPopupRect, new GUIContent(targetEnum.ToString()), FocusType.Keyboard))
            {
                if (windowInstance == null)
                {
                    windowInstance = EditorWindow.CreateInstance<EnumSearchGUIWindow>();
                    windowInstance.Init(targetEnum, (newSelectedEnum) =>
                    {
                        property.longValue = (int)Convert.ChangeType(newSelectedEnum, newSelectedEnum.GetTypeCode());
                        property.serializedObject.ApplyModifiedProperties();
                        property.serializedObject.Update();
                        EditorUtility.SetDirty(property.serializedObject.targetObject);
                    });
                }

                var windowRect = new Rect(GUIUtility.GUIToScreenPoint(enumPopupRect.position), new Vector2(0, enumPopupRect.height));
                windowInstance.ShowAsDropDown(windowRect, new Vector2(enumPopupRect.width, 500));
            }

        }


    }

    static T GetBaseProperty<T>(SerializedProperty prop)
    {
        // Separate the steps it takes to get to this property
        string[] separatedPaths = prop.propertyPath.Split('.');

        // Go down to the root of this serialized property
        System.Object reflectionTarget = prop.serializedObject.targetObject as object;
        // Walk down the path to get the target object
        for (var i = 0; i < separatedPaths.Length; i++)
        {
            var arrayIndex = -1;
            if (separatedPaths[i] == "Array")
            {
                //we get the index
                if (separatedPaths.Length > i && separatedPaths[i + 1] != null)
                {
                    arrayIndex = int.Parse(separatedPaths[i + 1].Replace("data[", "").Replace("]", ""));
                }

            }
            if (arrayIndex != -1)
            {
                i += 1;
                reflectionTarget = ((IEnumerable)reflectionTarget).Cast<object>().ToList()[arrayIndex];
            }
            else
            {
                FieldInfo fieldInfo = reflectionTarget.GetType().GetField(separatedPaths[i]);
                reflectionTarget = fieldInfo.GetValue(reflectionTarget);
            }

        }
        foreach (var path in separatedPaths)
        {

        }
        return (T)reflectionTarget;
    }


}
