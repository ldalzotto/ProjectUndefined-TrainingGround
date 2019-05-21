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
            modules.LevelHierarchyCreation.OnGenerationClicked(modules.EditorInformations.EditorInformationsData, this.editorProfile);
            modules.LevelSceneConfigurationCreation.OnGenerationClicked(modules.EditorInformations.EditorInformationsData, this.editorProfile);
            modules.LevelChunkPrefabCreation.OnGenerationClicked(modules.EditorInformations.EditorInformationsData, this.editorProfile);
            modules.LevelChunkConfigurationCreation.OnGenerationClicked(modules.EditorInformations.EditorInformationsData, this.editorProfile, modules.LevelHierarchyCreation);

            modules.SceneCreation.OnGenerationClicked(modules.EditorInformations.EditorInformationsData, modules.PuzzleLevelDynamicsCreation, this.editorProfile.AddToGeneratedObjects);
            modules.LevelChunkSceneCreation.OnGenerationClicked(modules.EditorInformations.EditorInformationsData, modules.LevelChunkPrefabCreation, this.editorProfile);

            modules.LevelSceneConfigurationCreation.AfterSceneGeneration(modules.SceneCreation);
            modules.LevelChunkConfigurationCreation.AfterSceneCreation(modules.EditorInformations.EditorInformationsData, modules.LevelChunkSceneCreation);
        }

        protected override void OnWizardGUI()
        {
            this.GetModule<EditorInformations>().OnInspectorGUI(ref this.editorProfile.Modules);
            this.GetModule<LevelSceneCreation>().OnInspectorGUI(ref this.editorProfile.Modules);
            this.GetModule<LevelConfigurationCreation>().OnInspectorGUI(ref this.editorProfile.Modules);
            this.GetModule<LevelCompletionCreation>().OnInspectorGUI(ref this.editorProfile.Modules);
            this.GetModule<LevelCompletionConditionCreation>().OnInspectorGUI(ref this.editorProfile.Modules);
            this.GetModule<LevelHierarchyCreation>().OnInspectorGUI(ref this.editorProfile.Modules);
            this.GetModule<PuzzleLevelDynamicsCreation>().OnInspectorGUI(ref this.editorProfile.Modules);
            this.GetModule<LevelSceneConfigurationCreation>().OnInspectorGUI(ref this.editorProfile.Modules);
            this.GetModule<LevelChunkPrefabCreation>().OnInspectorGUI(ref this.editorProfile.Modules);
            this.GetModule<LevelChunkConfigurationCreation>().OnInspectorGUI(ref this.editorProfile.Modules);
            this.GetModule<LevelChunkSceneCreation>().OnInspectorGUI(ref this.editorProfile.Modules);
        }
    }
}

