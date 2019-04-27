using System.Collections.Generic;
using CreationWizard;

namespace Editor_PlayerActionVariantCreationWizardEditor
{
    public class WheelActionCreation : CreateableScriptableObjectComponent<RTPuzzle.SelectionWheelNodeConfigurationData>
    {

        public WheelActionCreation(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        protected override string objectFieldLabel => "Wheel node configuration";

        protected override string foldoutLabel => "Wheel node configuration : ";

        protected override string headerDescriptionLabel => "The configuration of the UI wheel node.";

        public override string ComputeWarningState(ref Dictionary<string, CreationModuleComponent> editorModules)
        {
            var gameConfiguration = (GameConfiguration)editorModules[typeof(GameConfiguration).Name];
            var genericInformations = (GenericInformations)editorModules[typeof(GenericInformations).Name];

            if (this.IsNew && gameConfiguration.SelectionWheelNodeConfiguration.ConfigurationInherentData.ContainsKey(genericInformations.SelectionWheelNodeConfigurationId))
            {
                return ErrorMessages.GetConfigurationOverriteMessage(genericInformations.SelectionWheelNodeConfigurationId, gameConfiguration.SelectionWheelNodeConfiguration.name);
            }
            return string.Empty;
        }

    }

}
