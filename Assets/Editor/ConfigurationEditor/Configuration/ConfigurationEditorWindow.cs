#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace ConfigurationEditor
{

    [System.Serializable]
    public abstract class ConfigurationEditorWindow<T> : EditorWindow where T : ConfigurationEditorProfile
    {
        [SerializeField]
        private ConfigurationSelection ConfigurationSelection;

        [SerializeField] private Vector2 scrollPosition;

        [SerializeField] private static T ConfigurationProfile;

        private Func<T> buildConfigurationProfile;
        private string configurationProfiletAssetFilter;

        private GUIStyle profileRefreshStyle;

        public void Init(string configurationProfiletAssetFilter, Func<T> buildConfigurationProfile)
        {
            this.buildConfigurationProfile = buildConfigurationProfile;
            this.configurationProfiletAssetFilter = configurationProfiletAssetFilter;
            InitializeConfiguration();
        }

        public void OnEnable()
        {
            InitializeConfiguration();
            Undo.undoRedoPerformed += () => { this.Repaint(); };
        }

        private void InitializeConfiguration()
        {
            if (this.ConfigurationSelection == null)
            {
                this.ConfigurationSelection = new ConfigurationSelection();
            }
            if (ConfigurationProfile == null)
            {
                ConfigurationProfile = AssetFinder.SafeSingleAssetFind<T>(configurationProfiletAssetFilter);
                ConfigurationEditorUndoHelper.ConfigurationEditorProfile = ConfigurationProfile;
            }
            if (this.profileRefreshStyle == null)
            {
                this.profileRefreshStyle = new GUIStyle();
                this.profileRefreshStyle.alignment = TextAnchor.MiddleCenter;
            }
        }

        public void OnGUI()
        {
            this.InitializeConfiguration();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(this.profileRefreshStyle, GUILayout.Width(30));
            if (GUILayout.Button(new GUIContent("↺", "Refresh profile"), EditorStyles.miniButton))
            {
                if (ConfigurationEditorUndoHelper.ConfigurationEditorProfile != null)
                {
                    var path = AssetDatabase.GetAssetPath(ConfigurationEditorUndoHelper.ConfigurationEditorProfile);
                    if (AssetDatabase.DeleteAsset(path))
                    {
                        var newProfile = ScriptableObject.CreateInstance<T>();
                        AssetDatabase.CreateAsset(newProfile, path);
                    }

                }
            }
            EditorGUILayout.EndVertical();
            ConfigurationProfile = EditorGUILayout.ObjectField(ConfigurationProfile, typeof(T), false) as T;
            EditorGUILayout.EndHorizontal();
            if (ConfigurationProfile != null)
            {
                ConfigurationEditorUndoHelper.ConfigurationEditorProfile = ConfigurationProfile;
                ConfigurationSelection.GUITick();
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                var selectedConf = ConfigurationEditorUndoHelper.ConfigurationEditorProfile.GetSelectedConf();
                if (selectedConf != null)
                {
                    selectedConf.GUITick();
                }
                EditorGUILayout.EndScrollView();
            }
        }
    }

    [System.Serializable]
    class ConfigurationSelection
    {
        [SerializeField]
        private Vector2 headerSelectionScrollPosition;
        
        public void GUITick()
        {

            EditorGUILayout.Space();

            this.headerSelectionScrollPosition = EditorGUILayout.BeginScrollView(this.headerSelectionScrollPosition);
            EditorGUILayout.BeginHorizontal();
            var configurationsSelections = ConfigurationEditorUndoHelper.ConfigurationEditorProfile.ConfigurationSelection;
            foreach (var configurationSelection in configurationsSelections)
            {
                configurationsSelections[configurationSelection.Key].IsSelected =
                    OnToggleDisableOthers(configurationSelection.Value.TabName, configurationsSelections[configurationSelection.Key].ButtonStyle, configurationsSelections[configurationSelection.Key].IsSelected,
                     () => { ConfigurationEditorUndoHelper.ConfigurationEditorProfile.OnSelected(configurationSelection.Key); });
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
        }

        private bool OnToggleDisableOthers(string text, GUIStyle style, bool currentTab, Action OnActivated)
        {
            EditorGUI.BeginChangeCheck();
            var selected = GUILayout.Toggle(currentTab, text, style);
            if (EditorGUI.EndChangeCheck())
            {
                if (selected)
                {
                    OnActivated.Invoke();
                }
            }
            return selected;
        }
    }

}
#endif