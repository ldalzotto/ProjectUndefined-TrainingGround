using UnityEngine;
using System.Collections;
using UnityEditor;
using ConfigurationEditor;
using RTPuzzle;
using CoreGame;

namespace Editor_ContextMenuActions
{
    public class OpenSpecial
    {
        [MenuItem("Assets/Open Special")]
        private static void OpenConfiguration()
        {
            var selected = Selection.activeObject;
            if (TypeHelper.IsAssignableToGenericType(selected.GetType(), typeof(ConfigurationSerialization<,>)))
            {
                if (!PuzzleGameConfigurationEditorV2.OpenToDesiredConfiguration(selected.GetType()))
                {
                    if (!CoreGameConfigurationEditor.OpenToDesiredConfiguration(selected.GetType()))
                    {
                        Debug.LogError("Configuration not found.");
                    }
                }
            }
        }
    }

}
