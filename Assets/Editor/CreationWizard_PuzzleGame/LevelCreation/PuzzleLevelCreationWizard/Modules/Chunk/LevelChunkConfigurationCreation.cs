﻿using UnityEngine;
using System.Collections;
using CoreGame;
using Editor_PuzzleGameCreationWizard;

namespace Editor_PuzzleLevelCreationWizard
{
    [System.Serializable]
    public class LevelChunkConfigurationCreation : CreateableScriptableObjectComponent<LevelZonesSceneConfigurationData>
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var levelHierarchyCreation = editorProfile.GetModule<LevelHierarchyCreation>();

            var createdChunkConfig = this.CreateAsset(editorInformationsData.CommonGameConfigurations.InstancePath.LevelZoneChunkSceneConfigurationDataPath, editorInformationsData.LevelZoneChunkID.ToString() + NameConstants.LevelChunkSceneConfigurationData);
            editorProfile.AddToGeneratedObjects(new Object[] { createdChunkConfig });

            editorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.ChunkZonesSceneConfiguration.SetEntry(editorInformationsData.LevelZoneChunkID, createdChunkConfig);
            editorProfile.GameConfigurationModified(editorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.ChunkZonesSceneConfiguration, editorInformationsData.LevelZoneChunkID, createdChunkConfig);

            levelHierarchyCreation.CreatedObject.LevelHierarchy = new System.Collections.Generic.List<LevelZoneChunkID>();
            levelHierarchyCreation.CreatedObject.LevelHierarchy.Add(editorInformationsData.LevelZoneChunkID);
        }

        public void AfterSceneCreation(EditorInformationsData editorInformationsData, LevelChunkSceneCreation levelChunkSceneCreation)
        {
            this.CreatedObject.scene = levelChunkSceneCreation.CreatedSceneAsset;
        }
    }
}