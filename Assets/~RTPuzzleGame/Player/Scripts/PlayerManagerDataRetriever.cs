using UnityEngine;
using UnityEngine.AI;

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
        public Vector3 GetPlayerWorldPosition()
        {
            return this.GetPlayerTransform().position;
        }
        public Collider GetPlayerPuzzleLogicRootCollier()
        {
            return PlayerManager.PlayerPuzzleLogicRootCollier;
        }

        public Animator GetPlayerAnimator()
        {
            return PlayerManager.GetPlayerAnimator();
        }

        public Rigidbody GetPlayerRigidBody()
        {
            return PlayerManager.PlayerRigidbody;
        }
        #endregion
    }

}
