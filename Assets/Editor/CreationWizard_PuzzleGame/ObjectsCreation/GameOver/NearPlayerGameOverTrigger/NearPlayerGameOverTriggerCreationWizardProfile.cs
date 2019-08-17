using System.Collections.Generic;
using UnityEngine;

namespace Editor_NearPlayerGameOverTriggerCreationWizard
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "NearPlayerGameOverTriggerCreationWizardProfile", menuName = "CreationWizard/GameCreationWizardEditorProfile/NearPlayerGameOverTrigger/NearPlayerGameOverTriggerCreationWizardProfile", order = 1)]
    public class NearPlayerGameOverTriggerCreationWizardProfile : AbstractCreationWizardEditorProfile
    {
        private List<CreationWizardOrderConfiguration> ModuleTypes_IMPL = new List<CreationWizardOrderConfiguration>() {
            new CreationWizardOrderConfiguration(typeof(EditorInformations), -1),
            new CreationWizardOrderConfiguration(typeof(NearPlayerGameOverTriggerConfigurationCreation), 0)
        };

        public override List<CreationWizardOrderConfiguration> ModulesConfiguration => this.ModuleTypes_IMPL;
    }
}