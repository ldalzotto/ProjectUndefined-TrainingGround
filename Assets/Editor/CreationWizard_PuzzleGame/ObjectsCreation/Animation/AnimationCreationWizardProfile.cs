using System.Collections.Generic;
using UnityEngine;

namespace Editor_AnimationCreationWizard
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AnimationCreationWizardProfile", menuName = "CreationWizard/GameCreationWizardEditorProfile/Animation/AnimationCreationWizardProfile", order = 1)]
    public class AnimationCreationWizardProfile : AbstractCreationWizardEditorProfile
    {
        private List<CreationWizardOrderConfiguration> ModuleTypes_IMPL = new List<CreationWizardOrderConfiguration>() {
            new CreationWizardOrderConfiguration( typeof(EditorInformations), -1),
            new CreationWizardOrderConfiguration(typeof(AnimationConfigurationDataCreation), 0)
        };
        public override List<CreationWizardOrderConfiguration> ModulesConfiguration => this.ModuleTypes_IMPL;
    }

}
