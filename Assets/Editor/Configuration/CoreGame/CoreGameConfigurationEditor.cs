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

        public static bool OpenToDesiredConfiguration<T>(Type configurationDataType) where T : EditorWindow
        {
            Init();
            var window = EditorWindow.GetWindow<T>();
            var castedWindow = (IConfigurationEditorWindowV2)window;
            castedWindow.GetTreePicker().Init(() => { window.Repaint(); });
            if (!castedWindow.GetTreePicker().SetSelectedKey(configurationDataType))
            {
                window.Close();
                return false;
            }
            return true;
        }
    }

}
