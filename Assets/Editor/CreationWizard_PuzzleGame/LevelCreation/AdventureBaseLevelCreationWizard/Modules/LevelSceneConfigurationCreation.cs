using Editor_MainGameCreationWizard;
using GameConfigurationID;
using UnityEditor;

namespace Editor_AdventureBaseLevelCreationWizard
{
    [System.Serializable]
    public class LevelSceneConfigurationCreation : ALevelSceneConfigurationCreation //CreateableScriptableObjectComponent<LevelZonesSceneConfigurationData>
    {
        protected override LevelZonesID GetLevelZonesID(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            return editorInformationsData.LevelZonesID;
        }

        protected override CommonGameConfigurations GetCommonGameConfigurations(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            return editorInformationsData.CommonGameConfigurations;
        }

        protected override SceneAsset GetCreatedSceneAsset(AbstractCreationWizardEditorProfile editorProfile)
        {
            var sceneCreation = editorProfile.GetModule<LevelSceneCreation>();
            return sceneCreation.CreatedSceneAsset;
        }
    }
}