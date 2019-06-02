using System.Collections.Generic;
using UnityEngine;

namespace Editor_POICreationWizard
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ProjectileCreationWizardProfile", menuName = "CreationWizard/GameCreationWizardEditorProfile/POI/POICreationWizardProfile", order = 1)]
    public class POICreationWizardProfile : AbstractCreationWizardEditorProfile
    {
        private List<CreationWizardOrderConfiguration> ModuleTypes_IMPL = new List<CreationWizardOrderConfiguration>() {
            new CreationWizardOrderConfiguration( typeof(EditorInformations), -1),
            new CreationWizardOrderConfiguration( typeof(POIConfigurationCreation), 0),
            new CreationWizardOrderConfiguration( typeof(POIPrefabCreation), 1)
        };
        public override List<CreationWizardOrderConfiguration> ModulesConfiguration => this.ModuleTypes_IMPL;
    }

}
