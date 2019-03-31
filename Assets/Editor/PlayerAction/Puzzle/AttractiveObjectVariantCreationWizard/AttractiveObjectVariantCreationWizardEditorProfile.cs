using RTPuzzle;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor_AttractiveObjectVariantWizardEditor
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AttractiveObjectVariantCreationWizardEditorProfile", menuName = "PlayerAction/Puzzle/AttractiveObject/AttractiveObjectVariantCreationWizardEditorProfile", order = 1)]
    public class AttractiveObjectVariantCreationWizardEditorProfile : AbstractCreationWizardEditorProfile
    {
        public const string GenericAttractiveObjectprefabName = "GenericAttractiveObjectPrefab";

        private const string TmpDirectoryRelativePath = "tmp";
        private DirectoryInfo tmpDirectoryInfo;
        private string projectRelativeTmpFolderPath;

        [HideInInspector]
        public GenericInformation GenericInformation;

        [HideInInspector]
        public ModelCreation ModelCreation;

        [HideInInspector]
        public Configurationretrieval ConfigurationRetrieval;

        [HideInInspector]
        public bool AttractiveObjectConfigurationFoldout;

        public string ProjectRelativeTmpFolderPath { get => projectRelativeTmpFolderPath; }
        public DirectoryInfo TmpDirectoryInfo { get => tmpDirectoryInfo; }

        #region GUIManager
        [HideInInspector]
        public AttractiveObjectInherentDataModule AttractiveObjectInherentData;
        #endregion

        public override void OnEnable()
        {
            base.OnEnable();
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

        public override void ResetEditor()
        {
            base.ResetEditor();
            this.ModelCreation.ResetEditor();
            this.AttractiveObjectInherentData.ResetEditor();
        }

        public override void OnGenerationEnd()
        {
            this.AttractiveObjectInherentData.OnGenerationEnd();
        }
    }

    [System.Serializable]
    public class PathConfiguration
    {
        public string ObjectPrefabFolder;
        public string ObjectConfigurationFolder;
    }

    [System.Serializable]
    public class ModelCreation : CreationModuleComponent
    {

        public GameObject ModelAsset;

        public ModelCreation(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        protected override string foldoutLabel => "Model creation : ";

        public override void ResetEditor()
        {
            this.ModelAsset = null;
        }

        protected override void OnInspectorGUIImpl()
        {
            this.ModelAsset = EditorGUILayout.ObjectField("Model asset : ", this.ModelAsset, typeof(GameObject), false) as GameObject;
        }
    }

    [System.Serializable]
    public class Configurationretrieval : CreationModuleComponent
    {
        private AttractiveObjectConfiguration attractiveObjectConfiguration;

        public Configurationretrieval(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        public AttractiveObjectConfiguration AttractiveObjectConfiguration { get => attractiveObjectConfiguration; set => attractiveObjectConfiguration = value; }

        protected override string foldoutLabel => "Game configuration : ";

        public override void ResetEditor()
        {

        }

        protected override void OnInspectorGUIImpl()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Attractive object configuration : ", this.attractiveObjectConfiguration, typeof(AttractiveObjectConfiguration), false);
            EditorGUI.EndDisabledGroup();

            if (this.attractiveObjectConfiguration == null)
            {
                this.attractiveObjectConfiguration = AssetFinder.SafeSingleAssetFind<AttractiveObjectConfiguration>("t:" + typeof(AttractiveObjectConfiguration).ToString());
            }
        }
    }

    [System.Serializable]
    public class AttractiveObjectInherentDataModule : CreateableScriptableObjectComponent<AttractiveObjectInherentConfigurationData>
    {
        public AttractiveObjectInherentDataModule(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        protected override string objectFieldLabel => "Generated attractive object configuration : ";

        protected override string foldoutLabel => "Object game configuration : ";
    }

    [CustomEditor(typeof(AttractiveObjectVariantCreationWizardEditorProfile))]
    public class AttractiveObjectVariantCreationWizardEditorProfileEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();
            AttractiveObjectVariantCreationWizardEditorProfile profileTarget = target as AttractiveObjectVariantCreationWizardEditorProfile;

            if (profileTarget.GenericInformation == null)
            {
                profileTarget.GenericInformation = new GenericInformation(false, true, false);
            }

            if (profileTarget.GenericInformation.AttractiveObjectBasePrefab == null)
            {
                profileTarget.GenericInformation.AttractiveObjectBasePrefab = AssetFinder.SafeSingleAssetFind<AttractiveObjectType>(AttractiveObjectVariantCreationWizardEditorProfile.GenericAttractiveObjectprefabName);
            }

            if (profileTarget.ModelCreation == null)
            {
                profileTarget.ModelCreation = new ModelCreation(false, true, false);
            }
            if (profileTarget.ConfigurationRetrieval == null)
            {
                Debug.Log("new");
                profileTarget.ConfigurationRetrieval = new Configurationretrieval(false, true, false);
            }
            if (profileTarget.AttractiveObjectInherentData == null)
            {
                profileTarget.AttractiveObjectInherentData = new AttractiveObjectInherentDataModule(false, true, false);
            }
        }
    }
}

