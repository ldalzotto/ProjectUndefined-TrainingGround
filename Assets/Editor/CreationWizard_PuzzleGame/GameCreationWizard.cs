using UnityEditor;

namespace Editor_MainGameCreationWizard
{
    public class GameCreationWizard : EditorWindow
    {
        [MenuItem("CreationWizard/GameCreationWizard")]
        public static void Init()
        {
            var window = EditorWindow.GetWindow<GameCreationWizard>();
            window.Show();
        }

        public static void InitWithSelected(string key)
        {
            var window = EditorWindow.GetWindow<GameCreationWizard>();
            window.Show();
            if (window.playerActionCreationWizardEditorProfile == null)
            {
                window.playerActionCreationWizardEditorProfile = AssetFinder.SafeSingleAssetFind<GameCreationWizardEditorProfile>("t:" + typeof(GameCreationWizardEditorProfile).ToString());
                if (window.playerActionCreationWizardEditorProfile != null)
                {
                    window.playerActionCreationWizardEditorProfile.Init(() => { window.Repaint(); });
                    window.playerActionCreationWizardEditorProfile.SetSelectedKey(key);
                }
            }
        }

        private GameCreationWizardEditorProfile playerActionCreationWizardEditorProfile;
        
        private void OnGUI()
        {
            this.playerActionCreationWizardEditorProfile = EditorGUILayout.ObjectField(this.playerActionCreationWizardEditorProfile, typeof(GameCreationWizardEditorProfile), false) as GameCreationWizardEditorProfile;
            if (this.playerActionCreationWizardEditorProfile == null)
            {
                this.playerActionCreationWizardEditorProfile = AssetFinder.SafeSingleAssetFind<GameCreationWizardEditorProfile>("t:" + typeof(GameCreationWizardEditorProfile).ToString());
            }
            if (this.playerActionCreationWizardEditorProfile != null)
            {
                this.playerActionCreationWizardEditorProfile.GUITick(() => { Repaint(); });
                ICreationWizardEditor<AbstractCreationWizardEditorProfile> selectedTab = this.playerActionCreationWizardEditorProfile.GetSelectedConf();
                if (selectedTab != null)
                {
                    selectedTab.OnGUI();
                }
            }
        }

    }


}