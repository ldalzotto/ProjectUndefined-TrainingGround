using System.Collections.Generic;
using UnityEngine;

namespace Editor_InteractiveObjectCreationWizard
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "InteractiveObjectCreationWizardProfile", menuName = "CreationWizard/GameCreationWizardEditorProfile/InteractiveObject/InteractiveObjectCreationWizardProfile", order = 1)]
    public class InteractiveObjectCreationWizardProfile : AbstractCreationWizardEditorProfile
    {
        private List<CreationWizardOrderConfiguration> ModuleTypes_IMPL = new List<CreationWizardOrderConfiguration>() {
            new CreationWizardOrderConfiguration(typeof(EditorInformations), -1),
             new CreationWizardOrderConfiguration(typeof(InteractiveObjectPrefabCreation), 0)
        };

        public override List<CreationWizardOrderConfiguration> ModulesConfiguration => this.ModuleTypes_IMPL;
    }
}