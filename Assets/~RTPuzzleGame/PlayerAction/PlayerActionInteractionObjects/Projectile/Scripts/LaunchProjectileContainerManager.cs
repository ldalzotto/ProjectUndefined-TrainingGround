using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class LaunchProjectileContainerManager : MonoBehaviour
    {
        private List<LaunchProjectile> currentProjectiles = new List<LaunchProjectile>();

        private LaunchProjectileEventManager LaunchProjectileEventManager;

        public void Init()
        {
            LaunchProjectileEventManager = GameObject.FindObjectOfType<LaunchProjectileEventManager>();
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
            foreach (var launchProjectile in currentProjectiles)
            {
                launchProjectile.Tick(d, timeAttenuationFactor);
            }
        }

        #region External Events
        public void OnLaunchProjectileSpawn(LaunchProjectile launchProjectile)
        {
            currentProjectiles.Add(launchProjectile);
        }

        internal void OnProjectileGroundTriggerEnter(LaunchProjectile launchProjectile)
        {
            launchProjectile.OnGroundTriggerEnter();
            LaunchProjectileEventManager.OnLaunchProjectileDestroy(launchProjectile);
        }

        internal void OnLaunchProjectileDestroy(LaunchProjectile launchProjectile)
        {
            currentProjectiles.Remove(launchProjectile);
            Destroy(launchProjectile.gameObject);
        }
        #endregion

    }

}
