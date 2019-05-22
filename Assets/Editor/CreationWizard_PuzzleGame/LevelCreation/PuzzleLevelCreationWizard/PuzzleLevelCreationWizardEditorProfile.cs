using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Editor_PuzzleLevelCreationWizard
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "PuzzleLevelCreationWizardEditorProfile", menuName = "CreationWizard/PuzzleLevelCreationWizard/PuzzleLevelCreationWizardEditorProfile", order = 1)]
    public class PuzzleLevelCreationWizardEditorProfile : AbstractCreationWizardEditorProfile
    {

        private List<CreationWizardOrderConfiguration> ModuleTypes_IMPL = new List<CreationWizardOrderConfiguration>()
        {
           new CreationWizardOrderConfiguration(typeof(EditorInformations), -1),
           new CreationWizardOrderConfiguration(typeof(LevelSceneCreation), 8),
           new CreationWizardOrderConfiguration(typeof(LevelConfigurationCreation), 0),
           new CreationWizardOrderConfiguration(typeof(LevelCompletionCreation), 1),
           new CreationWizardOrderConfiguration(typeof(LevelCompletionConditionCreation), 2),
           new CreationWizardOrderConfiguration(typeof(PuzzleLevelDynamicsCreation), 3),
           new CreationWizardOrderConfiguration(typeof(LevelHierarchyCreation), 4),
           new CreationWizardOrderConfiguration(typeof(LevelSceneConfigurationCreation), 5, 0),
           new CreationWizardOrderConfiguration(typeof(LevelChunkPrefabCreation), 6),
           new CreationWizardOrderConfiguration(typeof(LevelChunkConfigurationCreation), 7,1),
           new CreationWizardOrderConfiguration(typeof(LevelChunkSceneCreation), 9)
        };
        public override List<CreationWizardOrderConfiguration> ModulesConfiguration => this.ModuleTypes_IMPL;
    }

}
