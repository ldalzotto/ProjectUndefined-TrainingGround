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
            Init();
            var window = EditorWindow.GetWindow<CoreGameConfigurationEditor>();
            if (!window.GetConfigurationProfile().SetSelectedKey(configurationDataType))
            {
                window.Close();
                return false;
            }
            return true;
        }
    }

}
