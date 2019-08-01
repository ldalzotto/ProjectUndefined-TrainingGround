using Editor_MainGameCreationWizard;
using RTPuzzle;

namespace Editor_RepelableObjectCreationWizard
{
    [System.Serializable]
    public class RepelableObjectConfigurationCreation : CreateableScriptableObjectComponent<RepelableObjectsInherentData>
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformations = editorProfile.GetModule<EditorInformations>();
            this.CreateAsset(InstancePath.RepelableObjectInherentDataPath, editorInformations.EditorInformationsData.RepelableObjectID.ToString() + NameConstants.RepelableObjectInherentData, editorProfile);
            this.AddToGameConfiguration(editorInformations.EditorInformationsData.RepelableObjectID, editorInformations.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.RepelableObjectsConfiguration, editorProfile);
        }
    }
}