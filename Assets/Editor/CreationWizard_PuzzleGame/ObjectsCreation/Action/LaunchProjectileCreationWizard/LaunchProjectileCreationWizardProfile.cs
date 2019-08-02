using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Editor_LaunchProjectileCreationWizard
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LaunchProjectileCreationWizardProfile", menuName = "CreationWizard/GameCreationWizardEditorProfile/Action/LaunchProjectile/LaunchProjectileCreationWizardProfile", order = 1)]
    public class LaunchProjectileCreationWizardProfile : AbstractCreationWizardEditorProfile
    {
        private List<CreationWizardOrderConfiguration> ModuleTypes_IMPL = new List<CreationWizardOrderConfiguration>() {
            new CreationWizardOrderConfiguration(typeof(EditorInformations), -1),
            new CreationWizardOrderConfiguration(typeof(ProjectileConfigurationDataCreation), 0)
        };

        public override List<CreationWizardOrderConfiguration> ModulesConfiguration => this.ModuleTypes_IMPL;
    }
}

