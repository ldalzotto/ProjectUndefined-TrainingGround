using System.Collections.Generic;
using UnityEngine;

namespace Editor_ObjectRepelCreationWizard
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ObjectRepelCreationWizardProfile", menuName = "CreationWizard/GameCreationWizardEditorProfile/InteractiveObject/ObjectRepel/ObjectRepelCreationWizardProfile", order = 1)]
    public class ObjectRepelCreationWizardProfile : AbstractCreationWizardEditorProfile
    {
        private List<CreationWizardOrderConfiguration> ModuleTypes_IMPL = new List<CreationWizardOrderConfiguration>() {
            new CreationWizardOrderConfiguration(typeof(EditorInformations), -1),
            new CreationWizardOrderConfiguration(typeof(RepelableObjectConfigurationCreation), 0)
        };

        public override List<CreationWizardOrderConfiguration> ModulesConfiguration => this.ModuleTypes_IMPL;
    }
}