using UnityEngine;
using System.Collections;
using CoreGame;
using Editor_PuzzleGameCreationWizard;
using UnityEditor;

namespace Editor_PuzzleLevelCreationWizard
{
    [System.Serializable]
    public class LevelHierarchyCreation : CreateableScriptableObjectComponent<LevelHierarchyConfigurationData>
    {

        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var generatedHierarchy = this.CreateAsset(editorInformationsData.CommonGameConfigurations.InstancePath.PuzzleLevelHierarchyDataPath, editorInformationsData.LevelZonesID + NameConstants.LevelHierarchyConfigurationData);
            editorProfile.AddToGeneratedObjects(new Object[] { generatedHierarchy });

            editorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LevelHierarchyConfiguration.SetEntry(editorInformationsData.LevelZonesID, generatedHierarchy);
            editorProfile.GameConfigurationModified(editorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LevelHierarchyConfiguration, editorInformationsData.LevelZonesID, generatedHierarchy);

        }
    }
}