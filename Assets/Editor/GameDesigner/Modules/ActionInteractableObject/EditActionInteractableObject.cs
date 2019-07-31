using RTPuzzle;
using System;
using UnityEngine;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class EditActionInteractableObject : EditScriptableObjectModule<ActionInteractableObjectModule>
    {
        private ActionInteractableObjectConfiguration ActionInteractableObjectConfiguration;
        protected override Func<ActionInteractableObjectModule, ScriptableObject> scriptableObjectResolver
        {
            get
            {
                return (ActionInteractableObjectModule actionInteractableObjectModule) =>
                {
                    if (ActionInteractableObjectConfiguration != null && ActionInteractableObjectConfiguration.ConfigurationInherentData.ContainsKey(actionInteractableObjectModule.ActionInteractableObjectID))
                    {
                        return ActionInteractableObjectConfiguration.ConfigurationInherentData[actionInteractableObjectModule.ActionInteractableObjectID];
                    }
                    return null;
                };
            }
        }

        public override void OnEnabled()
        {
            this.ActionInteractableObjectConfiguration = AssetFinder.SafeSingleAssetFind<ActionInteractableObjectConfiguration>("t:" + typeof(ActionInteractableObjectConfiguration));
        }
    }
}