using Editor_MainGameCreationWizard;
using RTPuzzle;

namespace Editor_ObjectRepelCreationWizard
{
    [System.Serializable]
    public class RepelableObjectConfigurationCreation : CreateableScriptableObjectComponent<ObjectRepelInherentData>
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformations = editorProfile.GetModule<EditorInformations>();
            var ObjectRepelConfiguration = editorInformations.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.ObjectRepelConfiguration;
            this.CreateAsset(InstancePath.GetConfigurationDataPath(ObjectRepelConfiguration),
                editorInformations.EditorInformationsData.RepelableObjectID.ToString() + "_" + this.GetType().BaseType.GetGenericArguments()[0].Name, editorProfile);
            this.AddToGameConfiguration(editorInformations.EditorInformationsData.RepelableObjectID, ObjectRepelConfiguration, editorProfile);
        }
    }
}