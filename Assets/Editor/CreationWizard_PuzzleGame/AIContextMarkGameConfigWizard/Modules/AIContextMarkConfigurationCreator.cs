using UnityEngine;
using System.Collections;
using UnityEditor;
using RTPuzzle;
using System.Collections.Generic;
using CreationWizard;

namespace Editor_AIContextMarkGameConfigWizard
{
    public class AIContextMarkConfigurationCreator : CreateableScriptableObjectComponent<ContextMarkVisualFeedbackInherentData>
    {
        public AIContextMarkConfigurationCreator(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        protected override string objectFieldLabel => "Context mak visual feedback configuration data : ";


        public override string ComputeWarningState(ref Dictionary<string, CreationModuleComponent> editorModules)
        {
            var genericInformations = (GenericInformations)editorModules[typeof(GenericInformations).Name];
            var gameConfiguration = (GameConfiguration)editorModules[typeof(GameConfiguration).Name];
            var selectedId = genericInformations.AiID;
            if (gameConfiguration!=null && gameConfiguration.ContextMarkVisualFeedbackConfiguration!=null &&
                gameConfiguration.ContextMarkVisualFeedbackConfiguration.ConfigurationInherentData != null &&
                gameConfiguration.ContextMarkVisualFeedbackConfiguration.ConfigurationInherentData.ContainsKey(selectedId))
            {
                return ErrorMessages.GetConfigurationOverriteMessage(selectedId, gameConfiguration.ContextMarkVisualFeedbackConfiguration.name);
            }

            return base.ComputeWarningState(ref editorModules);
        }
    }

}
