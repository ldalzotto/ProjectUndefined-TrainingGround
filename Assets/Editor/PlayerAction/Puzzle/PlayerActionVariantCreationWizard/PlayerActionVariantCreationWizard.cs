using UnityEngine.SceneManagement;

namespace Editor_PlayerActionVariantCreationWizardEditor
{
    public class PlayerActionVariantCreationWizard : AbstractCreationWizardEditor<PlayerActionVariantCreationWizardEditorProfile>
    {
        protected override void OnGenerationClicked(Scene tmpScene)
        {
        }

        protected override void OnWizardGUI()
        {
            this.editorProfile.AttractiveObjectActionInherentDataCreation.SetDependencies(this.editorProfile.GameConfiguration, this.editorProfile.GenericInformations);

            this.editorProfile.GenericInformations.OnInspectorGUI();
            this.editorProfile.GameConfiguration.OnInspectorGUI();
            this.editorProfile.AttractiveObjectActionInherentDataCreation.OnInspectorGUI();
        }
    }

}