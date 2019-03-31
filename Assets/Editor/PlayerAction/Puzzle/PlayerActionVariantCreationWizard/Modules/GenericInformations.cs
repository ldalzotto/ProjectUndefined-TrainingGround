using RTPuzzle;
using UnityEditor;

namespace Editor_PlayerActionVariantCreationWizardEditor
{
    [System.Serializable]
    public class GenericInformations : CreationModuleComponent
    {
        [SearchableEnum]
        public AttractiveObjectId AttractiveObjectId;

        [SearchableEnum]
        public PlayerActionId PlayerActionId;

        [SearchableEnum]
        public LevelZonesID LevelZonesID;

        public GenericInformations(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        protected override string foldoutLabel => "Generic informations : ";

        public override void ResetEditor()
        {
        }

        protected override void OnInspectorGUIImpl()
        {
            Editor.CreateEditor(this).OnInspectorGUI();
        }
    }

}
