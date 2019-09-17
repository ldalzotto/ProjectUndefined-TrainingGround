using CoreGame;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
[CustomEditor(typeof(AbstractObjectDefinitionConfigurationInherentData), true)]
public class ObjectDefinitionCustomEditor : Editor
{
    private Dictionary<Type, FoldableArea> fold = new Dictionary<Type, FoldableArea>();
    private Dictionary<Type, Editor> cachedEditors = new Dictionary<Type, Editor>();

    private IObjectDefinitionCustomEditorEventListener Listener;

    public void RegisterListener(IObjectDefinitionCustomEditorEventListener objectDefinitionCustomEditorEventListener)
    {
        this.Listener = objectDefinitionCustomEditorEventListener;
    }

    public override void OnInspectorGUI()
    {
        AbstractObjectDefinitionConfigurationInherentData RangeDefinitionTarget = (AbstractObjectDefinitionConfigurationInherentData)target;
        this.DoInit(RangeDefinitionTarget);


        EditorGUI.BeginChangeCheck();
        this.Listener.IfNotNull(Listener => Listener.BeforeOnInspectorGUI());
        EditorGUILayout.BeginVertical();
        foreach (var rangeDefinitionModule in RangeDefinitionTarget.RangeDefinitionModulesActivation.Keys.ToList())
        {
            if (!fold.ContainsKey(rangeDefinitionModule))
            {
                fold[rangeDefinitionModule] = new FoldableArea(true, rangeDefinitionModule.Name, RangeDefinitionTarget.RangeDefinitionModulesActivation[rangeDefinitionModule]);
            }
            fold[rangeDefinitionModule].OnGUI(() =>
            {
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
            });

            RangeDefinitionTarget.RangeDefinitionModulesActivation[rangeDefinitionModule] = fold[rangeDefinitionModule].IsEnabled;

        }
        EditorGUILayout.EndVertical();

        if (EditorGUI.EndChangeCheck())
        {
            this.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(this.target);
        }
    }

    private void DoInit(AbstractObjectDefinitionConfigurationInherentData RangeDefinitionTarget)
    {
        if (RangeDefinitionTarget.RangeDefinitionModulesActivation == null)
        {
            RangeDefinitionTarget.RangeDefinitionModulesActivation = new Dictionary<Type, bool>();
            RangeDefinitionTarget.ModuleTypes.ForEach((moduleDefinitionType) =>
            {
                RangeDefinitionTarget.RangeDefinitionModulesActivation[moduleDefinitionType] = false;
            });
        }
        else
        {
            if (RangeDefinitionTarget.RangeDefinitionModulesActivation.Count != RangeDefinitionTarget.ModuleTypes.Count)
            {
                RangeDefinitionTarget.ModuleTypes.ForEach((moduleDefinitionType) =>
                {
                    if (!RangeDefinitionTarget.RangeDefinitionModulesActivation.Keys.ToList().Contains(moduleDefinitionType))
                    {
                        RangeDefinitionTarget.RangeDefinitionModulesActivation[moduleDefinitionType] = false;
                    }
                });

                RangeDefinitionTarget.RangeDefinitionModulesActivation.Keys.ToList().ForEach((definitionModuleType) =>
                {
                    if (!RangeDefinitionTarget.ModuleTypes.Contains(definitionModuleType))
                    {
                        RangeDefinitionTarget.RangeDefinitionModulesActivation.Remove(definitionModuleType);
                        RangeDefinitionTarget.RangeDefinitionModules.Remove(definitionModuleType);
                    }
                });
            }
        }

        if (RangeDefinitionTarget.RangeDefinitionModules == null) { RangeDefinitionTarget.RangeDefinitionModules = new Dictionary<Type, ScriptableObject>(); }
    }
}

public interface IObjectDefinitionCustomEditorEventListener
{
    void BeforeOnInspectorGUI();
}