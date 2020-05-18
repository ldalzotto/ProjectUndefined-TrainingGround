﻿using OdinSerializer;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public abstract class AbstractCreationWizardEditorProfile : SerializedScriptableObject
{
    [HideInInspector]
    public Vector2 WizardScrollPosition { get; set; }

    [SerializeField]
    private List<ICreationWizardFeedLine> creationWizardFeedLines = new List<ICreationWizardFeedLine>();

    [HideInInspector]
    public Dictionary<string, CreationModuleComponent> Modules = new Dictionary<string, CreationModuleComponent>();

    private const string TmpDirectoryRelativePath = "tmp";
    private DirectoryInfo tmpDirectoryInfo;
    private string projectRelativeTmpFolderPath;

    public string ProjectRelativeTmpFolderPath { get => projectRelativeTmpFolderPath; }
    public DirectoryInfo TmpDirectoryInfo { get => tmpDirectoryInfo; }
    public List<ICreationWizardFeedLine> CreationWizardFeedLines { get => creationWizardFeedLines; }

    #region Logical Conditions
    public bool ContainsWarn()
    {
        foreach (var mod in this.Modules.Values)
        {
            if (mod.HasWarning())
            {
                return true;
            }
        }
        return false;
    }

    public bool ContainsError()
    {
        foreach (var mod in this.Modules.Values)
        {
            if (mod.HasError())
            {
                return true;
            }
        }
        return false;
    }

    #endregion

    public virtual void OnEnable()
    {
        var scriptableObjectScriptFileInfo = new FileInfo(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this)));
        try
        {
            this.tmpDirectoryInfo = scriptableObjectScriptFileInfo.Directory.CreateSubdirectory(TmpDirectoryRelativePath);
        }
        catch (IOException)
        {
            this.tmpDirectoryInfo = scriptableObjectScriptFileInfo.Directory.GetDirectories(TmpDirectoryRelativePath)[0];
        }
        var assetsFolderIndex = this.tmpDirectoryInfo.FullName.IndexOf("Assets\\");
        this.projectRelativeTmpFolderPath = this.tmpDirectoryInfo.FullName.Substring(assetsFolderIndex);
    }

    public virtual void ResetEditor()
    {
        this.creationWizardFeedLines.Clear();
    }

    public abstract void OnGenerationEnd();

    public void AddToGeneratedObjects(UnityEngine.Object[] objs)
    {
        foreach (var obj in objs)
        {
            this.creationWizardFeedLines.Add(new CreatedObjectFeedLine(AssetDatabase.GetAssetPath(obj)));
        }
    }

    public void GameConfigurationModified(UnityEngine.Object configuration, Enum key, UnityEngine.Object value)
    {
        this.creationWizardFeedLines.Add(new ConfigurationModifiedFeedLine(
            AssetDatabase.GetAssetPath(configuration), key, AssetDatabase.GetAssetPath(value)            ));
    }

    protected void InitModule<T>(bool moduleFoldout, bool moduleEnabled, bool moduleDistableAble) where T : CreationModuleComponent
    {
        if (!this.Modules.ContainsKey(typeof(T).Name))
        {
            this.Modules[typeof(T).Name] = CreationModuleComponent.Create<T>(this.ProjectRelativeTmpFolderPath + "\\" + typeof(T).Name + ".asset", moduleFoldout, moduleEnabled, moduleDistableAble, this.ProjectRelativeTmpFolderPath);
        }
    }

    public T GetModule<T>() where T : CreationModuleComponent
    {
        return (T)this.Modules[typeof(T).Name];
    }

}

public interface ICreationWizardFeedLine
{
    void GUITick();
}

[System.Serializable]
public class CreatedObjectFeedLine : ICreationWizardFeedLine
{
    [SerializeField]
    private string filePath;

    public CreatedObjectFeedLine(string filePath)
    {
        this.filePath = filePath;
    }

    private UnityEngine.Object createdAsset;

    public string FilePath { get => filePath; }

    public void GUITick()
    {
        if (this.createdAsset == null)
        {
            this.createdAsset = AssetDatabase.LoadAssetAtPath(this.filePath, typeof(UnityEngine.Object));
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("NEW : ", GUILayout.Width(35f));
        EditorGUILayout.ObjectField(this.createdAsset, typeof(UnityEngine.Object), false);
        EditorGUILayout.EndHorizontal();
    }
}

[System.Serializable]
public class ConfigurationModifiedFeedLine : ICreationWizardFeedLine
{
    [SerializeField]
    private string configurationPath;
    [SerializeField]
    private Enum keySet;
    [SerializeField]
    private string objectSetPath;

    public ConfigurationModifiedFeedLine(string configurationPath, Enum keySet, string objectSetPath)
    {
        this.configurationPath = configurationPath;
        this.keySet = keySet;
        this.objectSetPath = objectSetPath;
    }

    public void GUITick()
    {
        this.Init();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Added configuration key : ", GUILayout.Width(150));
        EditorGUILayout.ObjectField(this.configurationObject, typeof(UnityEngine.Object), false);
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel += 1;
        EditorGUILayout.LabelField(this.keySet.ToString());
        EditorGUILayout.ObjectField(this.objectSet, typeof(UnityEngine.Object), false);
        EditorGUI.indentLevel -= 1;
        EditorGUILayout.EndVertical();
    }

    private UnityEngine.Object configurationObject;
    private UnityEngine.Object objectSet;

    private void Init()
    {
        if(this.configurationObject == null)
        {
            this.configurationObject = AssetDatabase.LoadAssetAtPath(this.configurationPath, typeof(UnityEngine.Object));
        }
        if (this.objectSet == null)
        {
            this.objectSet = AssetDatabase.LoadAssetAtPath(this.objectSetPath, typeof(UnityEngine.Object));
        }
    }
}