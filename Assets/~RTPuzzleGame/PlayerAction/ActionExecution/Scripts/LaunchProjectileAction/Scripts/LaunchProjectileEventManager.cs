using UnityEngine;

namespace RTPuzzle
{
    public class LaunchProjectileEventManager : MonoBehaviour
    {

        private LaunchProjectileContainerManager LaunchProjectileContainerManager;
        private NpcAiManager NpcAiManager;

        public void Init()
        {
            LaunchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            NpcAiManager = GameObject.FindObjectOfType<NpcAiManager>();
        }

        public void OnLaunchProjectileSpawn(LaunchProjectile launchProjectile)
        {
            LaunchProjectileContainerManager.OnLaunchProjectileSpawn(launchProjectile);
        }

        public void OnLaunchProjectileDestroy(LaunchProjectile launchProjectile)
        {
            LaunchProjectileContainerManager.OnLaunchProjectileDestroy(launchProjectile);
            NpcAiManager.OnLaunchProjectileDestroyed(launchProjectile);
        }
    }

}
