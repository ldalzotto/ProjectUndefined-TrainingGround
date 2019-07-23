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
        private CutscenePlayerManagerV2 CutscenePlayerManagerV2;
        private CoreConfigurationManager CoreConfigurationManager;
        #endregion

        private Rigidbody Rigidbody;
        private NavMeshAgent Agent;

        private POICutsceneMoveManager POICutsceneMoveManager;
        private PlayerAnimationDataManager PlayerAnimationDataManager;

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

            if (!pointOfInterestTypeRef.IsPlayer())
            {
                this.PlayerAnimationDataManager = new PlayerAnimationDataManager(this.PointOfInterestModelObjectModule.Animator);
            }
        }

        public void Tick(float d)
        {
            this.POICutsceneMoveManager.Tick(d, this.PlayerInputMoveManagerComponentV2.SpeedMultiplicationFactor, this.PlayerInputMoveManagerComponentV2.AIRotationSpeed);

            this.PlayerAnimationDataManager.IfNotNull((PlayerAnimationDataManager) => PlayerAnimationDataManager.Tick(this.GetCurrentNormalizedSpeedMagnitude()));
        }

        public void Warp(Transform warpPosition)
        {
            this.Agent.Warp(warpPosition.position);
            this.Agent.transform.position = warpPosition.position;
            this.Agent.transform.rotation = warpPosition.rotation;
        }

        public IEnumerator SetAIDestination(Transform destination, float normalizedSpeed, AnimationCurve speedFactorOverDistance)
        {
            yield return this.POICutsceneMoveManager.SetDestination(destination, normalizedSpeed, speedFactorOverDistance);
            //Force position to ensure the destination is correctly reached
            this.Warp(destination);
        }

        public IEnumerator PlayAnimationAndWait(AnimationID animationID, float crossFadeDuration, Func<IEnumerator> animationEndCallback, bool updateModelImmediately, bool framePerfectEndDetection)
        {
            this.isAnimationPlaying = true;
            yield return AnimationPlayerHelper.PlayAndWait(this.PointOfInterestModelObjectModule.Animator, this.CoreConfigurationManager.AnimationConfiguration().ConfigurationInherentData[animationID], crossFadeDuration, animationEndCallback, updateModelImmediately, framePerfectEndDetection);
            this.isAnimationPlaying = false;
        }

        public void Play(AnimationID animationID, float crossFadeDuration, bool updateModelImmediately)
        {
            AnimationPlayerHelper.Play(this.PointOfInterestModelObjectModule.Animator, this.CoreConfigurationManager.AnimationConfiguration().ConfigurationInherentData[animationID], crossFadeDuration, updateModelImmediately);
        }

        // public void Play()

        public void StopAnimation(AnimationID animationID)
        {
            AnimationPlayerHelper.Play(this.PointOfInterestModelObjectModule.Animator, this.CoreConfigurationManager.AnimationConfiguration().ConfigurationInherentData[AnimationID.ACTION_LISTENING], 0f);
            this.isAnimationPlaying = false;
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
        private float distanceAttenuatedNormalizedSpeedMagnitude;
        private AnimationCurve speedFactorOverDistance;
        private float currentPathTotalDistance;

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
                this.distanceAttenuatedNormalizedSpeedMagnitude = this.normalizedSpeedMagnitude;

                if (this.speedFactorOverDistance != null)
                {
                    if (this.currentPathTotalDistance == 0f)
                    {
                        var pathCorners = this.playerAgent.path.corners;
                        for (var i = 1; i < pathCorners.Length; i++)
                        {
                            this.currentPathTotalDistance += Vector3.Distance(pathCorners[i - 1], pathCorners[i]);
                        }
                    }
                    else
                    {
                        var distanceAttanuationFacotr = this.speedFactorOverDistance.Evaluate(Mathf.Clamp01(1 - (this.playerAgent.remainingDistance / this.currentPathTotalDistance)));
                        playerAgent.speed *= distanceAttanuationFacotr;
                        this.distanceAttenuatedNormalizedSpeedMagnitude *= distanceAttanuationFacotr;
                    }
                }

                PlayerRigidBody.transform.position = playerAgent.nextPosition;
            }

        }

        public float GetCurrentNormalizedSpeedMagnitude()
        {
            if (this.isDirectedByAi)
            {
                return this.distanceAttenuatedNormalizedSpeedMagnitude;
            }
            else
            {
                return 0f;
            }
        }

        public IEnumerator SetDestination(Transform destination, float normalizedSpeed, AnimationCurve speedFactorOverDistance)
        {
            this.currentPathTotalDistance = 0f;
            this.isDirectedByAi = true;
            this.normalizedSpeedMagnitude = normalizedSpeed;
            this.speedFactorOverDistance = speedFactorOverDistance;
            playerAgent.nextPosition = this.PlayerRigidBody.transform.position;
            playerAgent.SetDestination(destination.position);
            PlayerRigidBody.isKinematic = true;
            //Let the AI move
            yield return Coroutiner.Instance.StartCoroutine(new WaitForNavAgentDestinationReached(playerAgent));
            PlayerRigidBody.isKinematic = false;
            playerAgent.ResetPath();
            this.isDirectedByAi = false;
        }
    }

}
