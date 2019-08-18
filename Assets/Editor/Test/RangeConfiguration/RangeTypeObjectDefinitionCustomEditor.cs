using RTPuzzle;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RangeTypeDefinition))]
[System.Serializable]
public class RangeTypeObjectDefinitionCustomEditor : Editor
{
    private List<Type> AvailableRangeShapeConfigurationTypes;
    private Type SelectedType;

    private void OnEnable()
    {
        if (target != null)
        {
            this.AvailableRangeShapeConfigurationTypes = TypeHelper.GetAllTypeAssignableFrom(typeof(RangeShapeConfiguration)).ToList();

            RangeTypeDefinition RangeTypeDefinitionTarget = (RangeTypeDefinition)target;
            if (RangeTypeDefinitionTarget.RangeShapeConfiguration != null)
            {
                this.SelectedType = RangeTypeDefinitionTarget.RangeShapeConfiguration.GetType();
            }
            else
            {
                this.SelectedType = this.AvailableRangeShapeConfigurationTypes[0];
            }

            if (RangeTypeDefinitionTarget.RangeShapeConfiguration == null)
            {
                RangeTypeDefinitionTarget.RangeShapeConfiguration = (RangeShapeConfiguration)AssetHelper.CreateAssetAtSameDirectoryLevel(RangeTypeDefinitionTarget, this.SelectedType, "Range");
            }
        }
    }

    public override void OnInspectorGUI()
    {
        RangeTypeDefinition RangeTypeDefinitionTarget = (RangeTypeDefinition)target;

        EditorGUI.BeginChangeCheck();
        EditorGUI.BeginChangeCheck();
        SelectedType = AvailableRangeShapeConfigurationTypes[EditorGUILayout.Popup(this.AvailableRangeShapeConfigurationTypes.IndexOf(SelectedType), this.AvailableRangeShapeConfigurationTypes.ConvertAll(t => t.Name).ToArray())];
        if (EditorGUI.EndChangeCheck())
        {
            if (RangeTypeDefinitionTarget.RangeShapeConfiguration.GetType() != this.SelectedType)
            {
                RangeTypeDefinitionTarget.RangeShapeConfiguration = (RangeShapeConfiguration)AssetHelper.CreateAssetAtSameDirectoryLevel(RangeTypeDefinitionTarget, this.SelectedType, "Range");
            }
        }

        EditorGUILayout.PropertyField(this.serializedObject.FindProperty(nameof(RangeTypeDefinitionTarget.RangeTypeID)), true);
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty(nameof(RangeTypeDefinitionTarget.RangeShapeConfiguration)), true);
        if (EditorGUI.EndChangeCheck())
        {
            this.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }
}
