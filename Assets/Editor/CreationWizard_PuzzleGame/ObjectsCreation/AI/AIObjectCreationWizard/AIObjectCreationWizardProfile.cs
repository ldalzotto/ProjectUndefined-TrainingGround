using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Editor_AICreationObjectCreationWizard
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AIObjectCreationWizardProfile", menuName = "CreationWizard/PuzzleObjectCreationWizard/AI/AIObjectCreationWizardProfile", order = 1)]
    public class AIObjectCreationWizardProfile : AbstractCreationWizardEditorProfile
    {
        private List<CreationWizardOrderConfiguration> ModuleTypes_IMPL = new List<CreationWizardOrderConfiguration>() {
            new CreationWizardOrderConfiguration( typeof(EditorInformations), 0),
            new CreationWizardOrderConfiguration( typeof(AIBehaviorConfigurationCreation), 1),
            new CreationWizardOrderConfiguration( typeof(AIPrefabCreation), 2)
        };

        public override List<CreationWizardOrderConfiguration> ModulesConfiguration => this.ModuleTypes_IMPL;
    }

}

