#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using System.Reflection;

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

    public static FieldInfo GetFieldInfo(SerializedProperty prop)
    {
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

}
#endif