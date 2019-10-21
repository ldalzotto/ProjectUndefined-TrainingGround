using UnityEngine;
using System.Collections;
using CoreGame;
using System;
using System.Collections.Generic;
using Editor_MainGameCreationWizard;
using LevelManagement;
using UnityEditor;

namespace Editor_LevelChunkCreationWizard
{
    [System.Serializable]
    public class LevelChunkPrefabCreation : CreateablePrefabComponent<LevelChunkType>
    {
        public override Func<AbstractCreationWizardEditorProfile, LevelChunkType> BasePrefabProvider
        {
            get { return (AbstractCreationWizardEditorProfile editorProfile) => { return editorProfile.GetModule<EditorInformations>().EditorInformationsData.CommonGameConfigurations.PuzzleLevelCommonPrefabs.BaseLevelChunkPrefab; }; }
        }

        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var createdBaseChunk = this.Create(InstancePath.LevelChunkBaseLevelPrefabPath, editorInformationsData.LevelZoneChunkID.ToString() + NameConstants.BaseLevelChunkPrefab, editorProfile);
            createdBaseChunk.LevelZoneChunkID = editorInformationsData.LevelZoneChunkID;
            PrefabUtility.SavePrefabAsset(createdBaseChunk.gameObject);
        }
    }
}