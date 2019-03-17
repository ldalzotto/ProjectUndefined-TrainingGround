using UnityEngine;

namespace RTPuzzle
{
    public class LaunchProjectileEventManager : MonoBehaviour
    {

        private LaunchProjectileContainerManager LaunchProjectileContainerManager;
        private NPCAIManager NpcAiManager;

        public void Init()
        {
            LaunchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            NpcAiManager = GameObject.FindObjectOfType<NPCAIManager>();
        }

        public void OnLaunchProjectileSpawn(LaunchProjectile launchProjectile)
        {
            LaunchProjectileContainerManager.OnLaunchProjectileSpawn(launchProjectile);
        }

        public void OnLaunchProjectileDestroy(LaunchProjectile launchProjectile)
        {
            Debug.Log("Projectile destroyed");
            LaunchProjectileContainerManager.OnLaunchProjectileDestroy(launchProjectile);
        }

        public void OnProjectileGroundTriggerEnter(LaunchProjectile launchProjectile)
        {
            Debug.Log("Projectile trigger enter");
            LaunchProjectileContainerManager.OnProjectileGroundTriggerEnter(launchProjectile);
        }
    }

}
