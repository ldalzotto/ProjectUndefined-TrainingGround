using UnityEngine;
using System.Collections;
using UnityEditor;
using RTPuzzle;
using CoreGame;
using System;
using Editor_GameDesigner;
using GameConfigurationID;
using AdventureGame;

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

        public static void OpenTextRepertoireAtID(string DisucssionSentenceTextId)
        {
            DiscussionTextRepertoire.EditorTemporaryChosenId = DisucssionSentenceTextId;
            GameDesignerEditor.InitWithSelectedKey(typeof(DiscussionRepertoireConfigurationModule));
        }
    }

}
