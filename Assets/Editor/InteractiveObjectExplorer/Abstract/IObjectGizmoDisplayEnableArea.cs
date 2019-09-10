using CoreGame;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class IObjectGizmoDisplayEnableArea
{
    private bool isEnabled;
    private string label;
    private Dictionary<Type, AdditionalEnumParameter> AdditionalEnumerationParameters;

    public IObjectGizmoDisplayEnableArea(bool isEnabled, string label, List<AdditionalEnumParameter> AdditionalEnumerationParameters = null)
    {
        this.isEnabled = isEnabled;
        this.label = label;

        if (AdditionalEnumerationParameters != null)
        {
            this.AdditionalEnumerationParameters = new Dictionary<Type, AdditionalEnumParameter>();
            foreach (var enumParam in AdditionalEnumerationParameters)
            {
                this.AdditionalEnumerationParameters.Add(enumParam.byEnumPropertyType, enumParam);
            }
        }
    }

    public bool IsEnabled { get => isEnabled; }

    public void OnGUI(Action guiAction)
    {
        var areaRect = EditorGUILayout.BeginVertical(EditorStyles.textArea);
        EditorGUILayout.BeginHorizontal();
        this.isEnabled = EditorGUILayout.Toggle(this.isEnabled, GUILayout.Width(30f));
        EditorGUILayout.LabelField(this.label);

        if (this.AdditionalEnumerationParameters != null)
        {
            foreach (var additionalEnumerationParameter in this.AdditionalEnumerationParameters.ToList())
            {
                this.AdditionalEnumerationParameters[additionalEnumerationParameter.Key].SelectedEnum = EditorGUILayout.EnumPopup(new GUIContent(""), this.AdditionalEnumerationParameters[additionalEnumerationParameter.Key].SelectedEnum, (value) => additionalEnumerationParameter.Value.AvailableEnums.Contains(value), false);
            }

        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        
        if (this.isEnabled && guiAction != null)
        {
            guiAction.Invoke();
        }
    }

    public Enum GetEnumParameter<T>() where T : IByEnumProperty
    {
        return this.AdditionalEnumerationParameters[typeof(T)].SelectedEnum;
    }
}


public class AdditionalEnumParameter
{
    public Type byEnumPropertyType;
    public List<Enum> AvailableEnums;
    public Enum SelectedEnum;

    public AdditionalEnumParameter(Type byEnumPropertyType, Type enumType, List<Enum> availableEnums)
    {
        this.byEnumPropertyType = byEnumPropertyType;
        AvailableEnums = availableEnums;
        var enumerator = Enum.GetValues(enumType).GetEnumerator();
        enumerator.MoveNext();
        this.SelectedEnum = (Enum)enumerator.Current;
    }
}