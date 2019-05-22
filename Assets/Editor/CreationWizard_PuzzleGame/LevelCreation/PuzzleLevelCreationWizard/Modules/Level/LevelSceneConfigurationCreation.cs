using UnityEngine;
using System.Collections;
using CoreGame;
using Editor_PuzzleGameCreationWizard;
using System;

namespace Editor_PuzzleLevelCreationWizard
{
    [System.Serializable]
    public class LevelSceneConfigurationCreation : CreateableScriptableObjectComponent<LevelZonesSceneConfigurationData>
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var levelZoneConfiguration = this.CreateAsset(editorInformationsData.CommonGameConfigurations.InstancePath.LevelZoneSceneConfigurationDataPath, editorInformationsData.LevelZonesID + NameConstants.LevelSceneConfigurationData);
            editorProfile.AddToGeneratedObjects(new UnityEngine.Object[] { levelZoneConfiguration });

            editorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LevelZonesSceneConfiguration.SetEntry(editorInformationsData.LevelZonesID, levelZoneConfiguration);
            editorProfile.GameConfigurationModified(editorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LevelZonesSceneConfiguration, editorInformationsData.LevelZonesID, levelZoneConfiguration);

        }



        public void AfterSceneGeneration(LevelSceneCreation sceneCreation)
        {
            this.CreatedObject.scene = sceneCreation.CreatedSceneAsset;
        }
    }
}