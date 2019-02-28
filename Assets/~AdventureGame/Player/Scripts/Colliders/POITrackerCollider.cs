using UnityEngine;

namespace AdventureGame
{

    public class POITrackerCollider : MonoBehaviour
    {
        private PlayerManager PlayerManager;

        private void Start()
        {
            this.PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
        }


        private void OnTriggerEnter(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                PlayerManager.TriggerEnter(other, collisionType);
            }
            else
            {
                // Debug.LogWarning("The collider : " + other.name + " has no CollisionType.");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                PlayerManager.TriggerExit(other, collisionType);
            }
            else
            {
                //   Debug.LogWarning("The collider : " + other.name + " has no CollisionType.");
            }
        }

    }

}