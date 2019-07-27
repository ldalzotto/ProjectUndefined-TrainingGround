using System.Collections.Generic;
using UnityEngine;

namespace Editor_DisarmObjectCreationWizard
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "DisarmObjectCreationWizardProfile", menuName = "CreationWizard/GameCreationWizardEditorProfile/DisarmObject/DisarmObjectCreationWizardProfile", order = 1)]
    public class DisarmObjectCreationWizardProfile : AbstractCreationWizardEditorProfile
    {
        private List<CreationWizardOrderConfiguration> ModuleTypes_IMPL = new List<CreationWizardOrderConfiguration>() {
            new CreationWizardOrderConfiguration(typeof(EditorInformations), -1),
            new CreationWizardOrderConfiguration(typeof(DisarmObjectConfigurationCreation), 0)
        };

        public override List<CreationWizardOrderConfiguration> ModulesConfiguration => this.ModuleTypes_IMPL;
    }
}