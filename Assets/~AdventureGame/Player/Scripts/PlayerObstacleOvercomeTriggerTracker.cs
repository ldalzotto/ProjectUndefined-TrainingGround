using UnityEngine;

namespace AdventureGame
{

    public class PlayerObstacleOvercomeTriggerTracker : MonoBehaviour
    {

        private PlayerManager PlayerManager;

        private void Start()
        {
            PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerManager.ObstacleOvercomeTriggerEnter(other);
        }

    }

}