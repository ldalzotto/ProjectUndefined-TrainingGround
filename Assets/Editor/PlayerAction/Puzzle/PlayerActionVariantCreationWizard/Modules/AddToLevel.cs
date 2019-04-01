namespace Editor_PlayerActionVariantCreationWizardEditor
{
    public class AddToLevel : CreationModuleComponent
    {

        public AddToLevel(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        protected override string foldoutLabel => "Add to level configuration : ";

        protected override string headerDescriptionLabel => "Should the player action added to level configuration ?";

        public override void ResetEditor()
        {
        }

        protected override void OnInspectorGUIImpl()
        {
        }

    }

}
