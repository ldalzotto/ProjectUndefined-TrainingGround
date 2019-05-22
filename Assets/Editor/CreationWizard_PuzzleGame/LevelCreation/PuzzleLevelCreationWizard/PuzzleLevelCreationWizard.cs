using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor_PuzzleLevelCreationWizard
{
    public class PuzzleLevelCreationWizard : AbstractCreationWizardEditor<PuzzleLevelCreationWizardEditorProfile>
    {
        /*
        protected override void OnGenerationClicked(Scene tmpScene)
        {
            this.GetModule<LevelConfigurationCreation>().OnGenerationClicked(this.editorProfile);
            this.GetModule<LevelCompletionCreation>().OnGenerationClicked(this.editorProfile);
            this.GetModule<LevelCompletionConditionCreation>().OnGenerationClicked(this.editorProfile);
            this.GetModule<PuzzleLevelDynamicsCreation>().OnGenerationClicked(this.editorProfile);
            this.GetModule<LevelHierarchyCreation>().OnGenerationClicked(this.editorProfile);
            this.GetModule<LevelSceneConfigurationCreation>().OnGenerationClicked(this.editorProfile);
            this.GetModule<LevelChunkPrefabCreation>().OnGenerationClicked(this.editorProfile);
            this.GetModule<LevelChunkConfigurationCreation>().OnGenerationClicked(this.editorProfile);

            this.GetModule<LevelSceneCreation>().OnGenerationClicked(this.editorProfile);
            this.GetModule<LevelChunkSceneCreation>().OnGenerationClicked(this.editorProfile);

            this.GetModule<LevelSceneConfigurationCreation>().AfterSceneGeneration(this.GetModule<LevelSceneCreation>());
            this.GetModule<LevelChunkConfigurationCreation>().AfterSceneCreation(this.GetModule<EditorInformations>().EditorInformationsData, this.GetModule<LevelChunkSceneCreation>());
        }
        */

    }
}

