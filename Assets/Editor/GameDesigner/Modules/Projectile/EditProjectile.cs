using UnityEngine;
using System.Collections;
using RTPuzzle;
using System;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class EditProjectile : EditScriptableObjectModule<LaunchProjectile>
    {
        private ProjectileConfiguration projectileConfiguration;
        protected override Func<LaunchProjectile, ScriptableObject> scriptableObjectResolver
        {
            get
            {
                return (LaunchProjectile LaunchProjectile) =>
                {
                    if (projectileConfiguration != null && projectileConfiguration.ConfigurationInherentData.ContainsKey(LaunchProjectile.LaunchProjectileId))
                    {
                        return projectileConfiguration.ConfigurationInherentData[LaunchProjectile.LaunchProjectileId];
                    }
                    return null;
                };
            }
        }

        public override void OnEnabled()
        {
            this.projectileConfiguration = AssetFinder.SafeSingleAssetFind<ProjectileConfiguration>("t:" + typeof(ProjectileConfiguration));
        }
    }
}