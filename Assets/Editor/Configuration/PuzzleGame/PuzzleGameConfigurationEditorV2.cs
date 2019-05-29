using UnityEngine;
using System.Collections;
using ConfigurationEditor;
using UnityEditor;
using System;

namespace RTPuzzle
{
    public class PuzzleGameConfigurationEditorV2 : ConfigurationEditorWindowV2<PuzzleGameConfigurationEditorProfileV2>
    {
        public PuzzleGameConfigurationEditorV2()
        {
        }

        [MenuItem("Configuration/PuzzleGameConfigurationEditorV2")]
        static void Init()
        {
            var window = EditorWindow.GetWindow<PuzzleGameConfigurationEditorV2>();
            window.Init("t:" + typeof(PuzzleGameConfigurationEditorProfileV2).Name);
            window.Show();
        }

        /*
        public static bool OpenToDesiredConfiguration(Type configurationDataType)
        {
            Init();
            var window = EditorWindow.GetWindow<PuzzleGameConfigurationEditorV2>();
            window.GetConfigurationProfile().Init(() => { window.Repaint(); });
            if (!window.GetConfigurationProfile().SetSelectedKey(configurationDataType))
            {
                window.Close();
                return false;
            }
            return true;
        }
        */

    }

}
