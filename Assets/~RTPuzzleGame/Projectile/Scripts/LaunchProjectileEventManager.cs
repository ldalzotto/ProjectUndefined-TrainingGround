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
            LaunchProjectileContainerManager.OnLaunchProjectileDestroy(launchProjectile);
        }

        public void OnProjectileGroundTriggerEnter(LaunchProjectile launchProjectile)
        {
            LaunchProjectileContainerManager.OnProjectileGroundTriggerEnter(launchProjectile);
        }
    }

}
