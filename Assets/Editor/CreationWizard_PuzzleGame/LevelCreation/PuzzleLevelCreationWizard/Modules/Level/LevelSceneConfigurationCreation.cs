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
        public LevelSceneConfigurationCreation(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        protected override string objectFieldLabel => typeof(LevelZonesSceneConfigurationData).Name;

        public void OnGenerationClicked(EditorInformationsData editorInformationsData, AbstractCreationWizardEditorProfile editorProfile)
        {
            var levelZoneConfiguration = this.CreateAsset(editorInformationsData.InstancePath.LevelZoneSceneConfigurationDataPath, editorInformationsData.LevelZonesID + NameConstants.LevelSceneConfigurationData);
            editorProfile.AddToGeneratedObjects(new UnityEngine.Object[] { levelZoneConfiguration });

            editorInformationsData.PuzzleGameConfigurations.LevelZonesSceneConfiguration.SetEntry(editorInformationsData.LevelZonesID, levelZoneConfiguration);
            editorProfile.GameConfigurationModified(editorInformationsData.PuzzleGameConfigurations.LevelZonesSceneConfiguration, editorInformationsData.LevelZonesID, levelZoneConfiguration);
        }

        public void AfterSceneGeneration(LevelSceneCreation sceneCreation)
        {
            this.CreatedObject.scene = sceneCreation.CreatedSceneAsset;
        }
    }
}