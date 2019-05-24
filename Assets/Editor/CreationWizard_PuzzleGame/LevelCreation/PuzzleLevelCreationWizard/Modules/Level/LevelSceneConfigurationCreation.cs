using UnityEngine;
using System.Collections;
using CoreGame;
using Editor_PuzzleGameCreationWizard;
using System;
using UnityEditor;

namespace Editor_PuzzleLevelCreationWizard
{
    [System.Serializable]
    public class LevelSceneConfigurationCreation : CreateableScriptableObjectComponent<LevelZonesSceneConfigurationData>
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var levelZoneConfiguration = this.CreateAsset(editorInformationsData.CommonGameConfigurations.InstancePath.LevelZoneSceneConfigurationDataPath, editorInformationsData.LevelZonesID + NameConstants.LevelSceneConfigurationData, editorProfile);

            editorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LevelZonesSceneConfiguration.SetEntry(editorInformationsData.LevelZonesID, levelZoneConfiguration);
            editorProfile.GameConfigurationModified(editorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LevelZonesSceneConfiguration, editorInformationsData.LevelZonesID, levelZoneConfiguration);

        }

        public override void AfterGeneration(AbstractCreationWizardEditorProfile editorProfile)
        {
            var sceneCreation = editorProfile.GetModule<LevelSceneCreation>();
            SerializableObjectHelper.Modify(this.CreatedObject, (so) => { so.FindProperty(nameof(this.CreatedObject.scene)).objectReferenceValue = sceneCreation.CreatedSceneAsset; });
        }

    }
}