#if UNITY_EDITOR

using CoreGame;
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
            Debug.Log("INIT");
            var window = EditorWindow.GetWindow<PuzzleGameConfigurationEditor>();
            window.Show();
        }

        [SerializeField]
        private ConfigurationSelection ConfigurationSelection;

        [SerializeField]
        private GenericConfigurationEditor<LaunchProjectileId, ProjectileInherentData> ProjectileConfiguration;
        [SerializeField]
        private GenericConfigurationEditor<TargetZoneID, TargetZoneInherentData> TargetZoneConfigurationEditor;
        [SerializeField]
        private GenericConfigurationEditor<LevelZonesID, PlayerActionsInherentData> PlayerActionConfigurationEditor;

        [SerializeField] private Vector2 scrollPosition;


        public void OnEnable()
        {
            var data = EditorPrefs.GetString(typeof(PuzzleGameConfigurationEditor).ToString(), JsonUtility.ToJson(this, false));
            JsonUtility.FromJsonOverwrite(data, this);
            if (this.ConfigurationSelection == null)
            {
                this.ConfigurationSelection = new ConfigurationSelection();
            }
            if (this.ProjectileConfiguration == null)
            {
                this.ProjectileConfiguration = new GenericConfigurationEditor<LaunchProjectileId, ProjectileInherentData>("t:ProjectileConfiguration");
            }
            if(this.TargetZoneConfigurationEditor == null)
            {
                this.TargetZoneConfigurationEditor = new GenericConfigurationEditor<TargetZoneID, TargetZoneInherentData>("t:TargetZonesConfiguration");
            }
            if(this.PlayerActionConfigurationEditor == null)
            {
                this.PlayerActionConfigurationEditor = new GenericConfigurationEditor<LevelZonesID, PlayerActionsInherentData>("t:PlayerActionConfiguration");
            }
        }


        public void OnDisable()
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
    class GenericConfigurationEditor<K, DATA> where K : struct, IConvertible where DATA : ScriptableObject
    {
        [SerializeField]
        private DictionarySerialization<K, DATA> LaunchProjectileInherentDataConfiguration;

        [SerializeField]
        private string assetSearchFilter;

        public GenericConfigurationEditor(string assetSearchFilter)
        {
            this.assetSearchFilter = assetSearchFilter;
        }

        #region Projectile dictionary configuration
        [SerializeField]
        private DictionaryEnumGUI<K, DATA> projectilesConf;
        #endregion

        public void GUITick()
        {
            EditorGUI.BeginChangeCheck();
            if (this.projectilesConf == null)
            {
                Debug.Log("new");
                this.projectilesConf = new DictionaryEnumGUI<K, DATA>();
            }

            if (LaunchProjectileInherentDataConfiguration == null)
            {
                LaunchProjectileInherentDataConfiguration = AssetFinder.SafeSingeAssetFind<DictionarySerialization<K, DATA>>(this.assetSearchFilter);
            }

            LaunchProjectileInherentDataConfiguration =
                EditorGUILayout.ObjectField(this.LaunchProjectileInherentDataConfiguration, typeof(DictionarySerialization<K, DATA>), false) as DictionarySerialization<K, DATA>;

            if (LaunchProjectileInherentDataConfiguration != null)
            {
                this.projectilesConf.GUITick(ref this.LaunchProjectileInherentDataConfiguration.LaunchProjectileInherentDatas);
            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(this.LaunchProjectileInherentDataConfiguration);
            }

        }
    }

}

#endif //UNITY_EDITOR