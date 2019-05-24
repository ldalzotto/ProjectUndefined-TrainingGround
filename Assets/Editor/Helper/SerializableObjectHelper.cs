﻿#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System;

public class SerializableObjectHelper
{

    public static T GetBaseProperty<T>(SerializedProperty prop)
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
        return (T)reflectionTarget;
    }

    public static SerializedProperty GetParentProperty(SerializedProperty prop)
    {
        // Separate the steps it takes to get to this property
        string[] separatedPaths = prop.propertyPath.Split('.');

        if (separatedPaths.Length > 1)
        {
            string parentPropertypath = string.Empty;
            for (var i = 0; i < separatedPaths.Length - 1; i++)
            {
                if (i != 0)
                {
                    parentPropertypath += ".";
                }
                parentPropertypath += separatedPaths[i];
            }
            return prop.serializedObject.FindProperty(parentPropertypath);

        }
        return null;

    }

    public static int GetArrayIndex(SerializedProperty elementProperty)
    {
        // Separate the steps it takes to get to this property
        string[] separatedPaths = elementProperty.propertyPath.Split('.');

        if (separatedPaths.Length > 0)
        {
            var elementArrayPath = separatedPaths[separatedPaths.Length - 1];
            return int.Parse(elementArrayPath.Replace("data[", "").Replace("]", ""));
        }
        return -1;
    }

    public static FieldInfo GetFieldInfo(SerializedProperty prop)
    {
        if (prop == null)
        {
            return null;
        }

        // Separate the steps it takes to get to this property
        string[] separatedPaths = prop.propertyPath.Split('.');

        // Go down to the root of this serialized property
        System.Object reflectionTarget = prop.serializedObject.targetObject as object;
        FieldInfo fieldInfo = null;
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
                fieldInfo = reflectionTarget.GetType().GetField(separatedPaths[i]);
                if (fieldInfo != null)
                {
                    reflectionTarget = fieldInfo.GetValue(reflectionTarget);
                }

            }

        }
        return fieldInfo;
    }

    public static void SetArray(List<Enum> list, SerializedProperty prop)
    {
        prop.ClearArray();
        for (var i = 0; i < list.Count; i++)
        {
            prop.InsertArrayElementAtIndex(0);
            prop.GetArrayElementAtIndex(0).enumValueIndex = (int)((object)list[i]);
        }
    }

    public static void Modify(UnityEngine.Object obj, Action<SerializedObject> modification)
    {
        var so = new SerializedObject(obj);
        modification.Invoke(so);
        so.ApplyModifiedProperties();
    }
}
#endif