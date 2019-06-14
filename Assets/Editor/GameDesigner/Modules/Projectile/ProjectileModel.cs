using RTPuzzle;
using System;
using UnityEngine;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class ProjectileModel : SetModelModule<LaunchProjectile>
    {
        protected override Func<LaunchProjectile, Transform> FindParent
        {
            get
            {
                return (LaunchProjectile LaunchProjectile) => LaunchProjectile.transform;
            }
        }

        private ProjectileConfiguration ProjectileConfiguration;

        protected override void OnClick(GameObject currentSelectedObj)
        {
            base.OnClick(currentSelectedObj);
            this.ProjectileConfiguration.ConfigurationInherentData[currentSelectedObj.GetComponent<LaunchProjectile>().LaunchProjectileId].SetProjectileModelPrefab(this.ModelObject);
        }

        public override void OnEnabled()
        {
            base.OnEnabled();
            this.ProjectileConfiguration = AssetFinder.SafeSingleAssetFind<ProjectileConfiguration>("t:" + typeof(ProjectileConfiguration).Name);
        }
    }
}