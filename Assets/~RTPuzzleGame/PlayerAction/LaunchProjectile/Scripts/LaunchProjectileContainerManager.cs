using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchProjectileContainerManager : MonoBehaviour
{
    private List<LaunchProjectile> currentProjectiles = new List<LaunchProjectile>();

    private LaunchProjectileEventManager LaunchProjectileEventManager;

    public void Init()
    {
        LaunchProjectileEventManager = GameObject.FindObjectOfType<LaunchProjectileEventManager>();
    }

    #region External Events
    public void OnLaunchProjectileSpawn(LaunchProjectile launchProjectile)
    {
        currentProjectiles.Add(launchProjectile);
        launchProjectile.transform.parent = transform;
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
