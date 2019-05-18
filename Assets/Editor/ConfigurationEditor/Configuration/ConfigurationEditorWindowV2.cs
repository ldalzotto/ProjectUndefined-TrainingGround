#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace ConfigurationEditor
{

    [System.Serializable]
    public abstract class ConfigurationEditorWindowV2<T> : EditorWindow where T : TreeChoiceHeaderTab<IGenericConfigurationEditor>
    {

        [SerializeField] private Vector2 scrollPosition;

        [SerializeField] private static T ConfigurationProfile;

        private string configurationProfiletAssetFilter;

        public void Init(string configurationProfiletAssetFilter)
        {
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
                ConfigurationEditorUndoHelper.ConfigurationEditorProfileV2 = ConfigurationProfile;
            }
        }

        public void OnGUI()
        {
            this.InitializeConfiguration();
            EditorGUILayout.BeginHorizontal();
            ConfigurationProfile = EditorGUILayout.ObjectField(ConfigurationProfile, typeof(T), false) as T;
            EditorGUILayout.EndHorizontal();
            if (ConfigurationProfile != null)
            {
                ConfigurationEditorUndoHelper.ConfigurationEditorProfileV2 = ConfigurationProfile;
                ConfigurationEditorUndoHelper.ConfigurationEditorProfileV2.GUITick(() => { Repaint(); });
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                var selectedConf = ConfigurationEditorUndoHelper.ConfigurationEditorProfileV2.GetSelectedConf();
                if (selectedConf != null)
                {
                    selectedConf.GUITick();
                }
                EditorGUILayout.EndScrollView();
            }
        }

        public T GetConfigurationProfile()
        {
            return ConfigurationProfile;
        }
    }
}
#endif