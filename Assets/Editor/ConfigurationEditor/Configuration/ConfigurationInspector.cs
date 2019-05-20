using UnityEngine;
using System.Collections;
using UnityEditor;
using RTPuzzle;
using CoreGame;

namespace ConfigurationEditor
{
    [CustomEditor(typeof(ConfigurationSerialization<,>), true)]
    public class ConfigurationInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("OPEN IN EDITOR"))
            {
                if (!PuzzleGameConfigurationEditorV2.OpenToDesiredConfiguration(this.target.GetType()))
                {
                    if (!CoreGameConfigurationEditor.OpenToDesiredConfiguration(this.target.GetType()))
                    {
                        Debug.LogError("Configuration not found.");
                    }
                }
            }
            base.OnInspectorGUI();
        }
    }

}
