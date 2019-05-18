using ConfigurationEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CoreGame
{
    public class CoreGameConfigurationEditor : ConfigurationEditorWindowV2<CoreGameConfigurationEditorProfile>
    {
        public CoreGameConfigurationEditor()
        {
        }

        [MenuItem("Configuration/CoreGameConfigurationEditor")]
        static void Init()
        {
            var window = EditorWindow.GetWindow<CoreGameConfigurationEditor>();
            window.Init("t:" + typeof(CoreGameConfigurationEditorProfile).Name);
            window.Show();
        }

        public static bool OpenToDesiredConfiguration(Type configurationDataType)
        {
            var foundConfiguration = CoreGameConfigurationEditorProfile.GetConfigurationID(configurationDataType);
            if (!string.IsNullOrEmpty(foundConfiguration))
            {
                Init();
                var window = EditorWindow.GetWindow<CoreGameConfigurationEditor>();
                window.GetConfigurationProfile().SetSelectedKey(foundConfiguration);
                return true;
            }
            else
            {
                Debug.LogError("Configuration not found.");
                return false;
            }
        }
    }

}
