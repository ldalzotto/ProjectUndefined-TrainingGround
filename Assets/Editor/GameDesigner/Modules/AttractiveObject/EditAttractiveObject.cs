using RTPuzzle;
using System;
using UnityEngine;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class EditAttractiveObject : EditScriptableObjectModule<AttractiveObjectModule>
    {
        private AttractiveObjectConfiguration AttractiveObjectConfiguration;
        protected override Func<AttractiveObjectModule, ScriptableObject> scriptableObjectResolver
        {
            get
            {
                return (AttractiveObjectModule attractiveObjectType) =>
                {
                    if (AttractiveObjectConfiguration != null && AttractiveObjectConfiguration.ConfigurationInherentData.ContainsKey(attractiveObjectType.AttractiveObjectId))
                    {
                        return AttractiveObjectConfiguration.ConfigurationInherentData[attractiveObjectType.AttractiveObjectId];
                    }
                    return null;
                };
            }
        }

        public override void OnEnabled()
        {
            this.AttractiveObjectConfiguration = AssetFinder.SafeSingleAssetFind<AttractiveObjectConfiguration>("t:" + typeof(AttractiveObjectConfiguration));
        }
    }
}