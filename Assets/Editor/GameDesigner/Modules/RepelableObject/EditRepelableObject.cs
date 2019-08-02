using RTPuzzle;
using System;
using UnityEngine;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class EditRepelableObject : EditScriptableObjectModule<ObjectRepelModule>
    {
        protected override Func<ObjectRepelModule, ScriptableObject> scriptableObjectResolver
        {
            get
            {
                return (ObjectRepelModule ObjectRepelType) => this.RepelableObjectsConfiguration.ConfigurationInherentData[ObjectRepelType.ObjectRepelID];
            }
        }

        private ObjectRepelConfiguration RepelableObjectsConfiguration;

        public override void OnEnabled()
        {
            this.RepelableObjectsConfiguration = AssetFinder.SafeSingleAssetFind<ObjectRepelConfiguration>("t:" + typeof(ObjectRepelConfiguration).Name);
        }
    }
}