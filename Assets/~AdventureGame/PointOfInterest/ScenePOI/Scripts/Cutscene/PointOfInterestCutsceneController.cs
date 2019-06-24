﻿using CoreGame;
using GameConfigurationID;
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
        private CoreConfigurationManager CoreConfigurationManager;
        #endregion
        
        private Rigidbody Rigidbody;
        private NavMeshAgent Agent;

        #region Internal Dependencies
        private PointOfInterestModelObjectType PointOfInterestModelObjectTypeRef;
        #endregion

        private POICutsceneMoveManager POICutsceneMoveManager;

        #region State 
        private bool askedForWarp;
        public bool IsDirectedByCutscene()
        {
            return this.POICutsceneMoveManager.IsDirectedByAi || this.CutscenePlayerManager.IsCutscenePlaying;
        }
        #endregion

        public void Init(PointOfInterestModelObjectType PointOfInterestModelObjectTypeRef)
        {
            this.CutscenePlayerManager = GameObject.FindObjectOfType<CutscenePlayerManager>();
            this.CoreConfigurationManager = GameObject.FindObjectOfType<CoreConfigurationManager>();
            
            this.PointOfInterestModelObjectTypeRef = PointOfInterestModelObjectTypeRef;

            this.Rigidbody = GetComponentInParent<Rigidbody>();
            this.Agent = GetComponentInParent<NavMeshAgent>();
            this.POICutsceneMoveManager = new POICutsceneMoveManager(this.Rigidbody, this.Agent);
        }

        public float Tick(float d, float SpeedMultiplicationFactor, float AIRotationSpeed)
        {
            return this.POICutsceneMoveManager.Tick(d, SpeedMultiplicationFactor, AIRotationSpeed);
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
            yield return AnimationPlayerHelper.PlayAndWait(this.PointOfInterestModelObjectTypeRef.Animator, this.CoreConfigurationManager.AnimationConfiguration().ConfigurationInherentData[animationID], crossFadeDuration, animationEndCallback);
        }

        public void PlayAnimation(AnimationID animationID, float crossFadeDuration)
        {
            AnimationPlayerHelper.Play(this.PointOfInterestModelObjectTypeRef.Animator, this.CoreConfigurationManager.AnimationConfiguration().ConfigurationInherentData[animationID], crossFadeDuration);
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
