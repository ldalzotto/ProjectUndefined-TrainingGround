using CreationWizard;
using RTPuzzle;

namespace Editor_PlayerActionVariantCreationWizardEditor
{
    public class AttractiveObjectActionInherentDataCreation : CreateableScriptableObjectComponent<AttractiveObjectActionInherentData>
    {
        #region External dependencies
        private GameConfiguration gameConfiguration;
        private GenericInformations genericInformations;
        #endregion

        public void SetDependencies(GameConfiguration gameConfiguration, GenericInformations genericInformations)
        {
            this.gameConfiguration = gameConfiguration;
            this.genericInformations = genericInformations;
        }

        public AttractiveObjectActionInherentDataCreation(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        protected override string objectFieldLabel => "Attractive object player action data";

        protected override string foldoutLabel => "Attractive object player action data :";

        protected override string headerDescriptionLabel => "The configuration of the attractive object player action.";

        public override string ComputeWarningState()
        {
            if (gameConfiguration.PlayerActionConfiguration != null && gameConfiguration.PlayerActionConfiguration.ConfigurationInherentData.ContainsKey(this.genericInformations.PlayerActionId))
            {
                return ErrorMessages.GetConfigurationOverriteMessage(this.genericInformations.PlayerActionId, gameConfiguration.PlayerActionConfiguration.name);
            }
            return null;
        }

        public override string ComputeErrorState()
        {
            if (this.IsNew && this.CreatedObject.CoolDownTime < 0)
            {
                return "CooldownTime must be >= 0";
            }
            return null;
        }
    }

}
