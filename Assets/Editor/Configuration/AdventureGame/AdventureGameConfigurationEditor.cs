using ConfigurationEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CoreGame
{
    public class AdventureGameConfigurationEditor : ConfigurationEditorWindowV2<AdventureGameConfigurationEditorProfile>
    {
        public AdventureGameConfigurationEditor() { }

        [MenuItem("Configuration/AdventureGameConfigurationEditor")]
        static void Init()
        {
            var window = EditorWindow.GetWindow<AdventureGameConfigurationEditor>();
            window.Init("t:" + typeof(AdventureGameConfigurationEditor).Name);
            window.Show();
        }

    }
}

