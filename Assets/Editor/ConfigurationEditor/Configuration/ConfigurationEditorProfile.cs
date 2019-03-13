#if UNITY_EDITOR

using OdinSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ConfigurationEditor
{
    [System.Serializable]
    public abstract class ConfigurationEditorProfile : SerializedScriptableObject
    {
        public abstract Dictionary<string, ConfigurationSelectionProfile> ConfigurationSelection { get; }

        public abstract Dictionary<string, IGenericConfigurationEditor> Configurations { get; }

        public void OnSelected(string newSelectedTag)
        {
            this.ConfigurationSelection[newSelectedTag].IsSelected = true;
            this.ConfigurationSelection.Keys.Where(p => p != newSelectedTag).ToList().ForEach((k) => { this.ConfigurationSelection[k].IsSelected = false; });
        }

        public IGenericConfigurationEditor GetSelectedConf()
        {
            var selectedEntry = this.ConfigurationSelection.Where(v => v.Value.IsSelected);
            if (selectedEntry != null && selectedEntry.Count() > 0)
            {
                return this.Configurations[selectedEntry.First().Key];
            }
            return null;

        }

        public static string ComputeSelectionKey(Type enumType)
        {
            return enumType.ToString();
        }

    }

    [System.Serializable]
    public class ConfigurationSelectionProfile
    {

        [SerializeField]
        private bool isSelected;
        [SerializeField] private string tabName;
        private GUIStyle buttonStyle;

        public ConfigurationSelectionProfile(string tabName)
        {
            this.isSelected = false;
            this.tabName = tabName;
        }

        public bool IsSelected { get => isSelected; set => isSelected = value; }
        public GUIStyle ButtonStyle
        {
            get
            {
                if (this.buttonStyle == null)
                {
                    this.buttonStyle = EditorStyles.miniButton;
                }
                return this.buttonStyle;
            }
        }

        public string TabName { get => tabName; }
    }
}
#endif //UNITY_EDITOR