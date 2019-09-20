using CoreGame;
using Editor_GameDesigner;
using GameConfigurationID;
using System;
using UnityEditor;
using UnityEngine;

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

        public static GameDesignerEditor OpenConfigurationEditor(Type targetType)
        {
            return GameDesignerEditor.InitWithSelectedKey(targetType);
        }

        public static void OpenTextRepertoireAtID(string DisucssionSentenceTextId)
        {
            var gameDesignerEditorWindow = GameDesignerEditor.InitWithSelectedKey(typeof(ConfigurationModule<DiscussionTextConfiguration, DiscussionTextID, DiscussionTextInherentData>));
            var discussionConfigurationModule = (ConfigurationModule<DiscussionTextConfiguration, DiscussionTextID, DiscussionTextInherentData>)gameDesignerEditorWindow.GetCrrentGameDesignerModule();
            ((GenericConfigurationEditor<DiscussionTextID, DiscussionTextInherentData>)discussionConfigurationModule.ConfigurationEditor).ProjectilesConf.SetSearchFilter(DisucssionSentenceTextId);
        }
    }

}
