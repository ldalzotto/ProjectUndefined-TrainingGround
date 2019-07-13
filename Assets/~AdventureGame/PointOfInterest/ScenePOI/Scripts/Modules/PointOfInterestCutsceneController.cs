using CoreGame;
using GameConfigurationID;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace AdventureGame
{
    public class PointOfInterestCutsceneController : APointOfInterestModule
    {
        #region External Dependencies
      //  private CutscenePlayerManager CutscenePlayerManager;
        private CutscenePlayerManagerV2 CutscenePlayerManagerV2;
        private CoreConfigurationManager CoreConfigurationManager;
        #endregion

        private Rigidbody Rigidbody;
        private NavMeshAgent Agent;

        private POICutsceneMoveManager POICutsceneMoveManager;

        #region Module Dependencies
        private PointOfInterestModelObjectModule PointOfInterestModelObjectModule;
        #endregion

        #region Data Components Dependencies
        private TransformMoveManagerComponentV2 PlayerInputMoveManagerComponentV2;
        #endregion

        #region State 
        private bool askedForWarp;
        private bool isAnimationPlaying;
        public bool IsAnimationPlaying { get => isAnimationPlaying; }

        public bool IsDirectedByCutscene()
        {
            return this.POICutsceneMoveManager.IsDirectedByAi || this.CutscenePlayerManagerV2.IsCutscenePlaying;
        }
        #endregion

        public void Init(PointOfInterestType pointOfInterestTypeRef, PointOfInterestModelObjectModule PointOfInterestModelObjectModule)
        {
            #region Data Components Dependencies
            this.PlayerInputMoveManagerComponentV2 = pointOfInterestTypeRef.POIDataComponentContainer.GetDataComponent<TransformMoveManagerComponentV2>();
            #endregion

            this.PointOfInterestModelObjectModule = PointOfInterestModelObjectModule;

            this.CutscenePlayerManagerV2 = GameObject.FindObjectOfType<CutscenePlayerManagerV2>();
            this.CoreConfigurationManager = GameObject.FindObjectOfType<CoreConfigurationManager>();

            this.Rigidbody = pointOfInterestTypeRef.GetComponentInParent<Rigidbody>();
            this.Agent = pointOfInterestTypeRef.GetComponentInParent<NavMeshAgent>();
            this.POICutsceneMoveManager = new POICutsceneMoveManager(this.Rigidbody, this.Agent);
        }

        public void Tick(float d)
        {
            this.POICutsceneMoveManager.Tick(d, this.PlayerInputMoveManagerComponentV2.SpeedMultiplicationFactor, this.PlayerInputMoveManagerComponentV2.AIRotationSpeed);
        }

        public void Warp(Transform warpPosition)
        {
            this.Agent.transform.position = warpPosition.position;
            this.Agent.transform.rotation = warpPosition.rotation;
        }

        public IEnumerator SetAIDestination(Transform destination, float normalizedSpeed)
        {
            yield return this.POICutsceneMoveManager.SetDestination(destination, normalizedSpeed);
            //Force position to ensure the destination is correctly reached
            this.Warp(destination);
        }

        public IEnumerator PlayAnimationAndWait(AnimationID animationID, float crossFadeDuration, Func<IEnumerator> animationEndCallback)
        {
            this.isAnimationPlaying = true;
            yield return AnimationPlayerHelper.PlayAndWait(this.PointOfInterestModelObjectModule.Animator, this.CoreConfigurationManager.AnimationConfiguration().ConfigurationInherentData[animationID], crossFadeDuration, animationEndCallback);
            this.isAnimationPlaying = false;
        }

        public void PlayAnimation(AnimationID animationID, float crossFadeDuration)
        {
            AnimationPlayerHelper.Play(this.PointOfInterestModelObjectModule.Animator, this.CoreConfigurationManager.AnimationConfiguration().ConfigurationInherentData[animationID], crossFadeDuration);
        }

        public float GetCurrentNormalizedSpeedMagnitude()
        {
            return this.POICutsceneMoveManager.GetCurrentNormalizedSpeedMagnitude();
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


        public void Tick(float d, float SpeedMultiplicationFactor, float AIRotationSpeed)
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
            }

        }

        public float GetCurrentNormalizedSpeedMagnitude()
        {
            if (this.isDirectedByAi)
            {
                return this.normalizedSpeedMagnitude;
            }
            else
            {
                return 0f;
            }
        }

        public IEnumerator SetDestination(Transform destination, float normalizedSpeed)
        {
            this.isDirectedByAi = true;
            this.normalizedSpeedMagnitude = normalizedSpeed;
            playerAgent.nextPosition = this.PlayerRigidBody.transform.position;
            playerAgent.SetDestination(destination.position);
            PlayerRigidBody.isKinematic = true;
            //Let the AI move
            yield return Coroutiner.Instance.StartCoroutine(new WaitForNavAgentDestinationReached(playerAgent));
            PlayerRigidBody.isKinematic = false;
            this.isDirectedByAi = false;
        }
    }

}
