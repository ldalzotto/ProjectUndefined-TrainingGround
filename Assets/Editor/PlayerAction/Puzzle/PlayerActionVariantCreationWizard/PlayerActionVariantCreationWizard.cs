using UnityEngine.SceneManagement;

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

            genericInformations.OnInspectorGUI(ref this.editorProfile.Modules);
            gameConfiguration.OnInspectorGUI(ref this.editorProfile.Modules);
            attractiveObjectActionInherentDataCreation.OnInspectorGUI(ref this.editorProfile.Modules);
            wheelActionCreation.OnInspectorGUI(ref this.editorProfile.Modules);
            addTolevel.OnInspectorGUI(ref this.editorProfile.Modules);
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

            if (attractiveObjectActionInherentDataCreation.IsNew)
            {
                this.editorProfile.GeneratedObjects.Add(attractiveObjectActionInherentConfigurationDataGenerated);
            }

            var wheelNodeConfiguration = wheelActionCreation.CreateAsset(this.editorProfile.ProjectRelativeTmpFolderPath,
                NamingConventionHelper.BuildName(genericInformations.PlayerActionId.ToString(), genericInformations.LevelZonesID, PrefixType.WHEEL_NODE, SufixType.NONE));

            if (wheelActionCreation.IsNew)
            {
                this.editorProfile.GeneratedObjects.Add(wheelNodeConfiguration);
            }

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



            attractiveObjectActionInherentDataCreation.OnGenerationEnd();

            attractiveObjectActionInherentDataCreation.MoveGeneratedAsset(genericInformations.PathConfiguration.AttractiveObjectPlayerActionConfigurationPath);
            wheelActionCreation.MoveGeneratedAsset(genericInformations.PathConfiguration.WheelActionConfigurationPath);

        }
    }

}