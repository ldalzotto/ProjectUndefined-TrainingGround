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
            this.CreateAsset(InstancePath.ActionInteractableObjectInherentDatapath, editorInfomrationsData.ActionInteractableObjectID.ToString() + NameConstants.ActionInteractableObjectInherentData, editorProfile);
            this.AddToGameConfiguration(editorInfomrationsData.ActionInteractableObjectID, editorInfomrationsData.CommonGameConfigurations.PuzzleGameConfigurations.ActionInteractableObjectConfiguration, editorProfile);
        }
    }
}