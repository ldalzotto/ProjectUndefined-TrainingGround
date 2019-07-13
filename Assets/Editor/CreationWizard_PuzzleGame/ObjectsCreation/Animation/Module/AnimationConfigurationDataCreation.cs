using CoreGame;
using Editor_MainGameCreationWizard;

namespace Editor_AnimationCreationWizard
{
    public class AnimationConfigurationDataCreation : CreateableScriptableObjectComponent<AnimationConfigurationData>
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            this.CreateAsset(editorInformationsData.CommonGameConfigurations.InstancePath.AnimationConfigurationDataPath,
                 editorInformationsData.AnimationID.ToString() + NameConstants.AnimationConfigurationdata, editorProfile);
            this.AddToGameConfiguration(editorInformationsData.AnimationID, editorInformationsData.CommonGameConfigurations.CoreGameConfigurations.AnimationConfiguration, editorProfile);
        }
    }
}