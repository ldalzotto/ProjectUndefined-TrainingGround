using RTPuzzle;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor_PuzzleGameCreationWizard
{
    public class AttractiveObjectVariantCreationWizardV2 : AbstractCreationWizardEditor<AttractiveObjectVariantCreationWizardEditorProfile>
    {
        protected override void OnWizardGUI()
        {
            var genericInformation = this.editorProfile.GetModule<GenericInformation>();
            var modelCreation = this.editorProfile.GetModule<ModelCreation>();
            var configurationRetrieval = this.editorProfile.GetModule<Configurationretrieval>();
            var attractiveObjectInherentData = this.editorProfile.GetModule<AttractiveObjectInherentDataModule>();

            genericInformation.OnInspectorGUI(ref this.editorProfile.Modules);
            modelCreation.OnInspectorGUI(ref this.editorProfile.Modules);
            configurationRetrieval.OnInspectorGUI(ref this.editorProfile.Modules);

            attractiveObjectInherentData.OnInspectorGUI(ref this.editorProfile.Modules);
        }

        protected override void OnGenerationClicked(Scene tmpScene)
        {
            var genericInformation = this.editorProfile.GetModule<GenericInformation>();
            var modelCreation = this.editorProfile.GetModule<ModelCreation>();
            var configurationRetrieval = this.editorProfile.GetModule<Configurationretrieval>();
            var attractiveObjectInherentData = this.editorProfile.GetModule<AttractiveObjectInherentDataModule>();

            #region Model Prefab
            var modelAssetGeneration = new GeneratedPrefabAssetManager<GameObject>(modelCreation.ModelAsset, this.editorProfile.ProjectRelativeTmpFolderPath,
                   NamingConventionHelper.BuildName(genericInformation.ObjectName, genericInformation.LevelZoneID, PrefixType.ATTRACTIVE_OBJECT, SufixType.MODEL));
            this.editorProfile.AddToGeneratedObjects(new Object[] { modelAssetGeneration.SavedAsset });
            #endregion

            #region Attractive Object Prefab
            var attractiveObjectAssetGeneration = new GeneratedPrefabAssetManager<AttractiveObjectType>(genericInformation.AttractiveObjectBasePrefab,
                this.editorProfile.ProjectRelativeTmpFolderPath, NamingConventionHelper.BuildName(genericInformation.ObjectName, genericInformation.LevelZoneID, PrefixType.ATTRACTIVE_OBJECT, SufixType.ATTRACTIVE_OBJECT));
           // PrefabUtility.InstantiatePrefab(modelAssetGeneration.SavedAsset, attractiveObjectAssetGeneration.transform);
            this.editorProfile.AddToGeneratedObjects(new Object[] { attractiveObjectAssetGeneration.SavedAsset });
            #endregion

            #region Attractive object game configuration.
            attractiveObjectInherentData.CreatedObject.AttractiveObjectPrefab = attractiveObjectAssetGeneration.SavedAsset.GetComponent<AttractiveObjectType>();
            attractiveObjectInherentData.CreatedObject.AttractiveObjectModelPrefab = modelAssetGeneration.SavedAsset;
            var attractiveObjectInherentConfigurationDataGenerated = attractiveObjectInherentData.CreateAsset(this.editorProfile.ProjectRelativeTmpFolderPath, NamingConventionHelper.BuildName(genericInformation.ObjectName, genericInformation.LevelZoneID, PrefixType.ATTRACTIVE_OBJECT, SufixType.ATTRACTIVE_OBJECT_INHERENT_DATA));
            this.editorProfile.AddToGeneratedObjects(new Object[] { attractiveObjectInherentConfigurationDataGenerated });
            #endregion

            #region Game configuration update
            configurationRetrieval.AttractiveObjectConfiguration.SetEntry(genericInformation.AttractiveObjectId, attractiveObjectInherentConfigurationDataGenerated);
            #endregion

            #region Assets move from tmp
            modelAssetGeneration.MoveGeneratedAsset(genericInformation.PathConfiguration.ObjectPrefabFolder);
            attractiveObjectAssetGeneration.MoveGeneratedAsset(genericInformation.PathConfiguration.ObjectPrefabFolder);
            attractiveObjectInherentData.MoveGeneratedAsset(genericInformation.PathConfiguration.ObjectConfigurationFolder);
            #endregion
        }


    }

}

