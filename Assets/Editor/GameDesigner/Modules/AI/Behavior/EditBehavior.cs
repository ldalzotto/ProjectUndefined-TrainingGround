using UnityEngine;
using System.Collections;
using RTPuzzle;
using UnityEditor;
using System;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class EditBehavior : EditScriptableObjectModule<NPCAIManager>
    {

        private AIComponentsConfiguration aIComponentsConfiguration;
        protected override Func<NPCAIManager, ScriptableObject> scriptableObjectResolver
        {
            get
            {
                return (NPCAIManager npcAIManager) =>
                {
                    if (aIComponentsConfiguration != null)
                    {
                        if (aIComponentsConfiguration.ConfigurationInherentData.ContainsKey(npcAIManager.AiID) && aIComponentsConfiguration.ConfigurationInherentData[npcAIManager.AiID] != null)
                        {
                            return aIComponentsConfiguration.ConfigurationInherentData[npcAIManager.AiID];
                        }
                    }
                    return null;
                };
            }
        }

        public override void OnEnabled()
        {
            if (this.aIComponentsConfiguration == null)
            {
                this.aIComponentsConfiguration = AssetFinder.SafeSingleAssetFind<AIComponentsConfiguration>("t:" + typeof(AIComponentsConfiguration));
            }
        }
    }
}