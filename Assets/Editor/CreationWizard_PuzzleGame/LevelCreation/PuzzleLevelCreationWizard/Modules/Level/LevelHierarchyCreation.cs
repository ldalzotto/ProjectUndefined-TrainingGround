using Editor_MainGameCreationWizard;
using GameConfigurationID;

namespace Editor_PuzzleLevelCreationWizard
{
    [System.Serializable]
    public class LevelHierarchyCreation : ALevelHierarchyCreation
    {
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