using UnityEngine;
using System.Collections;
using RTPuzzle;
using System.Collections.Generic;
using CreationWizard;

namespace Editor_PuzzleGameCreationWizard
{
    [System.Serializable]
    public class AttractiveObjectInherentDataModule : CreateableScriptableObjectComponent<AttractiveObjectInherentConfigurationData>
    {
        public AttractiveObjectInherentDataModule(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        protected override string objectFieldLabel => "Generated attractive object configuration : ";

        protected override string foldoutLabel => "Object game configuration : ";

        protected override string headerDescriptionLabel => "The configuration of the attractive object.";

        public override string ComputeWarningState(ref Dictionary<string, CreationModuleComponent> editorModules)
        {
            var gameConfiguration = (Configurationretrieval)editorModules[typeof(Configurationretrieval).Name];
            var genericInfor = (GenericInformation)editorModules[typeof(GenericInformation).Name];
            if(this.IsNew && gameConfiguration.AttractiveObjectConfiguration.ConfigurationInherentData.ContainsKey(genericInfor.AttractiveObjectId))
            {
                return ErrorMessages.GetConfigurationOverriteMessage(genericInfor.AttractiveObjectId, gameConfiguration.AttractiveObjectConfiguration.name);
            }
            return string.Empty;
        }
    }
}
