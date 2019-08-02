using UnityEngine;
using System.Collections;
using RTPuzzle;
using System;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class EditProjectile : EditScriptableObjectModule<LaunchProjectileModule>
    {
        private LaunchProjectileConfiguration projectileConfiguration;
        protected override Func<LaunchProjectileModule, ScriptableObject> scriptableObjectResolver
        {
            get
            {
                return (LaunchProjectileModule LaunchProjectile) =>
                {
                    if (projectileConfiguration != null && projectileConfiguration.ConfigurationInherentData.ContainsKey(LaunchProjectile.LaunchProjectileID))
                    {
                        return projectileConfiguration.ConfigurationInherentData[LaunchProjectile.LaunchProjectileID];
                    }
                    return null;
                };
            }
        }

        public override void OnEnabled()
        {
            this.projectileConfiguration = AssetFinder.SafeSingleAssetFind<LaunchProjectileConfiguration>("t:" + typeof(LaunchProjectileConfiguration));
        }
    }
}