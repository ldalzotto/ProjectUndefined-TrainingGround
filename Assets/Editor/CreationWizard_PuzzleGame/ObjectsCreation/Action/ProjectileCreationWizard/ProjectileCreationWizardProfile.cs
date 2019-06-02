using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Editor_ProjectileCreationWizard
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ProjectileCreationWizardProfile", menuName = "CreationWizard/GameCreationWizardEditorProfile/Action/Projectile/ProjectileCreationWizardProfile", order = 1)]
    public class ProjectileCreationWizardProfile : AbstractCreationWizardEditorProfile
    {
        private List<CreationWizardOrderConfiguration> ModuleTypes_IMPL = new List<CreationWizardOrderConfiguration>() {
            new CreationWizardOrderConfiguration(typeof(EditorInformations), -1),
            new CreationWizardOrderConfiguration(typeof(ProjectilePrefabCreation), 0),
            new CreationWizardOrderConfiguration(typeof(ProjectileConfigurationDataCreation), 1)
        };

        public override List<CreationWizardOrderConfiguration> ModulesConfiguration => this.ModuleTypes_IMPL;
    }
}

