using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Editor_PlayerActionCreationWizard
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "PlayerActionCreationWizardProfile", menuName = "CreationWizard/GameCreationWizardEditorProfile/Action/PlayerAction/PlayerActionCreationWizardProfile", order = 1)]
    public class PlayerActionCreationWizardProfile : AbstractCreationWizardEditorProfile
    {

        private List<CreationWizardOrderConfiguration> ModuleTypes_IMPL = new List<CreationWizardOrderConfiguration>()
        {
            new CreationWizardOrderConfiguration(typeof(EditorInformations), -1),
            new CreationWizardOrderConfiguration(typeof(PlayerActionInherentDataCreation), 0)
        };

        public override List<CreationWizardOrderConfiguration> ModulesConfiguration => this.ModuleTypes_IMPL;
    }

}