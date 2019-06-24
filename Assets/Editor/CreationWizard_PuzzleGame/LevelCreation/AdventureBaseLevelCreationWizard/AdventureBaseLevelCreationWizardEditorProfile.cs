using System.Collections.Generic;
using UnityEngine;

namespace Editor_AdventureBaseLevelCreationWizard
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AdventureBaseLevelCreationWizardEditorProfile", menuName = "CreationWizard/GameCreationWizardEditorProfile/AdventureBaseLevelCreationWizardEditorProfile", order = 1)]
    public class AdventureBaseLevelCreationWizardEditorProfile : AbstractCreationWizardEditorProfile
    {

        private List<CreationWizardOrderConfiguration> ModuleTypes_IMPL = new List<CreationWizardOrderConfiguration>()
        {
           new CreationWizardOrderConfiguration(typeof(EditorInformations), -1),
           new CreationWizardOrderConfiguration(typeof(AdventureLevelDynamicCreation), 0),
           new CreationWizardOrderConfiguration(typeof(LevelHierarchyCreation), 1),
           new CreationWizardOrderConfiguration(typeof(LevelSceneConfigurationCreation), 2, 0),
           new CreationWizardOrderConfiguration(typeof(LevelSceneCreation), 3),
        };
        public override List<CreationWizardOrderConfiguration> ModulesConfiguration => this.ModuleTypes_IMPL;
    }

}
