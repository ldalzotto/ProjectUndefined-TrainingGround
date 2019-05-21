using UnityEngine;
using System.Collections;
using RTPuzzle;
using Editor_PuzzleGameCreationWizard;

namespace Editor_AICreationObjectCreationWizard
{
    [System.Serializable]
    public class AIBehaviorConfigurationCreation : CreateableScriptableObjectComponent<AIBehaviorInherentData>
    {
        public AIBehaviorConfigurationCreation(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        protected override string objectFieldLabel => typeof(AIBehaviorConfigurationCreation).Name;

        public void OnGenerationClicked(EditorInformationsData editorInformationsData, AbstractCreationWizardEditorProfile editorProfile)
        {
            var createdBehavior = this.CreateAsset(editorInformationsData.CommonGameConfigurations.InstancePath.AIBehaviorConfigurationPath, editorInformationsData.AiID.ToString() + NameConstants.AIBehavior);
            editorProfile.AddToGeneratedObjects(new Object[] { createdBehavior });

            editorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.AIComponentsConfiguration.SetEntry(editorInformationsData.AiID, createdBehavior);
            editorProfile.GameConfigurationModified(editorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.AIComponentsConfiguration, editorInformationsData.AiID, createdBehavior);
        }
    }
}