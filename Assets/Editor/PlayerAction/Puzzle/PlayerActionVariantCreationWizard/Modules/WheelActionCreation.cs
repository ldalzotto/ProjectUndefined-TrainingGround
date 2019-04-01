using CreationWizard;

namespace Editor_PlayerActionVariantCreationWizardEditor
{
    public class WheelActionCreation : CreateableScriptableObjectComponent<RTPuzzle.SelectionWheelNodeConfigurationData>
    {
        private GameConfiguration gameConfiguration;
        private GenericInformations genericInformations;

        internal void SetDependencies(GameConfiguration gameConfiguration, GenericInformations genericInformations)
        {
            this.gameConfiguration = gameConfiguration;
            this.genericInformations = genericInformations;
        }

        public WheelActionCreation(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        protected override string objectFieldLabel => "Wheel node configuration";

        protected override string foldoutLabel => "Wheel node configuration : ";

        public override string ComputeWarningState()
        {
            if (this.IsNew && this.gameConfiguration.SelectionWheelNodeConfiguration.ConfigurationInherentData.ContainsKey(this.genericInformations.SelectionWheelNodeConfigurationId))
            {
                return ErrorMessages.GetConfigurationOverriteMessage(this.genericInformations.SelectionWheelNodeConfigurationId, this.gameConfiguration.SelectionWheelNodeConfiguration.name);
            }
            return string.Empty;
        }


    }

}
