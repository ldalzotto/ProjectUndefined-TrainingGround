using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace AdventureGame
{
    public class PointOfInterestCutsceneController : MonoBehaviour
    {
        #region External Dependencies
        private CutscenePlayerManager CutscenePlayerManager;
        #endregion

        private Rigidbody Rigidbody;
        private NavMeshAgent Agent;
        private POICutsceneMoveManager POICutsceneMoveManager;

        private Vector3 warpPosition;

        #region State 
        private bool askedForWarp;
        public bool IsDirectedByCutscene()
        {
            return this.POICutsceneMoveManager.IsDirectedByAi || this.CutscenePlayerManager.IsCutscenePlaying;
        }
        #endregion

        public void Init()
        {
            this.CutscenePlayerManager = GameObject.FindObjectOfType<CutscenePlayerManager>();

            this.Rigidbody = GetComponentInParent<Rigidbody>();
            this.Agent = GetComponentInParent<NavMeshAgent>();
            this.POICutsceneMoveManager = new POICutsceneMoveManager(this.Rigidbody, this.Agent);
        }

        public float Tick(float d, float SpeedMultiplicationFactor, float AIRotationSpeed)
        {
            return this.POICutsceneMoveManager.Tick(d, SpeedMultiplicationFactor, AIRotationSpeed);
        }

        public void FixedTick(float d)
        {
            if (this.askedForWarp)
            {
                this.Rigidbody.MovePosition(this.warpPosition);
                this.askedForWarp = false;
            }
        }

        public void Warp(Vector3 warpPosition)
        {
            this.askedForWarp = true;
            this.warpPosition = warpPosition;
        }

        public IEnumerator SetAIDestination(Vector3 destination, float normalizedSpeed)
        {
            yield return this.POICutsceneMoveManager.SetDestination(destination, normalizedSpeed);
        }
    }

    class POICutsceneMoveManager
    {
        private Rigidbody PlayerRigidBody;
        private NavMeshAgent playerAgent;

        public POICutsceneMoveManager(Rigidbody playerRigidBody, NavMeshAgent playerAgent)
        {
            PlayerRigidBody = playerRigidBody;
            this.playerAgent = playerAgent;
        }

        private bool isDirectedByAi;
        private float normalizedSpeedMagnitude = 1f;

        public bool IsDirectedByAi { get => isDirectedByAi; }


        public float Tick(float d, float SpeedMultiplicationFactor, float AIRotationSpeed)
        {
            if (this.isDirectedByAi)
            {
                if (playerAgent.velocity.normalized != Vector3.zero)
                {
                    this.PlayerRigidBody.transform.rotation = Quaternion.Slerp(this.PlayerRigidBody.transform.rotation, Quaternion.LookRotation(playerAgent.velocity.normalized), d * AIRotationSpeed);
                    //only rotate on world z axis
                    Vector3 axis = Vector3.up;
                    this.PlayerRigidBody.transform.eulerAngles = new Vector3(this.PlayerRigidBody.transform.eulerAngles.x * axis.x, this.PlayerRigidBody.transform.eulerAngles.y * axis.y, this.PlayerRigidBody.transform.eulerAngles.z * axis.z);
                }

                var playerMovementOrientation = (playerAgent.nextPosition - this.PlayerRigidBody.transform.position).normalized;
                playerAgent.speed = SpeedMultiplicationFactor * this.normalizedSpeedMagnitude;
                PlayerRigidBody.transform.position = playerAgent.nextPosition;
                return this.normalizedSpeedMagnitude;
            }
            else
            {
                return 0f;
            }

        }

        public IEnumerator SetDestination(Vector3 destination, float normalizedSpeed)
        {
            isDirectedByAi = true;
            this.normalizedSpeedMagnitude = normalizedSpeed;
            playerAgent.nextPosition = this.PlayerRigidBody.transform.position;
            playerAgent.SetDestination(destination);
            PlayerRigidBody.isKinematic = true;
            yield return Coroutiner.Instance.StartCoroutine(new WaitForNavAgentDestinationReached(playerAgent));
            isDirectedByAi = false;
            PlayerRigidBody.isKinematic = false;
        }
    }
}
