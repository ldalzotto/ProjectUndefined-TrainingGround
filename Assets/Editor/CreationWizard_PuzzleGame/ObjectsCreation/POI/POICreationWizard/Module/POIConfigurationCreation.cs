using AdventureGame;
using Editor_MainGameCreationWizard;

namespace Editor_POICreationWizard
{
    [System.Serializable]
    public class POIConfigurationCreation : CreateableScriptableObjectComponent<PointOfInterestInherentData>
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            this.CreateAsset(InstancePath.POIInherentDataPath,
                 editorInformationsData.PointOfInterestID.ToString() + NameConstants.POIInherentData, editorProfile);
            this.AddToGameConfiguration(editorInformationsData.PointOfInterestID, editorInformationsData.CommonGameConfigurations.AdventureGameConfigurations.PointOfInterestConfiguration, editorProfile);
        }
    }
}