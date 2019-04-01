namespace Editor_PlayerActionVariantCreationWizardEditor
{
    public class AddToLevel : CreationModuleComponent
    {

        private GameConfiguration gameConfiguration;
        private GenericInformations genericInformations;


        public void SetDependencies(GameConfiguration gameConfiguration, GenericInformations genericInformations)
        {
            this.gameConfiguration = gameConfiguration;
            this.genericInformations = genericInformations;
        }

        public AddToLevel(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        protected override string foldoutLabel => "Add to level configuration : ";

        public override void ResetEditor()
        {
        }

        protected override void OnInspectorGUIImpl()
        {
        }

    }

}
