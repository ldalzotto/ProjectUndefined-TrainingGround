#if UNITY_EDITOR

using CoreGame;
using System;
using UnityEditor;
using UnityEngine;
using ConfigurationEditor;

namespace RTPuzzle
{
    [Serializable]
    public class PuzzleGameConfigurationEditor : ConfigurationEditorWindow<GameConfigurationEditorProfile>
    {
        [MenuItem("Configuration/PuzzleGameConfigurationEditor")]
        static void Init()
        {
            var window = EditorWindow.GetWindow<PuzzleGameConfigurationEditor>();
            window.Init("t:GameConfigurationEditorProfile", () => new GameConfigurationEditorProfile());
            window.Show();
        }
    }

}

#endif //UNITY_EDITOR