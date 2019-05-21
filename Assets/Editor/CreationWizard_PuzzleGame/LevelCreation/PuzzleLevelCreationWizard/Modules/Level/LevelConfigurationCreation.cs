using UnityEngine;
using UnityEditor;
using RTPuzzle;
using System;
using Editor_PuzzleGameCreationWizard;

namespace Editor_PuzzleLevelCreationWizard
{
    [System.Serializable]
    public class LevelConfigurationCreation : CreateableScriptableObjectComponent<LevelConfigurationData>
    {
        public LevelConfigurationCreation(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }
        protected override string objectFieldLabel => typeof(LevelConfigurationData).Name;

        public void OnGenerationClicked(EditorInformationsData editorInformationsData, AbstractCreationWizardEditorProfile editorProfile)
        {
            var createdAsset = this.CreateAsset(editorInformationsData.InstancePath.LevelConfigurationDataPath, editorInformationsData.LevelZonesID.ToString() + NameConstants.LevelConfigurationData);
            editorInformationsData.PuzzleGameConfigurations.LevelConfiguration.SetEntry(editorInformationsData.LevelZonesID, createdAsset);
            editorProfile.AddToGeneratedObjects(new UnityEngine.Object[] { createdAsset });
            editorProfile.GameConfigurationModified(editorInformationsData.PuzzleGameConfigurations.LevelConfiguration, editorInformationsData.LevelZonesID, createdAsset);
        }
    }
}
