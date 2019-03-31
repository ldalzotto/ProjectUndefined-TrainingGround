﻿using RTPuzzle;

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

        public override string ComputeWarningState()
        {
            if (this.IsNew && gameConfiguration.PlayerActionConfiguration.ConfigurationInherentData.ContainsKey(this.genericInformations.PlayerActionId))
            {
                return "On generation, the key " + this.genericInformations.PlayerActionId + " of " + gameConfiguration.PlayerActionConfiguration.name + " will be overriten.";
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
