using UnityEngine;
using System.Collections;
using RTPuzzle;
using UnityEditor;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class EditBehavior : IGameDesignerModule
    {

        private AIComponentsConfiguration aIComponentsConfiguration;
        private Editor aiComponentsConfigurationEditor;
        private GameObject lastFrameObj;
        public void GUITick()
        {
            this.OnEnabled();
            var currentSelectedObj = GameDesignerHelper.GetCurrentSceneSelectedObject();
            if (currentSelectedObj != null)
            {
                var npcAIManager = currentSelectedObj.GetComponent<NPCAIManager>();
                if (currentSelectedObj == null || npcAIManager == null)
                {
                    this.aiComponentsConfigurationEditor = null;
                }
                else
                {
                    if (currentSelectedObj != this.lastFrameObj && npcAIManager != null)
                    {
                        CreateEditor(npcAIManager);
                    }
                }

                if (GUILayout.Button("REFRESH", GUILayout.Width(50f)))
                {
                    CreateEditor(npcAIManager);
                }

                if (this.aiComponentsConfigurationEditor != null)
                {
                    this.aiComponentsConfigurationEditor.OnInspectorGUI();
                }
            }
            this.lastFrameObj = currentSelectedObj;
        }

        private void CreateEditor(NPCAIManager npcAIManager)
        {
            if (aIComponentsConfiguration != null)
            {
                if (aIComponentsConfiguration.ConfigurationInherentData.ContainsKey(npcAIManager.AiID) && aIComponentsConfiguration.ConfigurationInherentData[npcAIManager.AiID] != null)
                {
                    this.aiComponentsConfigurationEditor = Editor.CreateEditor(aIComponentsConfiguration.ConfigurationInherentData[npcAIManager.AiID]);
                }
            }
        }

        public void OnDisabled()
        {
        }

        public void OnEnabled()
        {
            if (this.aIComponentsConfiguration == null)
            {
                this.aIComponentsConfiguration = AssetFinder.SafeSingleAssetFind<AIComponentsConfiguration>("t:" + typeof(AIComponentsConfiguration));
            }
            
        }
    }
}