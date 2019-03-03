﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{

    public class NPCAIManager : MonoBehaviour
    {

        public const string AnimationName_OnHittedByProjectileFirstTime = "OnHittedByProjectileFirstTime";
        public const string AnimationName_OnHittedByProjectile2InARow = "OnHittedByProjectile2InARow";
        public const string AnimationName_Fear = "Fear";
        public const string AnimationName_FearListening = "FearListening";


        [Header("Debug")]
        public bool DebugEabled;

        #region Internal Dependencies
        private NavMeshAgent agent;
        #endregion

        #region AI Behavior Components
        [Header("AI Behavior Components")]
        public AiID AiID;
        #endregion

        public AIDestimationMoveManagerComponent AIDestimationMoveManagerComponent;

        private NPCAIDestinationMoveManager AIDestinationMoveManager;
        private NPCSpeedAdjusterManager NPCSpeedAdjusterManager;

        private PuzzleAIBehavior PuzzleAIBehavior;
        private NPCAnimationDataManager NPCAnimationDataManager;
        private NpcInteractionRingManager NpcFOVRingManager;
        private ContextMarkVisualFeedbackManager ContextMarkVisualFeedbackManager;
        private AnimationVisualFeedbackManager AnimationVisualFeedbackManager;

        private Coroutine destinatioNReachedCoroutine;

        public void Init()
        {
            var NPCAIManagerContainer = GameObject.FindObjectOfType<NPCAIManagerContainer>();
            NPCAIManagerContainer.OnNPCAiManagerCreated(this);

            var camera = Camera.main;
            var canvas = GameObject.FindObjectOfType<Canvas>();

            var animator = GetComponentInChildren<Animator>();
            agent = GetComponent<NavMeshAgent>();
            agent.updatePosition = false;
            agent.updateRotation = false;

            var aiComponent = GameObject.FindObjectOfType<AIComponentsManager>().Get(AiID);
            var PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();

            NpcFOVRingManager = new NpcInteractionRingManager(this);

            AIDestinationMoveManager = new NPCAIDestinationMoveManager(AIDestimationMoveManagerComponent, agent, transform);
            NPCSpeedAdjusterManager = new NPCSpeedAdjusterManager(agent);
            PuzzleAIBehavior = new MouseAIBehavior(agent, aiComponent, OnFOVChange, PuzzleEventsManager, this.AiID);
            NPCAnimationDataManager = new NPCAnimationDataManager(animator);
            ContextMarkVisualFeedbackManager = new ContextMarkVisualFeedbackManager(this, camera, canvas);
            AnimationVisualFeedbackManager = new AnimationVisualFeedbackManager(animator);
        }

        public void TickWhenTimeFlows(float d, float timeAttenuationFactor)
        {
            var newDestination = PuzzleAIBehavior.TickAI(d, timeAttenuationFactor);
            if (newDestination.HasValue)
            {
                SetDestinationWithCoroutineReached(newDestination.Value);
            }

            AIDestinationMoveManager.Tick(d);
            NPCSpeedAdjusterManager.Tick(d, timeAttenuationFactor);
        }

        internal void TickAlways(float d, float timeAttenuationFactor)
        {
            NPCAnimationDataManager.Tick(timeAttenuationFactor);
            NpcFOVRingManager.Tick(d);
            ContextMarkVisualFeedbackManager.Tick(d);
        }

        private void OnTriggerEnter(Collider other)
        {
            PuzzleAIBehavior.OnTriggerEnter(other);
        }

        private void OnTriggerStay(Collider other)
        {
            PuzzleAIBehavior.OnTriggerStay(other);
        }

        private void OnTriggerExit(Collider other)
        {
            PuzzleAIBehavior.OnTriggerExit(other);
        }

        public void GizmoTick()
        {
            PuzzleAIBehavior.TickGizmo();
        }

        public void GUITick()
        {
            if (DebugEabled)
            {
                var mouseAIBehavior = PuzzleAIBehavior as MouseAIBehavior;
                var screenPos = Camera.main.WorldToScreenPoint(transform.position);
                screenPos.y = Camera.main.pixelHeight - screenPos.y;
                GUILayout.BeginArea(new Rect(screenPos, new Vector2(200, 300)));
                GUILayout.BeginVertical("box");
                GUILayout.Label("Position : " + transform.position.ToString());
                mouseAIBehavior.DebugGUITick();
                GUILayout.EndVertical();
                GUILayout.EndArea();
            }
        }

        #region External Events
        private void SetDestinationWithCoroutineReached(Vector3 destination)
        {
            AIDestinationMoveManager.SetDestination(destination);

            if (destinatioNReachedCoroutine != null)
            {
                StopCoroutine(destinatioNReachedCoroutine);
            }

            destinatioNReachedCoroutine = Coroutiner.Instance.StartCoroutine(OnDestinationReached());
        }
        private void SetDestinationSilent(Vector3 destination)
        {
            AIDestinationMoveManager.SetDestination(destination);
        }
        public void EnableAgent()
        {
            AIDestinationMoveManager.EnableAgent();
        }
        public void DisableAgent()
        {
            AIDestinationMoveManager.DisableAgent();
        }

        public void OnHittedByProjectileFirstTime()
        {
            this.ContextMarkVisualFeedbackManager.OnHittedByProjectileFirstTime();
            this.AnimationVisualFeedbackManager.OnHittedByProjectileFirstTime();
        }

        public void OnHittedByProjectile2InARow()
        {
            this.ContextMarkVisualFeedbackManager.OnHittedByProjectile2InARow();
            this.AnimationVisualFeedbackManager.OnHittedByProjectile2InARow();
        }

        internal void OnGameOver()
        {
            this.ContextMarkVisualFeedbackManager.OnGameOver();
        }


        internal void OnAIFearedStunnedEnded()
        {
            this.AnimationVisualFeedbackManager.OnAIFearedStunnedEnded();
        }

        internal void OnAIFearedStunned()
        {
            this.AnimationVisualFeedbackManager.OnAIFearedStunned();
        }
        #endregion

        #region Internal Events
        private IEnumerator OnDestinationReached()
        {
            yield return new WaitForNavAgentDestinationReached(agent);
            PuzzleAIBehavior.OnDestinationReached();
        }

        private void OnFOVChange(FOV currentFOV)
        {
            NpcFOVRingManager.OnFOVChanged(currentFOV);
        }
        #endregion

        #region Data Retrieval
        public Renderer[] GetRenderers()
        {
            return GetComponentsInChildren<Renderer>();
        }
        public Vector3 GetInteractionRingOffset()
        {
            return this.NpcFOVRingManager.RingPositionOffset;
        }
        #endregion
    }

    class NPCSpeedAdjusterManager
    {
        NavMeshAgent agent;

        public NPCSpeedAdjusterManager(NavMeshAgent agent)
        {
            this.agent = agent;
        }

        public void Tick(float d, float playerSpeedMagnitude)
        {
            this.agent.speed = this.agent.speed * playerSpeedMagnitude;
        }
    }

    class NPCAnimationDataManager
    {
        private const string ANIM_SpeedParameter = "Speed";
        private Animator NPCAnimator;

        public NPCAnimationDataManager(Animator nPCAnimator)
        {
            NPCAnimator = nPCAnimator;
        }

        public void Tick(float timeAttenuation)
        {
            NPCAnimator.SetFloat(ANIM_SpeedParameter, timeAttenuation);
        }

    }

    class ContextMarkVisualFeedbackManager
    {

        private NPCAIManager NPCAIManagerRef;
        private Camera camera;
        private Canvas mainCanvas;

        public ContextMarkVisualFeedbackManager(NPCAIManager NPCAIManagerRef, Camera camera, Canvas mainCanvas)
        {
            this.NPCAIManagerRef = NPCAIManagerRef;
            this.camera = camera;
            this.isVisualMarkDisplayed = false;
            this.visualFeedbackMark = null;
            this.mainCanvas = mainCanvas;
        }

        private bool isVisualMarkDisplayed;
        private GameObject visualFeedbackMark;
        private Coroutine destroyCoroutine;

        public void Tick(float d)
        {
            if (this.isVisualMarkDisplayed)
            {
                this.visualFeedbackMark.transform.position = camera.WorldToScreenPoint(this.NPCAIManagerRef.transform.position + this.NPCAIManagerRef.GetInteractionRingOffset());
            }
        }

        private IEnumerator DestroyMarkAfter1Second()
        {
            yield return new WaitForSeconds(1f);
            MonoBehaviour.Destroy(this.visualFeedbackMark);
            this.isVisualMarkDisplayed = false;
        }

        private void ReInitBeforeSpawningMark()
        {
            this.isVisualMarkDisplayed = false;
            if (this.visualFeedbackMark != null)
            {
                MonoBehaviour.Destroy(this.visualFeedbackMark);
            }
            if(this.destroyCoroutine != null)
            {
                Coroutiner.Instance.StopCoroutine(this.destroyCoroutine);
            }
        }

        internal void OnHittedByProjectileFirstTime()
        {
            ReInitBeforeSpawningMark();
            this.isVisualMarkDisplayed = true;
            this.visualFeedbackMark = MonoBehaviour.Instantiate(PrefabContainer.Instance.ExclamationMarkSimple, this.mainCanvas.transform);
            this.destroyCoroutine = Coroutiner.Instance.StartCoroutine(this.DestroyMarkAfter1Second());
        }

        internal void OnHittedByProjectile2InARow()
        {
            ReInitBeforeSpawningMark();
            this.isVisualMarkDisplayed = true;
            this.visualFeedbackMark = MonoBehaviour.Instantiate(PrefabContainer.Instance.ExclamationMarkDouble, this.mainCanvas.transform);
            this.destroyCoroutine = Coroutiner.Instance.StartCoroutine(this.DestroyMarkAfter1Second());
        }

        internal void OnGameOver()
        {
            ReInitBeforeSpawningMark();
        }
    }

    class AnimationVisualFeedbackManager
    {
        private Animator Animator;

        public AnimationVisualFeedbackManager(Animator animator)
        {
            Animator = animator;
        }

        internal void OnHittedByProjectileFirstTime()
        {
            this.Animator.Play(NPCAIManager.AnimationName_OnHittedByProjectileFirstTime);
        }

        internal void OnHittedByProjectile2InARow()
        {
            this.Animator.Play(NPCAIManager.AnimationName_OnHittedByProjectile2InARow);
        }

        internal void OnAIFearedStunnedEnded()
        {
            this.Animator.Play(NPCAIManager.AnimationName_FearListening);
        }

        internal void OnAIFearedStunned()
        {
            this.Animator.Play(NPCAIManager.AnimationName_Fear);
        }
    }
}
