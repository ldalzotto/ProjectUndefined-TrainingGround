using UnityEngine;
using System.Collections;
using UnityEditor;
using RTPuzzle;
using CoreGame;
using System;
using Editor_GameDesigner;

namespace ConfigurationEditor
{
    [CustomEditor(typeof(ConfigurationSerialization<,>), true)]
    public class ConfigurationInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("OPEN IN EDITOR"))
            {
                OpenConfigurationEditor(this.target.GetType());
            }
            base.OnInspectorGUI();
        }

        public static void OpenConfigurationEditor(Type targetType)
        {
            GameDesignerEditor.InitWithSelectedKey(targetType);
        }
    }

}
