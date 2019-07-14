using System.Collections.Generic;
using UnityEngine;

namespace Editor_RepelableObjectCreationWizard
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "RepelableObjectCreationWizardProfile", menuName = "CreationWizard/GameCreationWizardEditorProfile/InteractiveObject/RepelableObject/RepelableObjectCreationWizardProfile", order = 1)]
    public class RepelableObjectCreationWizardProfile : AbstractCreationWizardEditorProfile
    {
        private List<CreationWizardOrderConfiguration> ModuleTypes_IMPL = new List<CreationWizardOrderConfiguration>() {
            new CreationWizardOrderConfiguration(typeof(EditorInformations), -1),
            new CreationWizardOrderConfiguration(typeof(RepelableObjectConfigurationCreation), 0)
        };

        public override List<CreationWizardOrderConfiguration> ModulesConfiguration => this.ModuleTypes_IMPL;
    }
}