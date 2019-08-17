using Editor_MainGameCreationWizard;
using GameConfigurationID;
using RTPuzzle;
using UnityEditor;

namespace Editor_NearPlayerGameOverTriggerCreationWizard
{
    [System.Serializable]
    public class NearPlayerGameOverTriggerConfigurationCreation : CreateableScriptableObjectComponent<NearPlayerGameOverTriggerInherentData>
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInfomrationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            this.CreateAsset(InstancePath.NearPlayerGameOverTriggerInherentDataPath, editorInfomrationsData.NearPlayerGameOverTriggerID.ToString() + NameConstants.NearPlayerGameOverTriggerInherentData, editorProfile);
            this.AddToGameConfiguration(editorInfomrationsData.NearPlayerGameOverTriggerID, editorInfomrationsData.CommonGameConfigurations.PuzzleGameConfigurations.NearPlayerGameOverTriggerConfiguration, editorProfile);
        }
    }
}