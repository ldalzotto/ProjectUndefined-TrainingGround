using System.Collections.Generic;
using UnityEngine;

namespace Editor_ActionInteractableObjectCreationWizard
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ActionInteractableObjectCreationWizardProfile", menuName = "CreationWizard/GameCreationWizardEditorProfile/ActionInteractableObject/ActionInteractableObjectCreationWizardProfile", order = 1)]
    public class ActionInteractableObjectCreationWizardProfile : AbstractCreationWizardEditorProfile
    {
        private List<CreationWizardOrderConfiguration> ModuleTypes_IMPL = new List<CreationWizardOrderConfiguration>() {
            new CreationWizardOrderConfiguration(typeof(EditorInformations), -1),
            new CreationWizardOrderConfiguration(typeof(ActionInteractableObjectConfigurationCreation), 0),
        };

        public override List<CreationWizardOrderConfiguration> ModulesConfiguration => this.ModuleTypes_IMPL;
    }
}