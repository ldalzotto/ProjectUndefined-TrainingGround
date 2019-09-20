using Editor_MainGameCreationWizard;
using GameConfigurationID;
using RTPuzzle;
using UnityEditor;

namespace Editor_ActionInteractableObjectCreationWizard
{
    [System.Serializable]
    public class ActionInteractableObjectConfigurationCreation : CreateableScriptableObjectComponent<ActionInteractableObjectInherentData>
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInfomrationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var ActionInteractableObjectConfiguration = editorInfomrationsData.CommonGameConfigurations.PuzzleGameConfigurations.ActionInteractableObjectConfiguration;
            this.CreateAsset(InstancePath.GetConfigurationDataPath(ActionInteractableObjectConfiguration), 
                editorInfomrationsData.ActionInteractableObjectID.ToString() + "_" + this.GetType().BaseType.GetGenericArguments()[0].Name, editorProfile);
            this.AddToGameConfiguration(editorInfomrationsData.ActionInteractableObjectID, ActionInteractableObjectConfiguration, editorProfile);
        }
    }
}