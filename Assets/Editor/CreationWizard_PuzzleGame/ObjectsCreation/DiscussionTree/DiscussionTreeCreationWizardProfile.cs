using System.Collections.Generic;
using UnityEngine;

namespace Editor_DiscussionTreeCreationWizard
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "DiscussionTreeCreationWizardProfile", menuName = "CreationWizard/GameCreationWizardEditorProfile/DiscussionTree/DiscussionTreeCreationWizardProfile", order = 1)]
    public class DiscussionTreeCreationWizardProfile : AbstractCreationWizardEditorProfile
    {
        private List<CreationWizardOrderConfiguration> ModuleTypes_IMPL = new List<CreationWizardOrderConfiguration>() {
            new CreationWizardOrderConfiguration( typeof(EditorInformations), -1),
            new CreationWizardOrderConfiguration( typeof(DiscussionTreeDataCreation), 0),
        };
        public override List<CreationWizardOrderConfiguration> ModulesConfiguration => this.ModuleTypes_IMPL;
    }

}
