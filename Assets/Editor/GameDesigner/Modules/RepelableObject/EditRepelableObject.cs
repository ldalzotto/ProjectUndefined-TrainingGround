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
                return (ObjectRepelModule ObjectRepelType) => this.ObjectRepelConfiguration.ConfigurationInherentData[ObjectRepelType.ObjectRepelID];
            }
        }

        private ObjectRepelConfiguration ObjectRepelConfiguration;

        public override void OnEnabled()
        {
            this.ObjectRepelConfiguration = AssetFinder.SafeSingleAssetFind<ObjectRepelConfiguration>("t:" + typeof(ObjectRepelConfiguration).Name);
        }
    }
}