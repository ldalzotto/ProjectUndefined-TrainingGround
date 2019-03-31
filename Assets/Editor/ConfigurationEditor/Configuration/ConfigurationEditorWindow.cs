#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace ConfigurationEditor
{

    [System.Serializable]
    public abstract class ConfigurationEditorWindow<T> : EditorWindow where T : MultipleChoiceHeaderTab<IGenericConfigurationEditor>
    {

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
                ConfigurationEditorUndoHelper.ConfigurationEditorProfile.GUITick();
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
}
#endif