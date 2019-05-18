using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor_PuzzleLevelCreationWizard
{
    public class PuzzleLevelCreationWizard : AbstractCreationWizardEditor<PuzzleLevelCreationWizardEditorProfile>
    {
        protected override void OnGenerationClicked(Scene tmpScene)
        {
            var modules = this.editorProfile.GetAllModules();
            modules.LevelConfigurationCreation.OnGenerationClicked(modules.EditorInformations.EditorInformationsData, this.editorProfile);
            modules.LevelCompletionCreation.OnGenerationClicked(modules.EditorInformations.EditorInformationsData, modules.LevelConfigurationCreation, this.editorProfile.AddToGeneratedObjects);
            modules.LevelCompletionCreationCondition.OnGenerationClicked(modules.EditorInformations.EditorInformationsData, modules.LevelCompletionCreation, this.editorProfile.AddToGeneratedObjects);
            modules.PuzzleLevelDynamicsCreation.OnGenerationClicked(modules.EditorInformations.EditorInformationsData, this.editorProfile);
            modules.SceneCreation.OnGenerationClicked(modules.EditorInformations.EditorInformationsData, modules.PuzzleLevelDynamicsCreation, this.editorProfile.AddToGeneratedObjects);
        }

        protected override void OnWizardGUI()
        {
            this.GetModule<EditorInformations>().OnInspectorGUI(ref this.editorProfile.Modules);
            this.GetModule<SceneCreation>().OnInspectorGUI(ref this.editorProfile.Modules);
            this.GetModule<LevelConfigurationCreation>().OnInspectorGUI(ref this.editorProfile.Modules);
            this.GetModule<LevelCompletionCreation>().OnInspectorGUI(ref this.editorProfile.Modules);
            this.GetModule<PuzzleLevelDynamicsCreation>().OnInspectorGUI(ref this.editorProfile.Modules);
            this.GetModule<LevelCompletionCreationCondition>().OnInspectorGUI(ref this.editorProfile.Modules);
        }
    }
}

