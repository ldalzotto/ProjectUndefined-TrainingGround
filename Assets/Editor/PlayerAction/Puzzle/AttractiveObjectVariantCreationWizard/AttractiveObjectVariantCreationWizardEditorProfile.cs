using OdinSerializer;
using RTPuzzle;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor_AttractiveObjectVariantWizardEditor
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AttractiveObjectVariantCreationWizardEditorProfile", menuName = "PlayerAction/Puzzle/AttractiveObject/AttractiveObjectVariantCreationWizardEditorProfile", order = 1)]
    public class AttractiveObjectVariantCreationWizardEditorProfile : SerializedScriptableObject
    {
        public const string GenericAttractiveObjectprefabName = "GenericAttractiveObjectPrefab";

        [SearchableEnum]
        public LevelZonesID LevelZoneID;
        [SearchableEnum]
        public AttractiveObjectId AttractiveObjectId;

        public string ObjectName;
        public AttractiveObjectType AttractiveObjectBasePrefab;

        private const string TmpDirectoryRelativePath = "tmp";
        private DirectoryInfo tmpDirectoryInfo;
        private string projectRelativeTmpFolderPath;

        [HideInInspector]
        public Vector2 WizardScrollPosition;
        [HideInInspector]
        public bool GenericInfoFoldout;

        [HideInInspector]
        public ModelCreation ModelCreation;

        [HideInInspector]
        public Configurationretrieval ConfigurationRetrieval;

        [HideInInspector]
        public bool AttractiveObjectConfigurationFoldout;

        public string ProjectRelativeTmpFolderPath { get => projectRelativeTmpFolderPath; }
        public DirectoryInfo TmpDirectoryInfo { get => tmpDirectoryInfo; }

        [HideInInspector]
        private List<UnityEngine.Object> generatedObjects;
        public List<UnityEngine.Object> GeneratedObjects { get => generatedObjects; }

        public PathConfiguration PathConfiguration;

        #region GUIManager
        [HideInInspector]
        public CreateableScriptableObjectComponent<AttractiveObjectInherentConfigurationData> AttractiveObjectInherentData;
        #endregion

        private void OnEnable()
        {
            var scriptableObjectScriptFileInfo = new FileInfo(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this)));
            try
            {
                this.tmpDirectoryInfo = scriptableObjectScriptFileInfo.Directory.CreateSubdirectory(TmpDirectoryRelativePath);
            }
            catch (IOException e)
            {
                this.tmpDirectoryInfo = scriptableObjectScriptFileInfo.Directory.GetDirectories(TmpDirectoryRelativePath)[0];
            }
            var assetsFolderIndex = this.tmpDirectoryInfo.FullName.IndexOf("Assets\\");
            this.projectRelativeTmpFolderPath = this.tmpDirectoryInfo.FullName.Substring(assetsFolderIndex);
            this.generatedObjects = new List<UnityEngine.Object>();
        }


        public void ResetEditor()
        {
            this.ObjectName = "";
            this.ModelCreation.ResetEditor();
            this.AttractiveObjectInherentData.CreatedObject = null;
            this.generatedObjects = new List<UnityEngine.Object>();
        }
    }

    [System.Serializable]
    public class PathConfiguration
    {
        public string ObjectPrefabFolder;
        public string ObjectConfigurationFolder;
    }

    [System.Serializable]
    public class ModelCreation
    {
        public bool ModelCreationFoldout;

        public GameObject ModelAsset;

        public void OnInspectorGUI()
        {
            this.ModelAsset = EditorGUILayout.ObjectField("Model asset : ", this.ModelAsset, typeof(GameObject), false) as GameObject;
        }

        public void ResetEditor()
        {
            this.ModelAsset = null;
        }
    }

    public class Configurationretrieval
    {
        public bool ConfigurationFilesRetrievalFoldout;
        private AttractiveObjectConfiguration attractiveObjectConfiguration;

        public AttractiveObjectConfiguration AttractiveObjectConfiguration { get => attractiveObjectConfiguration; set => attractiveObjectConfiguration = value; }

        public void OnInspectorGUI()
        {
            this.attractiveObjectConfiguration = EditorGUILayout.ObjectField("Attractive object configuration : ", this.attractiveObjectConfiguration, typeof(AttractiveObjectConfiguration), false) as AttractiveObjectConfiguration;
        }
    }

    [CustomEditor(typeof(AttractiveObjectVariantCreationWizardEditorProfile))]
    public class AttractiveObjectVariantCreationWizardEditorProfileEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();
            AttractiveObjectVariantCreationWizardEditorProfile profileTarget = target as AttractiveObjectVariantCreationWizardEditorProfile;

            if (profileTarget.AttractiveObjectBasePrefab == null)
            {
                profileTarget.AttractiveObjectBasePrefab = AssetFinder.SafeSingleAssetFind<AttractiveObjectType>(AttractiveObjectVariantCreationWizardEditorProfile.GenericAttractiveObjectprefabName);
            }

            if (profileTarget.ModelCreation == null)
            {
                profileTarget.ModelCreation = new ModelCreation();
            }
            if (profileTarget.ConfigurationRetrieval == null)
            {
                Debug.Log("new");
                profileTarget.ConfigurationRetrieval = new Configurationretrieval();
            }
            if (profileTarget.AttractiveObjectInherentData == null)
            {
                profileTarget.AttractiveObjectInherentData = new CreateableScriptableObjectComponent<AttractiveObjectInherentConfigurationData>();
            }
        }
    }
}

