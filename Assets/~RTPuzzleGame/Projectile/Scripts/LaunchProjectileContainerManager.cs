using System.Collections;
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
            launchProjectile.transform.parent = transform;
        }

        internal void OnProjectileGroundTriggerEnter(LaunchProjectile launchProjectile)
        {
            launchProjectile.OnGroundTriggerEnter();
            StartCoroutine(DestroyLaunchProjectileAtEndOfFixedUpdate(launchProjectile));
        }
        #endregion

        private IEnumerator DestroyLaunchProjectileAtEndOfFixedUpdate(LaunchProjectile launchProjectile)
        {
            yield return new WaitForFixedUpdate();
            LaunchProjectileEventManager.OnLaunchProjectileDestroy(launchProjectile);
        }

        internal void OnLaunchProjectileDestroy(LaunchProjectile launchProjectile)
        {
            currentProjectiles.Remove(launchProjectile);
            Destroy(launchProjectile.gameObject);
        }
    }

}
