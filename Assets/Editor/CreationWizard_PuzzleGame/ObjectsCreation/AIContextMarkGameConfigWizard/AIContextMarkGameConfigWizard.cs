using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Editor_AIContextMarkGameConfigWizard
{
    public class AIContextMarkGameConfigWizard : AbstractCreationWizardEditor<AIContextMarkGameConfigEditorProfile>
    {
        protected override void OnGenerationClicked(Scene tmpScene)
        {
            var GenericInformations = this.GetModule<GenericInformations>();
            var GameConfiguration = this.GetModule<GameConfiguration>();
            var AIContextMarkConfigurationCreator = this.GetModule<AIContextMarkConfigurationCreator>();

            var aiFeedbackInherentData = AIContextMarkConfigurationCreator.CreateAsset(GenericInformations.PathConfiguration.AIContextMarkConfigurationPath, 
                NamingConventionHelper.BuildName(GenericInformations.BaseName, PrefixType.AI_FEEDBACK_MARK, SufixType.AI_FEEDBACK_MARK_INHERENT_DATA));
            this.editorProfile.AddToGeneratedObjects(new Object[] { aiFeedbackInherentData });
            GameConfiguration.ContextMarkVisualFeedbackConfiguration.SetEntry(GenericInformations.AiID, aiFeedbackInherentData);
        }

        protected override void OnWizardGUI()
        {
            this.GetModule<GenericInformations>().OnInspectorGUI(ref this.editorProfile.Modules);
            this.GetModule<GameConfiguration>().OnInspectorGUI(ref this.editorProfile.Modules);
            this.GetModule<AIContextMarkConfigurationCreator>().OnInspectorGUI(ref this.editorProfile.Modules);
        }
    }

}
