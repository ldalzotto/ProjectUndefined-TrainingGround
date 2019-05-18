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

        private PuzzleCreationWizardEditorProfile PlayerActionCreationWizardEditorProfile;

        private void OnGUI()
        {
            this.PlayerActionCreationWizardEditorProfile = EditorGUILayout.ObjectField(this.PlayerActionCreationWizardEditorProfile, typeof(PuzzleCreationWizardEditorProfile), false) as PuzzleCreationWizardEditorProfile;
            if (this.PlayerActionCreationWizardEditorProfile == null)
            {
                this.PlayerActionCreationWizardEditorProfile = AssetFinder.SafeSingleAssetFind<PuzzleCreationWizardEditorProfile>("t:" + typeof(PuzzleCreationWizardEditorProfile).ToString());
            }
            if (this.PlayerActionCreationWizardEditorProfile != null)
            {
                this.PlayerActionCreationWizardEditorProfile.GUITick(() => { Repaint(); });
                ICreationWizardEditor<AbstractCreationWizardEditorProfile> selectedTab = this.PlayerActionCreationWizardEditorProfile.GetSelectedConf();
                if (selectedTab != null)
                {
                    selectedTab.OnGUI();
                }
            }
        }

    }


}