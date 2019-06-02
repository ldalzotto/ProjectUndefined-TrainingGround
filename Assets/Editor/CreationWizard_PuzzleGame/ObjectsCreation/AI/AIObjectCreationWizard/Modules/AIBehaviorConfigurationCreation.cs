using UnityEngine;
using System.Collections;
using RTPuzzle;
using Editor_MainGameCreationWizard;

namespace Editor_AICreationObjectCreationWizard
{
    [System.Serializable]
    public class AIBehaviorConfigurationCreation : CreateableScriptableObjectComponent<AIBehaviorInherentData>
    {

        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            this.CreateAsset(editorInformationsData.CommonGameConfigurations.InstancePath.AIBehaviorConfigurationPath, editorInformationsData.AiID.ToString() + NameConstants.AIBehavior, editorProfile);
            this.AddToGameConfiguration(editorInformationsData.AiID, editorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.AIComponentsConfiguration, editorProfile);
        }
    }
}