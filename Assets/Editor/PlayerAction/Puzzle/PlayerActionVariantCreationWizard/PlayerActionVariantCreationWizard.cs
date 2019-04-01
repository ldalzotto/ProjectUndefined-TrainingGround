﻿using UnityEngine.SceneManagement;

namespace Editor_PlayerActionVariantCreationWizardEditor
{
    public class PlayerActionVariantCreationWizard : AbstractCreationWizardEditor<PlayerActionVariantCreationWizardEditorProfile>
    {
        protected override void OnWizardGUI()
        {
            var attractiveObjectActionInherentDataCreation = (AttractiveObjectActionInherentDataCreation)this.editorProfile.Modules[typeof(AttractiveObjectActionInherentDataCreation).Name];
            var genericInformations = (GenericInformations)this.editorProfile.Modules[typeof(GenericInformations).Name];
            var gameConfiguration = (GameConfiguration)this.editorProfile.Modules[typeof(GameConfiguration).Name];
            var addTolevel = (AddToLevel)this.editorProfile.Modules[typeof(AddToLevel).Name];
            var wheelActionCreation = (WheelActionCreation)this.editorProfile.Modules[typeof(WheelActionCreation).Name];

            attractiveObjectActionInherentDataCreation.SetDependencies(gameConfiguration, genericInformations);
            addTolevel.SetDependencies(gameConfiguration, genericInformations);
            wheelActionCreation.SetDependencies(gameConfiguration, genericInformations);

            genericInformations.OnInspectorGUI();
            gameConfiguration.OnInspectorGUI();
            attractiveObjectActionInherentDataCreation.OnInspectorGUI();
            wheelActionCreation.OnInspectorGUI();
            addTolevel.OnInspectorGUI();
        }

        protected override void OnGenerationClicked(Scene tmpScene)
        {
            var attractiveObjectActionInherentDataCreation = (AttractiveObjectActionInherentDataCreation)this.editorProfile.Modules[typeof(AttractiveObjectActionInherentDataCreation).Name];
            var genericInformations = (GenericInformations)this.editorProfile.Modules[typeof(GenericInformations).Name];
            var gameConfiguration = (GameConfiguration)this.editorProfile.Modules[typeof(GameConfiguration).Name];
            var addTolevel = (AddToLevel)this.editorProfile.Modules[typeof(AddToLevel).Name];
            var wheelActionCreation = (WheelActionCreation)this.editorProfile.Modules[typeof(WheelActionCreation).Name];

            var attractiveObjectActionInherentConfigurationDataGenerated = attractiveObjectActionInherentDataCreation.CreateAsset(this.editorProfile.ProjectRelativeTmpFolderPath,
                NamingConventionHelper.BuildName("AttractiveObject", genericInformations.LevelZonesID, PrefixType.PLAYER_ACTION, SufixType.NONE));

            var wheelNodeConfiguration = wheelActionCreation.CreateAsset(this.editorProfile.ProjectRelativeTmpFolderPath,
                NamingConventionHelper.BuildName("", PrefixType.WHEEL_NODE, SufixType.NONE));

            attractiveObjectActionInherentConfigurationDataGenerated.ActionWheelNodeConfigurationId = genericInformations.SelectionWheelNodeConfigurationId;

            gameConfiguration.PlayerActionConfiguration.SetEntry(genericInformations.PlayerActionId, attractiveObjectActionInherentConfigurationDataGenerated);

            if (wheelActionCreation.IsNew)
            {
                gameConfiguration.SelectionWheelNodeConfiguration.SetEntry(genericInformations.SelectionWheelNodeConfigurationId, wheelNodeConfiguration);
            }

            if (addTolevel.ModuleEnabled)
            {
                var levelConfigurationData = gameConfiguration.LevelConfiguration.ConfigurationInherentData[genericInformations.LevelZonesID];
                levelConfigurationData.AddPlayerActionId(new RTPuzzle.PlayerActionIdWrapper(genericInformations.PlayerActionId));
            }

            this.editorProfile.GeneratedObjects.Add(attractiveObjectActionInherentConfigurationDataGenerated);

            attractiveObjectActionInherentDataCreation.OnGenerationEnd();
        }
    }

}