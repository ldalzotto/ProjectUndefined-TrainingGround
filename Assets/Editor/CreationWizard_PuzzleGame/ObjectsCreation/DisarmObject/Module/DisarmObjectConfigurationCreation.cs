using Editor_MainGameCreationWizard;
using GameConfigurationID;
using RTPuzzle;
using UnityEditor;

namespace Editor_DisarmObjectCreationWizard
{
    [System.Serializable]
    public class DisarmObjectConfigurationCreation : CreateableScriptableObjectComponent<DisarmObjectInherentData>
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInfomrationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            this.CreateAsset(InstancePath.DisarmObjectInherentDatapath, editorInfomrationsData.DisarmObjectID.ToString() + NameConstants.DisarmObjectInherentData, editorProfile);
            this.AddToGameConfiguration(editorInfomrationsData.DisarmObjectID, editorInfomrationsData.CommonGameConfigurations.PuzzleGameConfigurations.DisarmObjectConfiguration, editorProfile);
        }
    }
}