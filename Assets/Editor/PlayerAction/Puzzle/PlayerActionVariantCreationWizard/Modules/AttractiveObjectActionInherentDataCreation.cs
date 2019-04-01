using System.Collections.Generic;
using CreationWizard;
using RTPuzzle;

namespace Editor_PlayerActionVariantCreationWizardEditor
{
    public class AttractiveObjectActionInherentDataCreation : CreateableScriptableObjectComponent<AttractiveObjectActionInherentData>
    {
        public AttractiveObjectActionInherentDataCreation(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        protected override string objectFieldLabel => "Attractive object player action data";

        protected override string foldoutLabel => "Attractive object player action data :";

        protected override string headerDescriptionLabel => "The configuration of the attractive object player action.";

        public override string ComputeWarningState(ref Dictionary<string, CreationModuleComponent> editorModules)
        {
            var gameConfiguration = (GameConfiguration) editorModules[typeof(GameConfiguration).Name];
            var genericInformations = (GenericInformations)editorModules[typeof(GenericInformations).Name];

            if (gameConfiguration.PlayerActionConfiguration != null && gameConfiguration.PlayerActionConfiguration.ConfigurationInherentData.ContainsKey(genericInformations.PlayerActionId))
            {
                return ErrorMessages.GetConfigurationOverriteMessage(genericInformations.PlayerActionId, gameConfiguration.PlayerActionConfiguration.name);
            }
            return null;
        }

        public override string ComputeErrorState(ref Dictionary<string, CreationModuleComponent> editorModules)
        {
            if (this.IsNew && this.CreatedObject.CoolDownTime < 0)
            {
                return "CooldownTime must be >= 0";
            }
            return null;
        }
    }

}
