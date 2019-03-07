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
        private ProjectileConfigurationEditor ProjectileConfiguration;
        [SerializeField]
        private TargetZoneConfigurationEditor TargetZoneConfigurationEditor;
        [SerializeField]
        private PlayerActionConfigurationEditor PlayerActionConfigurationEditor;

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
                this.ProjectileConfiguration = new ProjectileConfigurationEditor();
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
            if (ConfigurationSelection.targetZoneSelected)
            {
                TargetZoneConfigurationEditor.GUITick();
            }
            if (ConfigurationSelection.playerActionSelected)
            {
                PlayerActionConfigurationEditor.GUITick();
            }
            EditorGUILayout.EndScrollView();
        }
    }

    [System.Serializable]
    class ConfigurationSelection
    {
        public bool projectileSelected;
        public bool targetZoneSelected;
        public bool attractiveSelected;
        public bool playerActionSelected;

        private GUIStyle projectileButtonStyle;
        private GUIStyle targetZoneButtonStyle;
        private GUIStyle attractiveButtonStyle;
        private GUIStyle playerActionButtonStyle;

        public void GUITick()
        {
            if (projectileButtonStyle == null)
            {
                projectileButtonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
            }
            if (targetZoneButtonStyle == null)
            {
                targetZoneButtonStyle = new GUIStyle(EditorStyles.miniButtonMid);
            }
            if (attractiveButtonStyle == null)
            {
                attractiveButtonStyle = new GUIStyle(EditorStyles.miniButtonMid);
            }
            if (playerActionButtonStyle == null)
            {
                playerActionButtonStyle = new GUIStyle(EditorStyles.miniButtonRight);
            }

            EditorGUILayout.BeginHorizontal();

            OnToggleDisableOthers(ref projectileSelected, "PROJ", projectileButtonStyle, () => { targetZoneSelected = false; attractiveSelected = false; playerActionSelected = false; });
            OnToggleDisableOthers(ref targetZoneSelected, "TARG", targetZoneButtonStyle, () => { projectileSelected = false; attractiveSelected = false; playerActionSelected = false; });
            OnToggleDisableOthers(ref attractiveSelected, "ATTR", attractiveButtonStyle, () => { targetZoneSelected = false; projectileSelected = false; playerActionSelected = false; });
            OnToggleDisableOthers(ref playerActionSelected, "ACTI", playerActionButtonStyle, () => { targetZoneSelected = false; projectileSelected = false; attractiveSelected = false; });

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
    class ProjectileConfigurationEditor
    {
        [SerializeField]
        private ProjectileConfiguration LaunchProjectileInherentDataConfiguration;

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
                LaunchProjectileInherentDataConfiguration = AssetFinder.SafeSingeAssetFind<ProjectileConfiguration>("t:ProjectileConfiguration");
            }

            LaunchProjectileInherentDataConfiguration =
                EditorGUILayout.ObjectField(this.LaunchProjectileInherentDataConfiguration, typeof(ProjectileConfiguration), false) as ProjectileConfiguration;

            if (LaunchProjectileInherentDataConfiguration != null)
            {
                this.projectilesConf.GUITick(ref this.LaunchProjectileInherentDataConfiguration.LaunchProjectileInherentDatas);
            }

        }
    }

    [System.Serializable]
    class TargetZoneConfigurationEditor
    {
        [SerializeField]
        private TargetZonesConfiguration TargetZonesConfiguration;

        #region Projectile dictionary configuration
        [SerializeField]
        private DictionaryEnumGUI<TargetZoneID, TargetZoneInherentData> targetZonesConf;
        #endregion

        public void GUITick()
        {
            if (this.targetZonesConf == null)
            {
                this.targetZonesConf = new DictionaryEnumGUI<TargetZoneID, TargetZoneInherentData>();
            }

            if (TargetZonesConfiguration == null)
            {
                TargetZonesConfiguration = AssetFinder.SafeSingeAssetFind<TargetZonesConfiguration>("t:TargetZonesConfiguration");
            }

            TargetZonesConfiguration =
                EditorGUILayout.ObjectField(this.TargetZonesConfiguration, typeof(TargetZonesConfiguration), false) as TargetZonesConfiguration;

            if (TargetZonesConfiguration != null)
            {
                this.targetZonesConf.GUITick(ref this.TargetZonesConfiguration.conf);
            }

        }
    }

    [System.Serializable]
    class PlayerActionConfigurationEditor
    {
        [SerializeField]
        private PlayerActionConfiguration PlayerActionConfiguration;

        #region Projectile dictionary configuration
        [SerializeField]
        private DictionaryEnumGUI<LevelZonesID, PlayerActionsInherentData> playerActionsConf;
        #endregion

        public void GUITick()
        {
            if (this.playerActionsConf == null)
            {
                this.playerActionsConf = new DictionaryEnumGUI<LevelZonesID, PlayerActionsInherentData>();
            }

            if (PlayerActionConfiguration == null)
            {
                PlayerActionConfiguration = AssetFinder.SafeSingeAssetFind<PlayerActionConfiguration>("t:PlayerActionConfiguration");
            }

            PlayerActionConfiguration =
                EditorGUILayout.ObjectField(this.PlayerActionConfiguration, typeof(PlayerActionConfiguration), false) as PlayerActionConfiguration;

            if (PlayerActionConfiguration != null)
            {
                this.playerActionsConf.GUITick(ref this.PlayerActionConfiguration.conf);
            }

        }
    }

}

#endif //UNITY_EDITOR