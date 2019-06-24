using Editor_MainGameCreationWizard;
using GameConfigurationID;

namespace Editor_AdventureBaseLevelCreationWizard
{
    [System.Serializable]
    public class LevelHierarchyCreation : ALevelHierarchyCreation
    {

        /*
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var generatedHierarchy = this.CreateAsset(editorInformationsData.CommonGameConfigurations.InstancePath.PuzzleLevelHierarchyDataPath, editorInformationsData.LevelZonesID + NameConstants.LevelHierarchyConfigurationData, editorProfile);

            editorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LevelHierarchyConfiguration.SetEntry(editorInformationsData.LevelZonesID, generatedHierarchy);
            editorProfile.GameConfigurationModified(editorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LevelHierarchyConfiguration, editorInformationsData.LevelZonesID, generatedHierarchy);
        }
        */

        protected override CommonGameConfigurations GetCommonGameConfigurations(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            return editorInformationsData.CommonGameConfigurations;
        }

        protected override LevelZonesID GetLevelZonesID(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            return editorInformationsData.LevelZonesID;
        }
    }
}