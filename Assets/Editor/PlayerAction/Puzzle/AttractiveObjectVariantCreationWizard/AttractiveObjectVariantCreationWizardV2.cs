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
            var genericInformation = this.editorProfile.GetModule<GenericInformation>();
            var modelCreation = this.editorProfile.GetModule<ModelCreation>();
            var aiFeedbackMarkCreation = this.editorProfile.GetModule<AIFeedbackMarkCreation>();
            var configurationRetrieval = this.editorProfile.GetModule<Configurationretrieval>();
            var attractiveObjectInherentData = this.editorProfile.GetModule<AttractiveObjectInherentDataModule>();

            genericInformation.OnInspectorGUI(ref this.editorProfile.Modules);
            modelCreation.OnInspectorGUI(ref this.editorProfile.Modules);
            aiFeedbackMarkCreation.OnInspectorGUI(ref this.editorProfile.Modules);
            configurationRetrieval.OnInspectorGUI(ref this.editorProfile.Modules);

            attractiveObjectInherentData.OnInspectorGUI(ref this.editorProfile.Modules);
        }

        protected override void OnGenerationClicked(Scene tmpScene)
        {
            var genericInformation = this.editorProfile.GetModule<GenericInformation>();
            var modelCreation = this.editorProfile.GetModule<ModelCreation>();
            var aiFeedbackMarkCreation = this.editorProfile.GetModule<AIFeedbackMarkCreation>();
            var configurationRetrieval = this.editorProfile.GetModule<Configurationretrieval>();
            var attractiveObjectInherentData = this.editorProfile.GetModule<AttractiveObjectInherentDataModule>();

            #region Model Prefab
            var modelAssetGeneration = new GeneratedPrefabAssetManager<GameObject>(modelCreation.ModelAsset, tmpScene, this.editorProfile.ProjectRelativeTmpFolderPath,
                   NamingConventionHelper.BuildName(genericInformation.ObjectName, genericInformation.LevelZoneID, PrefixType.ATTRACTIVE_OBJECT, SufixType.MODEL), null);
            this.editorProfile.GeneratedObjects.Add(modelAssetGeneration.SavedAsset);
            #endregion

            #region AI Feedback prefab
            AIFeedbackMarkType genereatedFeedbackMarkType = aiFeedbackMarkCreation.Create(genericInformation.AIFeedbackMarkBasePrefab, tmpScene, this.editorProfile.ProjectRelativeTmpFolderPath,
                   NamingConventionHelper.BuildName(genericInformation.ObjectName, PrefixType.AI_FEEDBACK_MARK, SufixType.NONE),
                   (AIFeedbackMarkType aiFeedbackMarkType, GameObject newPrefab) =>
                   {
                       PrefabUtility.InstantiatePrefab(newPrefab, aiFeedbackMarkType.transform);
                       var meshFilter = aiFeedbackMarkType.GetComponentInChildren<MeshFilter>();
                       Renderer renderer = null;
                       if (meshFilter == null)
                       {
                           renderer = aiFeedbackMarkType.GetComponentInChildren<Renderer>();
                       }
                       else
                       {
                           renderer = meshFilter.gameObject.GetComponent<Renderer>();
                       }

                       renderer.material = aiFeedbackMarkCreation.AIFeedbackVertexLitMaterial;
                       renderer.gameObject.AddComponent<VertexUnlitInstanciatedPropertySetter>();
                   });
            this.editorProfile.GeneratedObjects.Add(genereatedFeedbackMarkType);
            #endregion

            #region Attractive Object Prefab
            var attractiveObjectAssetGeneration = new GeneratedPrefabAssetManager<AttractiveObjectType>(genericInformation.AttractiveObjectBasePrefab, tmpScene,
                this.editorProfile.ProjectRelativeTmpFolderPath, NamingConventionHelper.BuildName(genericInformation.ObjectName, genericInformation.LevelZoneID, PrefixType.ATTRACTIVE_OBJECT, SufixType.ATTRACTIVE_OBJECT),
                (AttractiveObjectType generatedAsset) =>
                {
                    PrefabUtility.InstantiatePrefab(modelAssetGeneration.SavedAsset, generatedAsset.transform);
                });
            this.editorProfile.GeneratedObjects.Add(attractiveObjectAssetGeneration.SavedAsset);
            #endregion

            #region Attractive object game configuration.
            attractiveObjectInherentData.CreatedObject.AttractiveObjectPrefab = attractiveObjectAssetGeneration.SavedAsset.GetComponent<AttractiveObjectType>();
            attractiveObjectInherentData.CreatedObject.AttractiveObjectModelPrefab = modelAssetGeneration.SavedAsset;
            attractiveObjectInherentData.CreatedObject.AttractiveObjectAIMarkPrefab = genereatedFeedbackMarkType;
            var attractiveObjectInherentConfigurationDataGenerated = attractiveObjectInherentData.CreateAsset(this.editorProfile.ProjectRelativeTmpFolderPath, NamingConventionHelper.BuildName(genericInformation.ObjectName, genericInformation.LevelZoneID, PrefixType.ATTRACTIVE_OBJECT, SufixType.ATTRACTIVE_OBJECT_INHERENT_DATA));
            this.editorProfile.GeneratedObjects.Add(attractiveObjectInherentConfigurationDataGenerated);
            #endregion

            #region Game configuration update
            configurationRetrieval.AttractiveObjectConfiguration.SetEntry(genericInformation.AttractiveObjectId, attractiveObjectInherentConfigurationDataGenerated);
            #endregion

            #region Assets move from tmp
            modelAssetGeneration.MoveGeneratedAsset(genericInformation.PathConfiguration.ObjectPrefabFolder);
            attractiveObjectAssetGeneration.MoveGeneratedAsset(genericInformation.PathConfiguration.ObjectPrefabFolder);
            attractiveObjectInherentData.MoveGeneratedAsset(genericInformation.PathConfiguration.ObjectConfigurationFolder);
            aiFeedbackMarkCreation.MoveGeneratedAsset(genericInformation.PathConfiguration.AIMarkPrefabFolder);
            #endregion
        }


    }

}

