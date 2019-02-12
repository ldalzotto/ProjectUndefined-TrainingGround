using UnityEngine;

namespace RTPuzzle
{
    public class PlayerManagerDataRetriever : MonoBehaviour
    {

        private PlayerManager PlayerManager;

        public void Init()
        {
            PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
        }

        #region Data Retrieval
        public float GetPlayerSpeedMagnitude()
        {
            return PlayerManager.GetPlayerSpeedMagnitude();
        }
        public Transform GetPlayerTransform()
        {
            return PlayerManager.transform;
        }
        public Collider GetPlayerCollider()
        {
            return PlayerManager.GetPlayerCollider();
        }
        #endregion
    }

}
