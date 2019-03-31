using UnityEngine;
using System.Collections;
using RTPuzzle;
using UnityEditor;

namespace Editor_PlayerActionVariantCreationWizardEditor
{
    public class GameConfiguration : CreationModuleComponent
    {

        private PlayerActionConfiguration playerActionConfiguration;

        public GameConfiguration(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        public PlayerActionConfiguration PlayerActionConfiguration { get => playerActionConfiguration; }

        protected override string foldoutLabel => "Game configuration : ";

        public override void ResetEditor()
        {
        }

        protected override void OnInspectorGUIImpl()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(this.playerActionConfiguration, typeof(PlayerActionConfiguration), false);
            EditorGUI.EndDisabledGroup();
            if(this.playerActionConfiguration == null)
            {
                this.playerActionConfiguration = AssetFinder.SafeSingleAssetFind<PlayerActionConfiguration>("t:" + typeof(PlayerActionConfiguration));
            }
        }
    }
}
