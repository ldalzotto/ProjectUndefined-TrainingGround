using UnityEngine;

namespace RTPuzzle
{
    public class LaunchProjectileEventManager : MonoBehaviour
    {
        public void Init()
        {
        }
  
        public void OnProjectileGroundTriggerEnter(LaunchProjectile launchProjectile)
        {
            Debug.Log(MyLog.Format("Projectile trigger enter"));
            launchProjectile.OnGroundTriggerEnter();
        }
    }

}
