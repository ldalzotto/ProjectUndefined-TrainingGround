using UnityEngine;

public class LaunchProjectileEventManager : MonoBehaviour
{

    private LaunchProjectileContainerManager LaunchProjectileContainerManager;
    private RTP_NPCManager RTP_NPCManager;

    public void Init()
    {
        LaunchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
        RTP_NPCManager = GameObject.FindObjectOfType<RTP_NPCManager>();
    }

    public void OnLaunchProjectileSpawn(LaunchProjectile launchProjectile)
    {
        LaunchProjectileContainerManager.OnLaunchProjectileSpawn(launchProjectile);
    }

    public void OnLaunchProjectileDestroy(LaunchProjectile launchProjectile)
    {
        LaunchProjectileContainerManager.OnLaunchProjectileDestroy(launchProjectile);
        RTP_NPCManager.OnLaunchProjectileDestroyed(launchProjectile);
    }
}
