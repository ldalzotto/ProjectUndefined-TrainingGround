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
            var targetZoneConfiguration = editorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.TargetZoneConfiguration;
            this.CreateAsset(InstancePath.GetConfigurationDataPath(targetZoneConfiguration),
                    editorInformationsData.TargetZoneID.ToString() + "_" + this.GetType().BaseType.GetGenericArguments()[0].Name, editorProfile);
            this.AddToGameConfiguration(editorInformationsData.TargetZoneID, targetZoneConfiguration, editorProfile);
        }
    }
}