using ConfigurationEditor;
using CoreGame;
using GameConfigurationID;
using OdinSerializer;
using RTPuzzle;
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

    public abstract List<CreationWizardOrderConfiguration> ModulesConfiguration { get; }

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

    internal void ColapseAll()
    {
        foreach (var mod in this.Modules.Values)
        {
            mod.ModuleFoldout = false;
        }
    }

    internal void CreateAll()
    {
        foreach (var mod in this.Modules.Values)
        {
            if (mod is ICreateable createable)
            {
                createable.InstanciateInEditor(this);
            }
        }
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

        foreach (var moduleConfiguration in this.ModulesConfiguration)
        {
            this.InitModule(moduleConfiguration.ModuleType);
        }
    }

    public virtual void ResetEditor()
    {
        this.creationWizardFeedLines.Clear();
    }

    public void OnGenerationEnd()
    {
        foreach (var module in this.Modules.Values)
        {
            module.OnGenerationEnd();
        }
    }

    public void AddToGeneratedObjects(UnityEngine.Object[] objs)
    {
        foreach (var obj in objs)
        {
            this.creationWizardFeedLines.Add(new CreatedObjectFeedLine(AssetDatabase.GetAssetPath(obj)));
        }
    }
    public void AddToGeneratedObjects(UnityEngine.Object obj)
    {
        this.creationWizardFeedLines.Add(new CreatedObjectFeedLine(AssetDatabase.GetAssetPath(obj)));
    }

    public void GameConfigurationModified(UnityEngine.Object configuration, Enum key, UnityEngine.Object value)
    {
        this.creationWizardFeedLines.Add(new ConfigurationModifiedFeedLine(
            AssetDatabase.GetAssetPath(configuration), key, AssetDatabase.GetAssetPath(value)));
    }

    public void AddedPlayerAction(PlayerActionId playerActionId, LevelConfiguration levelConfiguration, LevelZonesID levelZonesID)
    {
        this.creationWizardFeedLines.Add(new PlayerActionAddedFeedLine(levelConfiguration, playerActionId, levelZonesID));
    }

    public void LevelHierarchyAdded(LevelHierarchyConfiguration levelHierarchyConfiguration, LevelZonesID levelZonesID, LevelZoneChunkID addedChunkID) {
        this.creationWizardFeedLines.Add(new LevelHierarchyAddFeedLine(levelHierarchyConfiguration, levelZonesID, addedChunkID));
    }

    private void InitModule(Type objType)
    {
        if (!this.Modules.ContainsKey(objType.Name))
        {
            this.Modules[objType.Name] = CreationModuleComponent.Create(objType, this.ProjectRelativeTmpFolderPath + "\\" + objType.Name + ".asset", this.ProjectRelativeTmpFolderPath);
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
        EditorGUILayout.ObjectField((UnityEngine.Object)this.configurationObject, typeof(UnityEngine.Object), false);
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel += 1;
        EditorGUILayout.LabelField(this.keySet.ToString());
        EditorGUILayout.ObjectField(this.objectSet, typeof(UnityEngine.Object), false);
        EditorGUI.indentLevel -= 1;
        EditorGUILayout.EndVertical();
    }

    private IConfigurationSerialization configurationObject;
    private UnityEngine.Object objectSet;

    public string ConfigurationPath { get => configurationPath; }

    private void Init()
    {
        if (this.configurationObject == null)
        {
            this.configurationObject = (IConfigurationSerialization)AssetDatabase.LoadAssetAtPath(this.configurationPath, typeof(UnityEngine.Object));
        }
        if (this.objectSet == null)
        {
            this.objectSet = AssetDatabase.LoadAssetAtPath(this.objectSetPath, typeof(UnityEngine.Object));
        }
    }

    public void RemoveEntry()
    {
        this.configurationObject.ClearEntry(this.keySet);
    }

}

[System.Serializable]
public class PlayerActionAddedFeedLine : ICreationWizardFeedLine
{
    [SerializeField]
    private LevelConfiguration levelConfiguration;
    [SerializeField]
    private PlayerActionId playerActionId;
    [SerializeField]
    private LevelZonesID levelZonesID;

    public PlayerActionAddedFeedLine(LevelConfiguration levelConfiguration, PlayerActionId playerActionId, LevelZonesID levelZonesID)
    {
        this.levelConfiguration = levelConfiguration;
        this.playerActionId = playerActionId;
        this.levelZonesID = levelZonesID;
    }

    public LevelConfiguration LevelConfiguration { get => levelConfiguration; }
    public PlayerActionId PlayerActionId { get => playerActionId; }
    public LevelZonesID LevelZonesID { get => levelZonesID; }

    public void GUITick()
    {
        EditorGUILayout.LabelField("Added player action ID : " + this.playerActionId.ToString());
        EditorGUI.indentLevel += 1;
        EditorGUILayout.ObjectField(this.levelConfiguration, typeof(LevelConfiguration), false);
        EditorGUI.indentLevel -= 1;
    }
}

[System.Serializable]
public class LevelHierarchyAddFeedLine : ICreationWizardFeedLine
{
    [SerializeField]
    private LevelHierarchyConfiguration levelHierarchyConfiguration;
    [SerializeField]
    private LevelZonesID levelZonesID;
    [SerializeField]
    private LevelZoneChunkID addedChunkID;

    public LevelHierarchyAddFeedLine(LevelHierarchyConfiguration levelHierarchyConfiguration, LevelZonesID levelZonesID, LevelZoneChunkID addedChunkID)
    {
        this.levelHierarchyConfiguration = levelHierarchyConfiguration;
        this.levelZonesID = levelZonesID;
        this.addedChunkID = addedChunkID;
    }

    public LevelHierarchyConfiguration LevelHierarchyConfiguration { get => levelHierarchyConfiguration; }
    public LevelZonesID LevelZonesID { get => levelZonesID; }
    public LevelZoneChunkID AddedChunkID { get => addedChunkID; }

    public void GUITick()
    {
        EditorGUILayout.LabelField("Added level chunk : " + this.addedChunkID.ToString());
        EditorGUILayout.LabelField("To adventure level : " + this.levelZonesID.ToString());
        EditorGUI.indentLevel += 1;
        EditorGUILayout.ObjectField(this.levelHierarchyConfiguration, typeof(LevelHierarchyConfiguration), false);
        EditorGUI.indentLevel -= 1;
    }
}

[System.Serializable]
public class CreationWizardOrderConfiguration
{
    private Type moduleType;
    private int generationOrder;
    private int afterGenerationOrder;

    public CreationWizardOrderConfiguration(Type moduleType, int generationOrder, int afterGenerationOrder = -1)
    {
        this.moduleType = moduleType;
        this.generationOrder = generationOrder;
        this.afterGenerationOrder = afterGenerationOrder;
    }

    public Type ModuleType { get => moduleType; }
    public int GenerationOrder { get => generationOrder; }
    public int AfterGenerationOrder { get => afterGenerationOrder; }
}