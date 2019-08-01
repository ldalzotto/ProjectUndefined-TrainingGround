using CoreGame;
using Editor_MainGameCreationWizard;
using UnityEditor;

namespace Editor_LevelChunkCreationWizard
{
    [System.Serializable]
    public class LevelChunkConfigurationCreation : CreateableScriptableObjectComponent<LevelZonesSceneConfigurationData>
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            this.CreateAsset(InstancePath.LevelZoneChunkSceneConfigurationDataPath, editorInformationsData.LevelZoneChunkID.ToString() + NameConstants.LevelChunkSceneConfigurationData, editorProfile);
            this.AddToGameConfiguration(editorInformationsData.LevelZoneChunkID, editorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.ChunkZonesSceneConfiguration, editorProfile);
        }

        public override void AfterGeneration(AbstractCreationWizardEditorProfile editorProfile)
        {
            var levelChunkSceneCreation = editorProfile.GetModule<LevelChunkSceneCreation>();
            var serializedCreatedObject = new SerializedObject(this.CreatedObject);
            serializedCreatedObject.FindProperty(nameof(this.CreatedObject.scene)).objectReferenceValue = levelChunkSceneCreation.CreatedSceneAsset;
            serializedCreatedObject.ApplyModifiedProperties();
        }
    }
}