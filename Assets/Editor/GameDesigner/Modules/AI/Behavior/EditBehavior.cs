using UnityEngine;
using System.Collections;
using RTPuzzle;
using UnityEditor;
using System;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class EditBehavior : EditScriptableObjectModule<AIObjectType>
    {

        private AIComponentsConfiguration aIComponentsConfiguration;
        protected override Func<AIObjectType, ScriptableObject> scriptableObjectResolver
        {
            get
            {
                return (AIObjectType npcAIManager) =>
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