using Editor_MainGameCreationWizard;
using UnityEditor;
using UnityEngine;

public class FileExplorer : EditorWindow
{
    public const string FileExplorerProfileAssetPath = "Assets/Editor/FileExplorer";

    [MenuItem("FileExplorer/FileExplorer")]
    static void Init()
    {
        FileExplorer window = (FileExplorer)EditorWindow.GetWindow(typeof(FileExplorer));
        window.Show();
    }

    public FileExplorerEditorProfile FileExplorerEditorProfile;
    public CommonGameConfigurations CommonGameConfigurations;

    private void OnEnable()
    {
        if (this.FileExplorerEditorProfile == null)
        {
            this.FileExplorerEditorProfile = AssetFinder.SafeSingleAssetFind<FileExplorerEditorProfile>("t:" + typeof(FileExplorerEditorProfile).Name);
            if (this.FileExplorerEditorProfile == null)
            {
                this.FileExplorerEditorProfile = (FileExplorerEditorProfile)ScriptableObject.CreateInstance(typeof(FileExplorerEditorProfile));
                AssetDatabase.CreateAsset(this.FileExplorerEditorProfile, FileExplorerProfileAssetPath + "/FileExplorerEditorProfile.asset");
            }
        }
        if (this.CommonGameConfigurations == null)
        {
            this.CommonGameConfigurations = new CommonGameConfigurations();
            EditorInformationsHelper.InitProperties(ref this.CommonGameConfigurations);
        }

        if (this.FileRegexSearch == null)
        {
            this.FileRegexSearch = new RegexTextFinder();
        }

        this.FileExplorerEditorProfile.FoundedFolders.Clear();
        this.FileExplorerEditorProfile.FoundedFolders.Add("Interactive objects : ", (DefaultAsset)AssetDatabase.LoadAssetAtPath(InstancePath.InteractiveObjectPrefabPath, typeof(DefaultAsset)));
        this.FileExplorerEditorProfile.FoundedFolders.Add("POI : ", (DefaultAsset)AssetDatabase.LoadAssetAtPath(InstancePath.POIPrefabPath, typeof(DefaultAsset)));
        this.FileExplorerEditorProfile.FoundedFolders.Add("Cutscenes : ", (DefaultAsset)AssetDatabase.LoadAssetAtPath(InstancePath.CutsceneGraphPath, typeof(DefaultAsset)));
        this.FileExplorerEditorProfile.FoundedFolders.Add("AI : ", (DefaultAsset)AssetDatabase.LoadAssetAtPath(InstancePath.AIPrefabPaths, typeof(DefaultAsset)));
    }

    private RegexTextFinder FileRegexSearch;

    private void OnGUI()
    {
        if (this.FileExplorerEditorProfile != null)
        {
            this.FileRegexSearch.GUITick();

            this.FileExplorerEditorProfile.ScrollPosition = EditorGUILayout.BeginScrollView(this.FileExplorerEditorProfile.ScrollPosition);
            EditorGUILayout.Separator();

            foreach (var foundedFolder in this.FileExplorerEditorProfile.FoundedFolders)
            {
                if (this.FileRegexSearch.IsMatchingWith(foundedFolder.Key))
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(foundedFolder.Key, foundedFolder.Value, typeof(DefaultAsset), false);
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.ObjectField(this.FileExplorerEditorProfile, typeof(FileExplorerEditorProfile), false);
    }
}
