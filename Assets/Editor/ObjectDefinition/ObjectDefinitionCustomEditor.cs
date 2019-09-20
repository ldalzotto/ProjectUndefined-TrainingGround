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
    [SerializeField]
    private ObjectDefinitionCutstomEditorProfile ObjectDefinitionCutstomEditorProfile;

    private Dictionary<string, List<Type>> RangeDefinitionModulesActivationGrouped;

    private IObjectDefinitionCustomEditorEventListener Listener;

    public void RegisterListener(IObjectDefinitionCustomEditorEventListener objectDefinitionCustomEditorEventListener)
    {
        this.Listener = objectDefinitionCustomEditorEventListener;
    }

    public virtual void OnEnable()
    {
        if (this.ObjectDefinitionCutstomEditorProfile == null)
        {
            this.ObjectDefinitionCutstomEditorProfile = new ObjectDefinitionCutstomEditorProfile();
        }
        this.RangeDefinitionModulesActivationGrouped = new Dictionary<string, List<Type>>();
    }

    public override void OnInspectorGUI()
    {
        this.RangeDefinitionModulesActivationGrouped.Clear();

        AbstractObjectDefinitionConfigurationInherentData RangeDefinitionTarget = (AbstractObjectDefinitionConfigurationInherentData)target;
        this.DoInit(RangeDefinitionTarget);

        EditorGUI.BeginChangeCheck();
        this.Listener.IfNotNull(Listener => Listener.BeforeOnInspectorGUI());

        EditorGUILayout.BeginHorizontal(GUILayout.Width(20f));

        this.ObjectDefinitionCutstomEditorProfile.ShowInactives =
            GUILayout.Toggle(this.ObjectDefinitionCutstomEditorProfile.ShowInactives, new GUIContent("S", "Show inactives."), EditorStyles.miniButton);

        EditorGUILayout.EndHorizontal();

        this.ObjectDefinitionCutstomEditorProfile.SearchField.GUITick();
        EditorGUILayout.Separator();

        EditorGUILayout.BeginVertical();

        var rangeDefinitionTypes = RangeDefinitionTarget.RangeDefinitionModulesActivation.Keys
                .Select(k => k).Where(k => this.ObjectDefinitionCutstomEditorProfile.SearchField.IsMatchingWith(k.Name))
                .Select(k => k).Where(k =>
                {
                    if (this.ObjectDefinitionCutstomEditorProfile.ShowInactives)
                    {
                        return true;
                    }

                    RangeDefinitionTarget.RangeDefinitionModulesActivation.TryGetValue(k, out bool show);
                    return show;
                })
                .ToList();

        foreach (var rangeDefinitionType in rangeDefinitionTypes)
        {
            var moduleMetadata = rangeDefinitionType.GetCustomAttributes(typeof(ModuleMetadata), true).FirstOrDefault() as ModuleMetadata;
            string headerName = "Default";
            if (moduleMetadata != null)
            {
                headerName = moduleMetadata.Header;
            }

            if (!this.RangeDefinitionModulesActivationGrouped.ContainsKey(headerName)) { this.RangeDefinitionModulesActivationGrouped[headerName] = new List<Type>(); }
            this.RangeDefinitionModulesActivationGrouped[headerName].Add(rangeDefinitionType);
        }

        foreach (var rangeDefinitionModuleGrouped in this.RangeDefinitionModulesActivationGrouped)
        {
            EditorGUILayout.LabelField(rangeDefinitionModuleGrouped.Key, EditorStyles.boldLabel);
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            rect.height = 1;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
            EditorGUILayout.Separator();

            foreach (var rangeDefinitionModule in rangeDefinitionModuleGrouped.Value.OrderBy(k => k.Name))
            {
                if (!this.ObjectDefinitionCutstomEditorProfile.fold.ContainsKey(rangeDefinitionModule))
                {
                    this.ObjectDefinitionCutstomEditorProfile.fold[rangeDefinitionModule] = new FoldableArea(true, rangeDefinitionModule.Name, RangeDefinitionTarget.RangeDefinitionModulesActivation[rangeDefinitionModule]);
                }
                this.ObjectDefinitionCutstomEditorProfile.fold[rangeDefinitionModule].OnGUI(() =>
                {
                    if (RangeDefinitionTarget.RangeDefinitionModulesActivation[rangeDefinitionModule])
                    {
                        if (!RangeDefinitionTarget.RangeDefinitionModules.ContainsKey(rangeDefinitionModule) || RangeDefinitionTarget.RangeDefinitionModules[rangeDefinitionModule] == null)
                        {
                            RangeDefinitionTarget.RangeDefinitionModules[rangeDefinitionModule] = AssetHelper.CreateAssetAtSameDirectoryLevel(RangeDefinitionTarget, rangeDefinitionModule, rangeDefinitionModule.Name);
                        }
                        else
                        {
                            if (!this.ObjectDefinitionCutstomEditorProfile.cachedEditors.ContainsKey(rangeDefinitionModule)) { this.ObjectDefinitionCutstomEditorProfile.cachedEditors[rangeDefinitionModule] = Editor.CreateEditor(RangeDefinitionTarget.RangeDefinitionModules[rangeDefinitionModule]); }
                            this.ObjectDefinitionCutstomEditorProfile.cachedEditors[rangeDefinitionModule].OnInspectorGUI();
                        }
                    }
                });

                RangeDefinitionTarget.RangeDefinitionModulesActivation[rangeDefinitionModule] = this.ObjectDefinitionCutstomEditorProfile.fold[rangeDefinitionModule].IsEnabled;

            }
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

public class ObjectDefinitionCutstomEditorProfile
{
    public RegexTextFinder SearchField;

    public Dictionary<Type, FoldableArea> fold;

    public Dictionary<Type, Editor> cachedEditors;

    public bool ShowInactives;

    public ObjectDefinitionCutstomEditorProfile()
    {
        if (this.fold == null) { this.fold = new Dictionary<Type, FoldableArea>(); }
        if (this.cachedEditors == null) { this.cachedEditors = new Dictionary<Type, Editor>(); }
        if (this.SearchField == null) { this.SearchField = new RegexTextFinder(); }
        this.ShowInactives = false;
    }
}

public interface IObjectDefinitionCustomEditorEventListener
{
    void BeforeOnInspectorGUI();
}