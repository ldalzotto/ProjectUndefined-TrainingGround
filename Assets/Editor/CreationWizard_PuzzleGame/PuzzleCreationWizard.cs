using UnityEditor;

namespace Editor_PuzzleGameCreationWizard
{
    public class PuzzleCreationWizard : EditorWindow
    {
        [MenuItem("CreationWizard/PuzzleCreationWizard")]
        public static void Init()
        {
            var window = EditorWindow.GetWindow<PuzzleCreationWizard>();
            window.Show();
        }

        public static void InitWithSelected(string key)
        {
            var window = EditorWindow.GetWindow<PuzzleCreationWizard>();
            window.Show();
            if (window.playerActionCreationWizardEditorProfile == null)
            {
                window.playerActionCreationWizardEditorProfile = AssetFinder.SafeSingleAssetFind<PuzzleCreationWizardEditorProfile>("t:" + typeof(PuzzleCreationWizardEditorProfile).ToString());
                if (window.playerActionCreationWizardEditorProfile != null)
                {
                    window.playerActionCreationWizardEditorProfile.SetSelectedKey(key);
                }
            }
        }

        private PuzzleCreationWizardEditorProfile playerActionCreationWizardEditorProfile;
        
        private void OnGUI()
        {
            this.playerActionCreationWizardEditorProfile = EditorGUILayout.ObjectField(this.playerActionCreationWizardEditorProfile, typeof(PuzzleCreationWizardEditorProfile), false) as PuzzleCreationWizardEditorProfile;
            if (this.playerActionCreationWizardEditorProfile == null)
            {
                this.playerActionCreationWizardEditorProfile = AssetFinder.SafeSingleAssetFind<PuzzleCreationWizardEditorProfile>("t:" + typeof(PuzzleCreationWizardEditorProfile).ToString());
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