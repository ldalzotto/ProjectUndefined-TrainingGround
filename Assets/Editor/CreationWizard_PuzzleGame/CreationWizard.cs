using UnityEditor;

namespace Editor_AttractiveObjectVariantWizardEditor
{
    public class CreationWizard : EditorWindow
    {
        [MenuItem("CreationWizard/CreationWizard")]
        public static void Init()
        {
            var window = EditorWindow.GetWindow<CreationWizard>();
            window.Show();
        }

        private CreationWizardEditorProfile PlayerActionCreationWizardEditorProfile;

        private void OnGUI()
        { 
            this.PlayerActionCreationWizardEditorProfile = EditorGUILayout.ObjectField(this.PlayerActionCreationWizardEditorProfile, typeof(CreationWizardEditorProfile), false) as CreationWizardEditorProfile;
            if (this.PlayerActionCreationWizardEditorProfile == null)
            {
                this.PlayerActionCreationWizardEditorProfile = AssetFinder.SafeSingleAssetFind<CreationWizardEditorProfile>("t:" + typeof(CreationWizardEditorProfile).ToString());
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