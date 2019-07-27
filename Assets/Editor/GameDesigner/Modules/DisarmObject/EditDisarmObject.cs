using RTPuzzle;
using System;
using UnityEngine;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class EditDisarmObject : EditScriptableObjectModule<DisarmObjectModule>
    {
        private DisarmObjectConfiguration DisarmObjectConfiguration;
        protected override Func<DisarmObjectModule, ScriptableObject> scriptableObjectResolver
        {
            get
            {
                return (DisarmObjectModule disarmObjectModule) =>
                {
                    if (DisarmObjectConfiguration != null && DisarmObjectConfiguration.ConfigurationInherentData.ContainsKey(disarmObjectModule.DisarmObjectID))
                    {
                        return DisarmObjectConfiguration.ConfigurationInherentData[disarmObjectModule.DisarmObjectID];
                    }
                    return null;
                };
            }
        }

        public override void OnEnabled()
        {
            this.DisarmObjectConfiguration = AssetFinder.SafeSingleAssetFind<DisarmObjectConfiguration>("t:" + typeof(DisarmObjectConfiguration));
        }
    }
}