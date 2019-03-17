using UnityEngine;
namespace RTPuzzle
{
    public class GroundCollision : MonoBehaviour
    {

        private LaunchProjectileEventManager LaunchProjectileEventManager;

        public void Init()
        {
            LaunchProjectileEventManager = GameObject.FindObjectOfType<LaunchProjectileEventManager>();
        }
        /*
        private void OnTriggerEnter(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                if (collisionType.IsRTPProjectile)
                {
                    var launchProjectile = collisionType.GetComponent<LaunchProjectile>();
                    if (launchProjectile != null)
                    {
                        LaunchProjectileEventManager.OnProjectileGroundTriggerEnter(launchProjectile);
                    }
                }
            }
        }
        */
    }

}

