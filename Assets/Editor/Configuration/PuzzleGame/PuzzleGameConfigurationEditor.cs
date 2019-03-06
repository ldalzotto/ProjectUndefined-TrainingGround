#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace RTPuzzle
{
    public class PuzzleGameConfigurationEditor : EditorWindow
    {
        [MenuItem("Configuration/PuzzleGame/GlobalPuzzleGameConfiguration")]
        static void Init()
        {
            var window = EditorWindow.GetWindow<PuzzleGameConfigurationEditor>();
            window.Show();
        }

        [SerializeField]
        private ConfigurationSelection ConfigurationSelection;

        [SerializeField]
        private ProjectileConfiguration ProjectileConfiguration;

        [SerializeField] private Vector2 scrollPosition;

        private void OnEnable()
        {
            var data = EditorPrefs.GetString(typeof(PuzzleGameConfigurationEditor).ToString(), JsonUtility.ToJson(this, false));
            JsonUtility.FromJsonOverwrite(data, this);
            if (this.ConfigurationSelection == null)
            {
                this.ConfigurationSelection = new ConfigurationSelection();
            }
            if (this.ProjectileConfiguration == null)
            {
                this.ProjectileConfiguration = new ProjectileConfiguration();
            }
        }


        private void OnDisable()
        {
            var data = JsonUtility.ToJson(this, false);
            EditorPrefs.SetString(typeof(PuzzleGameConfigurationEditor).ToString(), data);
        }

        private void OnGUI()
        {
            ConfigurationSelection.GUITick();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            if (ConfigurationSelection.projectileSelected)
            {
                ProjectileConfiguration.GUITick();
            }
            EditorGUILayout.EndScrollView();
        }
    }

    [System.Serializable]
    class ConfigurationSelection
    {
        public bool projectileSelected;
        public bool targetZoneSelected;

        private GUIStyle projectileButtonStyle;
        private GUIStyle targetZoneButtonStyle;

        public void GUITick()
        {
            if (projectileButtonStyle == null)
            {
                projectileButtonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
            }
            if (targetZoneButtonStyle == null)
            {
                targetZoneButtonStyle = new GUIStyle(EditorStyles.miniButtonRight);
            }

            EditorGUILayout.BeginHorizontal();

            OnToggleDisableOthers(ref projectileSelected, "PRO", projectileButtonStyle, () => { targetZoneSelected = false; });
            OnToggleDisableOthers(ref targetZoneSelected, "TAR", targetZoneButtonStyle, () => { projectileSelected = false; });

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
        }

        private void OnToggleDisableOthers(ref bool value, string text, GUIStyle style, Action OnActivated)
        {
            EditorGUI.BeginChangeCheck();
            value = GUILayout.Toggle(value, text, style);
            if (EditorGUI.EndChangeCheck())
            {
                if (value)
                {
                    OnActivated.Invoke();
                }
            }
        }

    }

    [System.Serializable]
    class ProjectileConfiguration
    {

        [SerializeField]
        private LaunchProjectileInherentDataConfiguration LaunchProjectileInherentDataConfiguration;

        #region Projectile dictionary configuration
        [SerializeField]
        private DictionaryEnumGUI<LaunchProjectileId, ProjectileInherentData> projectilesConf;
        #endregion

        public void GUITick()
        {
            if (this.projectilesConf == null)
            {
                Debug.Log("new");
                this.projectilesConf = new DictionaryEnumGUI<LaunchProjectileId, ProjectileInherentData>();
            }

            if (LaunchProjectileInherentDataConfiguration == null)
            {
                LaunchProjectileInherentDataConfiguration = AssetFinder.SafeSingeAssetFind<LaunchProjectileInherentDataConfiguration>("t:LaunchProjectileInherentDataConfiguration");
            }

            LaunchProjectileInherentDataConfiguration =
                EditorGUILayout.ObjectField(this.LaunchProjectileInherentDataConfiguration, typeof(LaunchProjectileInherentDataConfiguration), false) as LaunchProjectileInherentDataConfiguration;

            if (LaunchProjectileInherentDataConfiguration != null)
            {
                this.projectilesConf.GUITick(ref this.LaunchProjectileInherentDataConfiguration.LaunchProjectileInherentDatas);
            }

        }
    }

}

#endif //UNITY_EDITOR