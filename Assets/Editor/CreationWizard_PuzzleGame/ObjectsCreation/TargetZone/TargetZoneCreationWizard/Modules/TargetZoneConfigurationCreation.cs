using UnityEngine;
using System.Collections;
using RTPuzzle;
using Editor_MainGameCreationWizard;

namespace Editor_TargetZoneCreationWizard
{
    [System.Serializable]
    public class TargetZoneConfigurationCreation : CreateableScriptableObjectComponent<TargetZoneInherentData>
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            this.CreateAsset(editorInformationsData.CommonGameConfigurations.InstancePath.TargetZoneConfigurationDataPath,
                 editorInformationsData.TargetZoneID.ToString() + NameConstants.TargetZoneInherentData, editorProfile);
            this.AddToGameConfiguration(editorInformationsData.TargetZoneID, editorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.TargetZonesConfiguration, editorProfile);
        }
    }
}