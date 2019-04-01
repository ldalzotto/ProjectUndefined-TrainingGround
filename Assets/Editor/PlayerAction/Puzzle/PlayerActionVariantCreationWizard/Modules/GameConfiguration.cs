using RTPuzzle;
using UnityEditor;

namespace Editor_PlayerActionVariantCreationWizardEditor
{
    public class GameConfiguration : CreationModuleComponent
    {

        private PlayerActionConfiguration playerActionConfiguration;
        private LevelConfiguration levelConfiguration;
        private SelectionWheelNodeConfiguration selectionWheelNodeConfiguration;

        public GameConfiguration(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        public PlayerActionConfiguration PlayerActionConfiguration { get => playerActionConfiguration; }
        public LevelConfiguration LevelConfiguration { get => levelConfiguration; }
        public SelectionWheelNodeConfiguration SelectionWheelNodeConfiguration { get => selectionWheelNodeConfiguration; }

        protected override string foldoutLabel => "Game configuration : ";

        public override void ResetEditor()
        {
        }

        protected override void OnInspectorGUIImpl()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(this.playerActionConfiguration, typeof(PlayerActionConfiguration), false);
            EditorGUILayout.ObjectField(this.levelConfiguration, typeof(LevelConfiguration), false);
            EditorGUILayout.ObjectField(this.selectionWheelNodeConfiguration, typeof(SelectionWheelNodeConfiguration), false);
            EditorGUI.EndDisabledGroup();
            if (this.playerActionConfiguration == null)
            {
                this.playerActionConfiguration = AssetFinder.SafeSingleAssetFind<PlayerActionConfiguration>("t:" + typeof(PlayerActionConfiguration));
            }
            if (this.levelConfiguration == null) { this.levelConfiguration = AssetFinder.SafeSingleAssetFind<LevelConfiguration>("t:" + typeof(LevelConfiguration)); }
            if (this.selectionWheelNodeConfiguration == null) { this.selectionWheelNodeConfiguration = AssetFinder.SafeSingleAssetFind<SelectionWheelNodeConfiguration>("t:" + typeof(SelectionWheelNodeConfiguration)); }
        }
    }
}
