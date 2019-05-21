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
            this.InitModule<LevelSceneCreation>(true, true, false);
            this.InitModule<LevelConfigurationCreation>(true, true, false);
            this.InitModule<LevelCompletionCreation>(true, true, false);
            this.InitModule<LevelCompletionConditionCreation>(true, true, false);
            this.InitModule<PuzzleLevelDynamicsCreation>(true, true, false);
            this.InitModule<LevelHierarchyCreation>(true, true, false);
            this.InitModule<LevelSceneConfigurationCreation>(true, true, false);
            this.InitModule<LevelChunkPrefabCreation>(true, true, false);
            this.InitModule<LevelChunkSceneCreation>(true, true, false);
            this.InitModule<LevelChunkConfigurationCreation>(true, true, false);
        }

        public override void OnGenerationEnd()
        {
            this.GetModule<LevelConfigurationCreation>().OnGenerationEnd();
            this.GetModule<LevelCompletionCreation>().OnGenerationEnd();
            this.GetModule<LevelCompletionConditionCreation>().OnGenerationEnd();
            this.GetModule<PuzzleLevelDynamicsCreation>().OnGenerationEnd();
            this.GetModule<LevelHierarchyCreation>().OnGenerationEnd();
            this.GetModule<LevelSceneConfigurationCreation>().OnGenerationEnd();
            this.GetModule<LevelChunkPrefabCreation>().OnGenerationEnd();
            this.GetModule<LevelChunkConfigurationCreation>().OnGenerationEnd();
        }

        public CreationWizardModules GetAllModules()
        {
            return new CreationWizardModules(
                this.GetModule<EditorInformations>(),
                this.GetModule<LevelSceneCreation>(),
                this.GetModule<LevelConfigurationCreation>(),
                this.GetModule<LevelCompletionCreation>(),
                this.GetModule<LevelCompletionConditionCreation>(),
                this.GetModule<PuzzleLevelDynamicsCreation>(),
                this.GetModule<LevelHierarchyCreation>(),
                this.GetModule<LevelSceneConfigurationCreation>(),
                this.GetModule<LevelChunkPrefabCreation>(),
                this.GetModule<LevelChunkSceneCreation>(),
                this.GetModule<LevelChunkConfigurationCreation>()
            );
        }

        public static CreationWizardModules GetAllModules(Dictionary<string, CreationModuleComponent> modules)
        {
            return new CreationWizardModules(
              (EditorInformations)modules[typeof(EditorInformations).Name],
              (LevelSceneCreation)modules[typeof(LevelSceneCreation).Name],
              (LevelConfigurationCreation)modules[typeof(LevelConfigurationCreation).Name],
              (LevelCompletionCreation)modules[typeof(LevelCompletionCreation).Name],
              (LevelCompletionConditionCreation)modules[typeof(LevelCompletionConditionCreation).Name],
              (PuzzleLevelDynamicsCreation)modules[typeof(PuzzleLevelDynamicsCreation).Name],
              (LevelHierarchyCreation)modules[typeof(LevelHierarchyCreation).Name],
              (LevelSceneConfigurationCreation)modules[typeof(LevelSceneConfigurationCreation).Name],
              (LevelChunkPrefabCreation)modules[typeof(LevelChunkPrefabCreation).Name],
              (LevelChunkSceneCreation)modules[typeof(LevelChunkSceneCreation).Name],
              (LevelChunkConfigurationCreation)modules[typeof(LevelChunkConfigurationCreation).Name]
           );
        }
    }

    public class CreationWizardModules
    {
        public EditorInformations EditorInformations;
        public LevelSceneCreation SceneCreation;
        public LevelConfigurationCreation LevelConfigurationCreation;
        public LevelCompletionCreation LevelCompletionCreation;
        public LevelCompletionConditionCreation LevelCompletionCreationCondition;
        public PuzzleLevelDynamicsCreation PuzzleLevelDynamicsCreation;
        public LevelHierarchyCreation LevelHierarchyCreation;
        public LevelSceneConfigurationCreation LevelSceneConfigurationCreation;
        public LevelChunkPrefabCreation LevelChunkPrefabCreation;
        public LevelChunkSceneCreation LevelChunkSceneCreation;
        public LevelChunkConfigurationCreation LevelChunkConfigurationCreation;

        public CreationWizardModules(EditorInformations editorInformations, LevelSceneCreation sceneCreation, LevelConfigurationCreation levelConfigurationCreation, LevelCompletionCreation levelCompletionCreation, LevelCompletionConditionCreation levelCompletionCreationCondition, PuzzleLevelDynamicsCreation puzzleLevelDynamicsCreation, LevelHierarchyCreation levelHierarchyCreation, LevelSceneConfigurationCreation levelSceneConfigurationCreation, LevelChunkPrefabCreation levelChunkPrefabCreation, LevelChunkSceneCreation levelChunkSceneCreation, LevelChunkConfigurationCreation levelChunkConfigurationCreation)
        {
            EditorInformations = editorInformations;
            SceneCreation = sceneCreation;
            LevelConfigurationCreation = levelConfigurationCreation;
            LevelCompletionCreation = levelCompletionCreation;
            LevelCompletionCreationCondition = levelCompletionCreationCondition;
            PuzzleLevelDynamicsCreation = puzzleLevelDynamicsCreation;
            LevelHierarchyCreation = levelHierarchyCreation;
            LevelSceneConfigurationCreation = levelSceneConfigurationCreation;
            LevelChunkPrefabCreation = levelChunkPrefabCreation;
            LevelChunkSceneCreation = levelChunkSceneCreation;
            LevelChunkConfigurationCreation = levelChunkConfigurationCreation;
        }
    }

}
