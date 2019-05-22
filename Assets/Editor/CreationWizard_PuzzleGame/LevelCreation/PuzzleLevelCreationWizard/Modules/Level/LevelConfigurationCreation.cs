using UnityEngine;
using UnityEditor;
using RTPuzzle;
using System;
using Editor_PuzzleGameCreationWizard;
using System.Collections.Generic;

namespace Editor_PuzzleLevelCreationWizard
{
    [System.Serializable]
    public class LevelConfigurationCreation : CreateableScriptableObjectComponent<LevelConfigurationData>
    {
        protected override string objectFieldLabel => typeof(LevelConfigurationData).Name;

        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var createdAsset = this.CreateAsset(editorInformationsData.CommonGameConfigurations.InstancePath.LevelConfigurationDataPath, editorInformationsData.LevelZonesID.ToString() + NameConstants.LevelConfigurationData);
            editorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LevelConfiguration.SetEntry(editorInformationsData.LevelZonesID, createdAsset);
            editorProfile.AddToGeneratedObjects(new UnityEngine.Object[] { createdAsset });
            editorProfile.GameConfigurationModified(editorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LevelConfiguration, editorInformationsData.LevelZonesID, createdAsset);
        }

    }
}
