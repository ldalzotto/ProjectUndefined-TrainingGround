using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Editor_TargetZoneCreationWizard
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "TargetZoneCreationWizardProfile", menuName = "CreationWizard/GameCreationWizardEditorProfile/TargetZone/TargetZoneCreationWizardProfile", order = 1)]

    public class TargetZoneCreationWizardProfile : AbstractCreationWizardEditorProfile
    {
        private List<CreationWizardOrderConfiguration> ModuleTypes_IMPL = new List<CreationWizardOrderConfiguration>() {
            new CreationWizardOrderConfiguration( typeof(EditorInformations), -1),
            new CreationWizardOrderConfiguration( typeof(TargetZoneConfigurationCreation), 0)
        };
        public override List<CreationWizardOrderConfiguration> ModulesConfiguration => this.ModuleTypes_IMPL;
    }
}