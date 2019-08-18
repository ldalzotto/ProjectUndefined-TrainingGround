using RTPuzzle;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
[CustomEditor(typeof(RangeTypeObjectDefinitionConfigurationInherentData))]
public class RangeDefinitionCustomEditor : Editor
{

    private Dictionary<Type, Editor> cachedEditors = new Dictionary<Type, Editor>();

    public override void OnInspectorGUI()
    {
        RangeTypeObjectDefinitionConfigurationInherentData RangeDefinitionTarget = (RangeTypeObjectDefinitionConfigurationInherentData)target;
        this.DoInit(RangeDefinitionTarget);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginVertical();
        foreach (var rangeDefinitionModule in RangeDefinitionTarget.RangeDefinitionModulesActivation.Keys.ToList())
        {
            EditorGUILayout.BeginHorizontal();
            RangeDefinitionTarget.RangeDefinitionModulesActivation[rangeDefinitionModule] = EditorGUILayout.Toggle(RangeDefinitionTarget.RangeDefinitionModulesActivation[rangeDefinitionModule]);
            EditorGUILayout.LabelField(rangeDefinitionModule.Name);
            EditorGUILayout.EndHorizontal();

            if (RangeDefinitionTarget.RangeDefinitionModulesActivation[rangeDefinitionModule])
            {
                if (!RangeDefinitionTarget.RangeDefinitionModules.ContainsKey(rangeDefinitionModule) || RangeDefinitionTarget.RangeDefinitionModules[rangeDefinitionModule] == null)
                {
                    if (GUILayout.Button("CREATE"))
                    {
                        RangeDefinitionTarget.RangeDefinitionModules[rangeDefinitionModule] = AssetHelper.CreateAssetAtSameDirectoryLevel(RangeDefinitionTarget, rangeDefinitionModule, rangeDefinitionModule.Name);
                    }
                }
                else
                {
                    if (!cachedEditors.ContainsKey(rangeDefinitionModule)) { cachedEditors[rangeDefinitionModule] = Editor.CreateEditor(RangeDefinitionTarget.RangeDefinitionModules[rangeDefinitionModule]); }
                    cachedEditors[rangeDefinitionModule].OnInspectorGUI();
                }
            }
        }
        EditorGUILayout.EndVertical();

        if (EditorGUI.EndChangeCheck())
        {
            this.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(this.target);
        }
    }

    private void DoInit(RangeTypeObjectDefinitionConfigurationInherentData RangeDefinitionTarget)
    {
        if (RangeDefinitionTarget.RangeDefinitionModulesActivation == null)
        {
            RangeDefinitionTarget.RangeDefinitionModulesActivation = new Dictionary<Type, bool>();
            RangeTypeObjectDefinitionConfigurationInherentData.RangeModuleTypes.ForEach((moduleDefinitionType) =>
            {
                RangeDefinitionTarget.RangeDefinitionModulesActivation[moduleDefinitionType] = false;
            });
        }
        else
        {
            if (RangeDefinitionTarget.RangeDefinitionModulesActivation.Count != RangeTypeObjectDefinitionConfigurationInherentData.RangeModuleTypes.Count)
            {
                RangeTypeObjectDefinitionConfigurationInherentData.RangeModuleTypes.ForEach((moduleDefinitionType) =>
                {
                    if (!RangeDefinitionTarget.RangeDefinitionModulesActivation.Keys.ToList().ConvertAll(k => k.GetType()).Contains(moduleDefinitionType))
                    {
                        RangeDefinitionTarget.RangeDefinitionModulesActivation[moduleDefinitionType] = false;
                    }
                });
            }
        }

        if (RangeDefinitionTarget.RangeDefinitionModules == null) { RangeDefinitionTarget.RangeDefinitionModules = new Dictionary<Type, ScriptableObject>(); }
    }
}