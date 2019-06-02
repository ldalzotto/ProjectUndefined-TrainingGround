using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Editor_AIBehaviorCreationWizard
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AIBehaviorCreationWizardProfile", menuName = "CreationWizard/GameCreationWizardEditorProfile/AI/AIBehaviorCreationWizardProfile", order = 1)]

    public class AIBehaviorCreationWizardProfile : AbstractCreationWizardEditorProfile
    {
        private List<CreationWizardOrderConfiguration> ModuleTypes_IMPL = new List<CreationWizardOrderConfiguration>() {
            new CreationWizardOrderConfiguration(typeof(EditorInformations), -1),
            new CreationWizardOrderConfiguration(typeof(AIComponentsCreation), 0)
        };
        public override List<CreationWizardOrderConfiguration> ModulesConfiguration => this.ModuleTypes_IMPL;
    }

}
