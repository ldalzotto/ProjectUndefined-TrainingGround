using UnityEngine;
using System.Collections;
using RTPuzzle;
using Editor_PuzzleGameCreationWizard;

namespace Editor_AICreationObjectCreationWizard
{
    [System.Serializable]
    public class AIBehaviorConfigurationCreation : CreateableScriptableObjectComponent<AIBehaviorInherentData>
    {

        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var createdBehavior = this.CreateAsset(editorInformationsData.CommonGameConfigurations.InstancePath.AIBehaviorConfigurationPath, editorInformationsData.AiID.ToString() + NameConstants.AIBehavior);
            editorProfile.AddToGeneratedObjects(new Object[] { createdBehavior });

            editorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.AIComponentsConfiguration.SetEntry(editorInformationsData.AiID, createdBehavior);
            editorProfile.GameConfigurationModified(editorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.AIComponentsConfiguration, editorInformationsData.AiID, createdBehavior);

        }
    }
}