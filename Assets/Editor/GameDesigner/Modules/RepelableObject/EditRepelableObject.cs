using RTPuzzle;
using System;
using UnityEngine;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class EditRepelableObject : EditScriptableObjectModule<ObjectRepelTypeModule>
    {
        protected override Func<ObjectRepelTypeModule, ScriptableObject> scriptableObjectResolver
        {
            get
            {
                return (ObjectRepelTypeModule ObjectRepelType) => this.RepelableObjectsConfiguration.ConfigurationInherentData[ObjectRepelType.RepelableObjectID];
            }
        }

        private RepelableObjectsConfiguration RepelableObjectsConfiguration;

        public override void OnEnabled()
        {
            this.RepelableObjectsConfiguration = AssetFinder.SafeSingleAssetFind<RepelableObjectsConfiguration>("t:" + typeof(RepelableObjectsConfiguration).Name);
        }
    }
}