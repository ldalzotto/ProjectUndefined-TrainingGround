using System.Collections.Generic;
using UnityEngine;

namespace Editor_AttractiveObjectCreationWizard
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AttractiveObjectCreationWizardProfile", menuName = "CreationWizard/GameCreationWizardEditorProfile/Action/AttractiveObject/AttractiveObjectCreationWizardProfile", order = 1)]
    public class AttractiveObjectCreationWizardProfile : AbstractCreationWizardEditorProfile
    {
        private List<CreationWizardOrderConfiguration> ModuleTypes_IMPL = new List<CreationWizardOrderConfiguration>() {
            new CreationWizardOrderConfiguration(typeof(EditorInformations), -1),
            new CreationWizardOrderConfiguration(typeof(AttractiveObjectPrefabCreation), 0),
            new CreationWizardOrderConfiguration(typeof(AttractiveObjectConfigurationCreation), 1)
        };

        public override List<CreationWizardOrderConfiguration> ModulesConfiguration => this.ModuleTypes_IMPL;
    }
}