using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Editor_PuzzleLevelCreationWizard
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "PuzzleLevelCreationWizardEditorProfile", menuName = "CreationWizard/PuzzleLevelCreationWizard/PuzzleLevelCreationWizardEditorProfile", order = 1)]
    public class PuzzleLevelCreationWizardEditorProfile : AbstractCreationWizardEditorProfile
    {
        public override void OnEnable()
        {
            base.OnEnable();
            this.InitModule<EditorInformations>(false, true, false);
            this.InitModule<SceneCreation>(true, true, false);
            this.InitModule<LevelConfigurationCreation>(true, true, false);
            this.InitModule<LevelCompletionCreation>(true, true, false);
            this.InitModule<LevelCompletionCreationCondition>(true, true, false);
            this.InitModule<PuzzleLevelDynamicsCreation>(true, true, false);
        }

        public override void OnGenerationEnd()
        {
            this.GetModule<LevelConfigurationCreation>().OnGenerationEnd();
            this.GetModule<LevelCompletionCreation>().OnGenerationEnd();
            this.GetModule<LevelCompletionCreationCondition>().OnGenerationEnd();
            this.GetModule<PuzzleLevelDynamicsCreation>().OnGenerationEnd();
        }

        public CreationWizardModules GetAllModules()
        {
            return new CreationWizardModules(
                this.GetModule<EditorInformations>(),
                this.GetModule<SceneCreation>(),
                this.GetModule<LevelConfigurationCreation>(),
                this.GetModule<LevelCompletionCreation>(),
                this.GetModule<LevelCompletionCreationCondition>(),
                this.GetModule<PuzzleLevelDynamicsCreation>()
            );
        }

        public static CreationWizardModules GetAllModules(Dictionary<string, CreationModuleComponent> modules)
        {
            return new CreationWizardModules(
              (EditorInformations)modules[typeof(EditorInformations).Name],
              (SceneCreation)modules[typeof(SceneCreation).Name],
              (LevelConfigurationCreation)modules[typeof(LevelConfigurationCreation).Name],
              (LevelCompletionCreation)modules[typeof(LevelCompletionCreation).Name],
              (LevelCompletionCreationCondition)modules[typeof(LevelCompletionCreationCondition).Name],
              (PuzzleLevelDynamicsCreation)modules[typeof(PuzzleLevelDynamicsCreation).Name]
           );
        }
    }

    public class CreationWizardModules
    {
        public EditorInformations EditorInformations;
        public SceneCreation SceneCreation;
        public LevelConfigurationCreation LevelConfigurationCreation;
        public LevelCompletionCreation LevelCompletionCreation;
        public LevelCompletionCreationCondition LevelCompletionCreationCondition;
        public PuzzleLevelDynamicsCreation PuzzleLevelDynamicsCreation;

        public CreationWizardModules(EditorInformations editorInformations, SceneCreation sceneCreation, LevelConfigurationCreation levelConfigurationCreation, LevelCompletionCreation levelCompletionCreation, LevelCompletionCreationCondition levelCompletionCreationCondition, PuzzleLevelDynamicsCreation puzzleLevelDynamicsCreation)
        {
            EditorInformations = editorInformations;
            SceneCreation = sceneCreation;
            LevelConfigurationCreation = levelConfigurationCreation;
            LevelCompletionCreation = levelCompletionCreation;
            LevelCompletionCreationCondition = levelCompletionCreationCondition;
            PuzzleLevelDynamicsCreation = puzzleLevelDynamicsCreation;
        }
    }

}
