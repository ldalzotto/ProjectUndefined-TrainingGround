using RTPuzzle;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor_AttractiveObjectVariantWizardEditor
{
    public class AttractiveObjectVariantCreationWizardV2 : AbstractCreationWizardEditor<AttractiveObjectVariantCreationWizardEditorProfile>
    {
        protected override void OnWizardGUI()
        {
            this.editorProfile.GenericInformation.OnInspectorGUI();
            this.editorProfile.ModelCreation.OnInspectorGUI();
            this.editorProfile.ConfigurationRetrieval.OnInspectorGUI();

            this.editorProfile.AttractiveObjectInherentData.OnInspectorGUI(); /*.OnGui((AttractiveObjectInherentConfigurationData attractiveObjectInherentConfigurationData) => attractiveObjectInherentConfigurationData.CreationWizardGUI());*/
        }

        protected override void OnGenerationClicked(Scene tmpScene)
        {
            #region Model Prefab
            var modelAssetGeneration = new GeneratedPrefabAssetManager<GameObject>(this.editorProfile.ModelCreation.ModelAsset, PrefabAssetGenerationWorkflow.MODEL, tmpScene, this.editorProfile.ProjectRelativeTmpFolderPath,
                   NamingConventionHelper.BuildName(editorProfile.GenericInformation.ObjectName, editorProfile.GenericInformation.LevelZoneID, PrefixType.ATTRACTIVE_OBJECT, SufixType.MODEL), null);
            this.editorProfile.GeneratedObjects.Add(modelAssetGeneration.SavedAsset);
            #endregion

            #region Attractive Object Prefab
            var attractiveObjectAssetGeneration = new GeneratedPrefabAssetManager<AttractiveObjectType>(this.editorProfile.GenericInformation.AttractiveObjectBasePrefab, PrefabAssetGenerationWorkflow.PREFAB, tmpScene,
                this.editorProfile.ProjectRelativeTmpFolderPath, NamingConventionHelper.BuildName(editorProfile.GenericInformation.ObjectName, editorProfile.GenericInformation.LevelZoneID, PrefixType.ATTRACTIVE_OBJECT, SufixType.ATTRACTIVE_OBJECT),
                (AttractiveObjectType generatedAsset) =>
                {
                    PrefabUtility.InstantiatePrefab(modelAssetGeneration.SavedAsset, generatedAsset.transform);
                });
            this.editorProfile.GeneratedObjects.Add(attractiveObjectAssetGeneration.SavedAsset);
            #endregion

            #region Attractive object game configuration.
            this.editorProfile.AttractiveObjectInherentData.CreatedObject.AttractiveObjectPrefab = attractiveObjectAssetGeneration.SavedAsset.GetComponent<AttractiveObjectType>();
            this.editorProfile.AttractiveObjectInherentData.CreatedObject.AttractiveObjectModelPrefab = modelAssetGeneration.SavedAsset;
            var attractiveObjectInherentConfigurationDataGenerated = this.editorProfile.AttractiveObjectInherentData.CreateAsset(this.editorProfile.ProjectRelativeTmpFolderPath, NamingConventionHelper.BuildName(editorProfile.GenericInformation.ObjectName, editorProfile.GenericInformation.LevelZoneID, PrefixType.ATTRACTIVE_OBJECT, SufixType.ATTRACTIVE_OBJECT_INHERENT_DATA));
            this.editorProfile.GeneratedObjects.Add(attractiveObjectInherentConfigurationDataGenerated);
            #endregion

            #region Game configuration update
            this.editorProfile.ConfigurationRetrieval.AttractiveObjectConfiguration.SetEntry(this.editorProfile.GenericInformation.AttractiveObjectId, attractiveObjectInherentConfigurationDataGenerated);
            #endregion

            #region Assets move from tmp
            modelAssetGeneration.MoveGeneratedAsset(this.editorProfile.GenericInformation.PathConfiguration.ObjectPrefabFolder);
            attractiveObjectAssetGeneration.MoveGeneratedAsset(this.editorProfile.GenericInformation.PathConfiguration.ObjectPrefabFolder);
            this.editorProfile.AttractiveObjectInherentData.MoveGeneratedAsset(this.editorProfile.GenericInformation.PathConfiguration.ObjectConfigurationFolder);
            #endregion
        }


    }

}

