#if UNITY_EDITOR
using ConfigurationEditor;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TypeHelper
{
    public static Type[] GetAllTypeAssignableFrom(Type abstractType)
    {
        if (abstractType.IsGenericType)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
                  .Where(p => IsAssignableToGenericType(p, abstractType))
                  .Where(p => p.Name != abstractType.Name)
                  .ToArray();
        }
        else
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
                  .Where(p => abstractType.IsAssignableFrom(p))
                  .Where(p => p.Name != abstractType.Name)
                  .ToArray();
        }

    }

    public static Type[] GetAllGameConfigurationTypes()
    {
        return
            typeof(IConfigurationSerialization)
                   .Assembly.GetTypes()
                   .Union(typeof(RTPuzzle.GameManager).Assembly.GetTypes())
                   .Union(typeof(AdventureGame.GameManager).Assembly.GetTypes())
                   .Where(t => typeof(IConfigurationSerialization).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface).ToArray();
    }

    public static Type GetType(string typeName)
    {
        var type = Type.GetType(typeName);
        if (type != null) return type;
        foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
        {
            type = a.GetType(typeName);
            if (type != null)
                return type;
        }
        return null;
    }

    public static bool IsAssignableToGenericType(Type givenType, Type genericType)
    {
        if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            return true;

        Type baseType = givenType.BaseType;
        if (baseType == null) return false;

        return IsAssignableToGenericType(baseType, genericType);
    }


}

public class TypeSelectionerManager
{
    private string label;
    private Type[] elligibleTypes;

    public void OnEnable(Type abstractManagerType, string label)
    {
        this.elligibleTypes = TypeHelper.GetAllTypeAssignableFrom(abstractManagerType);
        this.label = label;
    }

    public Type OnInspectorGUI(Type SelectedManagerType)
    {
        return this.OnInspectorGUI(SelectedManagerType, null);
    }

    public Type OnInspectorGUI(Type SelectedManagerType, Func<Type, string> getDescriptionFunc)
    {
        Type changedType = null;
        var selectedIndex = 0;
        if (SelectedManagerType != null)
        {
            var eligibleT = this.elligibleTypes.ToList().Select(t => t).Where(t => t.AssemblyQualifiedName == SelectedManagerType.AssemblyQualifiedName);
            if (eligibleT != null && eligibleT.Count() > 0)
            {
                selectedIndex = this.elligibleTypes.ToList().IndexOf(eligibleT.First());
            }
        }


        EditorGUI.BeginChangeCheck();
        selectedIndex = EditorGUILayout.Popup(new GUIContent(this.label), selectedIndex, this.elligibleTypes.ToList().ConvertAll(t => t.Name).ToArray());
        if (EditorGUI.EndChangeCheck())
        {
            SelectedManagerType = this.elligibleTypes[selectedIndex];
            changedType = SelectedManagerType;
        }

        //Initialisation
        if (SelectedManagerType == null)
        {
            SelectedManagerType = this.elligibleTypes[selectedIndex];
            changedType = SelectedManagerType;
        }

        if (SelectedManagerType != null && getDescriptionFunc != null)
        {
            var description = getDescriptionFunc.Invoke(SelectedManagerType);
            var gContent = new GUIContent(description, description);
            GUILayout.Label(gContent, EditorStyles.miniLabel);
        }

        return changedType;

    }

}
#endif //UNITY_EDITOR