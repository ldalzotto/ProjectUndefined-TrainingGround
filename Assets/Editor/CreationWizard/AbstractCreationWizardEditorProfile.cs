using OdinSerializer;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public abstract class AbstractCreationWizardEditorProfile : SerializedScriptableObject
{
    [HideInInspector]
    public Vector2 WizardScrollPosition { get; set; }

    [HideInInspector]
    public List<Object> GeneratedObjects { get; set; }

    private const string TmpDirectoryRelativePath = "tmp";
    private DirectoryInfo tmpDirectoryInfo;
    private string projectRelativeTmpFolderPath;

    public string ProjectRelativeTmpFolderPath { get => projectRelativeTmpFolderPath; }
    public DirectoryInfo TmpDirectoryInfo { get => tmpDirectoryInfo; }

    public virtual void OnEnable()
    {
        this.GeneratedObjects = new List<Object>();
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
        this.GeneratedObjects = new List<UnityEngine.Object>();
    }

    public virtual void ResetEditor()
    {
        this.GeneratedObjects.Clear();
    }

    public abstract void OnGenerationEnd();
}
