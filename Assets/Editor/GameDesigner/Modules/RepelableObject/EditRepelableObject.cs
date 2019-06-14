using RTPuzzle;
using System;
using UnityEngine;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class EditRepelableObject : EditScriptableObjectModule<ObjectRepelType>
    {
        protected override Func<ObjectRepelType, ScriptableObject> scriptableObjectResolver
        {
            get
            {
                return (ObjectRepelType ObjectRepelType) => this.RepelableObjectsConfiguration.ConfigurationInherentData[ObjectRepelType.RepelableObjectID];
            }
        }

        private RepelableObjectsConfiguration RepelableObjectsConfiguration;

        public override void OnEnabled()
        {
            this.RepelableObjectsConfiguration = AssetFinder.SafeSingleAssetFind<RepelableObjectsConfiguration>("t:" + typeof(RepelableObjectsConfiguration).Name);
        }
    }
}