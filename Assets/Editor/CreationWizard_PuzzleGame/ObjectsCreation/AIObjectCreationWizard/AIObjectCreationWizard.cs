using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Editor_AICreationObjectCreationWizard
{
    public class AIObjectCreationWizard : AbstractCreationWizardEditor<AIObjectCreationWizardProfile>
    {

        protected override void OnGenerationClicked(Scene tmpScene)
        {
            var modules = this.editorProfile.GetAllModules();
            modules.AIPrefabCreation.OnGenerationClicked(modules.EditorInformations.EditorInformationsData, this.editorProfile);
            modules.AIBehaviorConfigurationCreation.OnGenerationClicked(modules.EditorInformations.EditorInformationsData, this.editorProfile);
        }

        protected override void OnWizardGUI()
        {
            var modules = this.editorProfile.GetAllModules();
            modules.EditorInformations.OnInspectorGUI(ref this.editorProfile.Modules);
            modules.AIPrefabCreation.OnInspectorGUI(ref this.editorProfile.Modules);
            modules.AIBehaviorConfigurationCreation.OnInspectorGUI(ref this.editorProfile.Modules);
        }
    }

}
