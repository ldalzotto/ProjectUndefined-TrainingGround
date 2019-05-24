using UnityEngine;
using System.Collections;
using CoreGame;
using Editor_PuzzleGameCreationWizard;
using UnityEditor;
using System;

namespace Editor_PuzzleLevelCreationWizard
{
    [System.Serializable]
    public class LevelChunkConfigurationCreation : CreateableScriptableObjectComponent<LevelZonesSceneConfigurationData>
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var levelHierarchyCreation = editorProfile.GetModule<LevelHierarchyCreation>();

            this.CreateAsset(editorInformationsData.CommonGameConfigurations.InstancePath.LevelZoneChunkSceneConfigurationDataPath, editorInformationsData.LevelZoneChunkID.ToString() + NameConstants.LevelChunkSceneConfigurationData, editorProfile);
            this.AddToGameConfiguration(editorInformationsData.LevelZoneChunkID, editorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.ChunkZonesSceneConfiguration, editorProfile);

            var levelHierarchyCreationSerialized = new SerializedObject(levelHierarchyCreation.CreatedObject);
            var levelHierarchy = new System.Collections.Generic.List<LevelZoneChunkID>() { editorInformationsData.LevelZoneChunkID };
            SerializableObjectHelper.SetArray(levelHierarchy.ConvertAll(e => (Enum)e), levelHierarchyCreationSerialized.FindProperty(nameof(levelHierarchyCreation.CreatedObject.LevelHierarchy)));
            levelHierarchyCreationSerialized.ApplyModifiedProperties();
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