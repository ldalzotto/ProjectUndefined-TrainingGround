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

        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

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

        public void Init()
        {
            this.PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            var puzzleCOnfigurationmanager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var NPCAIManagerContainer = GameObject.FindObjectOfType<NPCAIManagerContainer>();
            NPCAIManagerContainer.OnNPCAiManagerCreated(this);

            var camera = Camera.main;
            var canvas = GameObject.FindObjectOfType<Canvas>();

            var animator = GetComponentInChildren<Animator>();
            agent = GetComponent<NavMeshAgent>();
            agent.updatePosition = false;
            agent.updateRotation = false;

            var aiComponent = puzzleCOnfigurationmanager.AIComponentsConfiguration()[AiID];
            var PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();

            NpcFOVRingManager = new NpcInteractionRingManager(this);

            AIDestinationMoveManager = new NPCAIDestinationMoveManager(AIDestimationMoveManagerComponent, agent, transform, this.SendOnDestinationReachedEvent);
            NPCSpeedAdjusterManager = new NPCSpeedAdjusterManager(agent);
            PuzzleAIBehavior = new MouseAIBehavior(agent, aiComponent, OnFOVChange, PuzzleEventsManager, this.AiID);
            NPCAnimationDataManager = new NPCAnimationDataManager(animator);
            ContextMarkVisualFeedbackManager = new ContextMarkVisualFeedbackManager(this, camera, canvas);
            AnimationVisualFeedbackManager = new AnimationVisualFeedbackManager(animator);
        }

        public void TickWhenTimeFlows(in float d, in float timeAttenuationFactor)
        {
            this.ComputeAINewDestination(d, timeAttenuationFactor);
            AIDestinationMoveManager.Tick(d);
            NPCSpeedAdjusterManager.Tick(d, timeAttenuationFactor);
        }

        private Vector3? ComputeAINewDestination(in float d, in float timeAttenuationFactor)
        {
            var newDestination = PuzzleAIBehavior.TickAI(d, timeAttenuationFactor);
            if (newDestination.HasValue)
            {
                SetDestinationWithCoroutineReached(newDestination.Value);
            }
            return newDestination;
        }

        internal void TickAlways(in float d, in float timeAttenuationFactor)
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

        public void OnAttractiveObjectDestroyed(AttractiveObjectType attractiveObjectToDestroy)
        {

            this.PuzzleAIBehavior.OnAttractiveObjectDestroyed(attractiveObjectToDestroy);
        }
        public void OnDestinationReached()
        {
            PuzzleAIBehavior.OnDestinationReached();
            // empty tick to immediately choose the next position
            this.ComputeAINewDestination(0,0);
        }
        #endregion

        #region Internal Events
        private void OnFOVChange(FOV currentFOV)
        {
            NpcFOVRingManager.OnFOVChanged(currentFOV);
        }
        private void SendOnDestinationReachedEvent()
        {
            this.PuzzleEventsManager.OnDestinationReached(this.AiID);
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
        public PuzzleAIBehavior GetAIBehavior()
        {
            return this.PuzzleAIBehavior;
        }
        public NavMeshAgent GetAgent()
        {
            return this.agent;
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
            if (this.destroyCoroutine != null)
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
