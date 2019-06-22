using CoreGame;
using System;
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
        private Animator Animator;
        private POICutsceneMoveManager POICutsceneMoveManager;

        private Transform warpPosition;

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
            this.Animator = transform.parent.GetComponentInChildren<Animator>();
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
                this.Rigidbody.MovePosition(this.warpPosition.position);
                this.Rigidbody.MoveRotation(this.warpPosition.rotation);
                this.askedForWarp = false;
            }
        }

        public void Warp(Transform warpPosition)
        {
            this.askedForWarp = true;
            this.warpPosition = warpPosition;
        }

        public IEnumerator SetAIDestination(Transform destination, float normalizedSpeed)
        {
            yield return this.POICutsceneMoveManager.SetDestination(destination, normalizedSpeed);
            //Force position to ensure the destination is correctly reached
            this.Warp(destination);
        }

        public IEnumerator PlayAnimationAndWait(PlayerAnimatioNamesEnum playerAnimatioNnamesEnum, float crossFadeDuration, Func<IEnumerator> animationEndCallback)
        {
            yield return AnimationPlayerHelper.PlayAndWait(this.Animator, playerAnimatioNnamesEnum, crossFadeDuration, animationEndCallback);
        }

        public void PlayAnimation(PlayerAnimatioNamesEnum playerAnimatioNnamesEnum, float crossFadeDuration)
        {
            AnimationPlayerHelper.Play(this.Animator, playerAnimatioNnamesEnum, crossFadeDuration);
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

        public IEnumerator SetDestination(Transform destination, float normalizedSpeed)
        {
            isDirectedByAi = true;
            this.normalizedSpeedMagnitude = normalizedSpeed;
            playerAgent.nextPosition = this.PlayerRigidBody.transform.position;
            playerAgent.SetDestination(destination.position);
            PlayerRigidBody.isKinematic = true;
            //Let the AI move
            yield return Coroutiner.Instance.StartCoroutine(new WaitForNavAgentDestinationReached(playerAgent));
            PlayerRigidBody.isKinematic = false;
            isDirectedByAi = false;
        }
    }

}
