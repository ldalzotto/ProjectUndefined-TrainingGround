using RTPuzzle;
using System;
using UnityEngine;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class EditAttractiveObject : EditScriptableObjectModule<AttractiveObjectType>
    {
        private AttractiveObjectConfiguration AttractiveObjectConfiguration;
        protected override Func<AttractiveObjectType, ScriptableObject> scriptableObjectResolver
        {
            get
            {
                return (AttractiveObjectType attractiveObjectType) =>
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