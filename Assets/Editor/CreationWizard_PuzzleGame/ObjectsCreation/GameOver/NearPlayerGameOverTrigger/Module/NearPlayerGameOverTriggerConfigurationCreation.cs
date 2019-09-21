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
            var NearPlayerGameOverTriggerConfiguration = editorInfomrationsData.CommonGameConfigurations.GetConfiguration<NearPlayerGameOverTriggerConfiguration>();
            this.CreateAsset(InstancePath.GetConfigurationDataPath(NearPlayerGameOverTriggerConfiguration), 
                editorInfomrationsData.NearPlayerGameOverTriggerID.ToString() + "_" + this.GetType().BaseType.GetGenericArguments()[0].Name, editorProfile);
            this.AddToGameConfiguration(editorInfomrationsData.NearPlayerGameOverTriggerID, NearPlayerGameOverTriggerConfiguration, editorProfile);
        }
    }
}