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
        private Collider objectCollider;
        #endregion

        #region AI Behavior Components
        [Header("AI Behavior Components")]
        public AiID AiID;
        #endregion

        public AIDestimationMoveManagerComponent AIDestimationMoveManagerComponent;
        public ContextMarkVisualFeedbackManagerComponent ContextMarkVisualFeedbackManagerComponent;

        private NPCAIDestinationMoveManager AIDestinationMoveManager;
        private NPCSpeedAdjusterManager NPCSpeedAdjusterManager;

        private IPuzzleAIBehavior<AbstractAIComponents> puzzleAIBehavior;
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
            var targetZoneContainer = GameObject.FindObjectOfType<TargetZoneContainer>();

            var camera = Camera.main;
            var canvas = GameObject.FindObjectOfType<Canvas>();

            var animator = GetComponentInChildren<Animator>();
            this.objectCollider = GetComponent<Collider>();
            agent = GetComponent<NavMeshAgent>();
            agent.updatePosition = false;
            agent.updateRotation = false;

            var aiBehaviorInherentData = puzzleCOnfigurationmanager.AIComponentsConfiguration()[AiID];
            var PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();

            NpcFOVRingManager = new NpcInteractionRingManager(this);

            AIDestinationMoveManager = new NPCAIDestinationMoveManager(AIDestimationMoveManagerComponent, agent, transform, this.SendOnDestinationReachedEvent);
            NPCSpeedAdjusterManager = new NPCSpeedAdjusterManager(agent);
            puzzleAIBehavior = PuzzleAIBehavior<AbstractAIComponents>.BuildAIBehaviorFromType(aiBehaviorInherentData.BehaviorType, new AIBheaviorBuildInputData(agent, aiBehaviorInherentData.AIComponents, OnFOVChange, PuzzleEventsManager, targetZoneContainer, this.AiID));
            NPCAnimationDataManager = new NPCAnimationDataManager(animator);
            ContextMarkVisualFeedbackManager = new ContextMarkVisualFeedbackManager(ContextMarkVisualFeedbackManagerComponent, this, camera, canvas, NpcFOVRingManager);
            AnimationVisualFeedbackManager = new AnimationVisualFeedbackManager(animator);

            //Intiialize with 0 time
            this.TickWhenTimeFlows(0, 0);
        }

        public void TickWhenTimeFlows(in float d, in float timeAttenuationFactor)
        {
            this.ComputeAINewDestination(d, timeAttenuationFactor);
            AIDestinationMoveManager.Tick(d);
            NPCSpeedAdjusterManager.Tick(d, timeAttenuationFactor);
        }

        private Vector3? ComputeAINewDestination(in float d, in float timeAttenuationFactor)
        {
            var newDestination = puzzleAIBehavior.TickAI(d, timeAttenuationFactor);
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

        public void OnTriggerEnter(Collider other)
        {
            puzzleAIBehavior.OnTriggerEnter(other);
        }

        private void OnTriggerStay(Collider other)
        {
            puzzleAIBehavior.OnTriggerStay(other);
        }

        private void OnTriggerExit(Collider other)
        {
            puzzleAIBehavior.OnTriggerExit(other);
        }

        public void GizmoTick()
        {
            puzzleAIBehavior.TickGizmo();
        }

        public void GUITick()
        {
            if (DebugEabled)
            {
                var mouseAIBehavior = puzzleAIBehavior as GenericPuzzleAIBehavior;
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
        public void OnProjectileTriggerEnter(SphereCollider sphereCollider)
        {
            this.puzzleAIBehavior.OnProjectileTriggerEnter(sphereCollider);
        }

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
        public void OnAiAffectedByProjectileEnd()
        {
            this.ContextMarkVisualFeedbackManager.OnAiAffectedByProjectileEnd();
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

        internal void OnAIAttractedStart(AttractiveObjectType attractiveObject)
        {
            this.ContextMarkVisualFeedbackManager.OnAIAttractedStart(attractiveObject);
        }
        internal void OnAIAttractedEnd()
        {
            this.ContextMarkVisualFeedbackManager.OnAIAttractedEnd();
        }
        public void OnAttractiveObjectDestroyed(AttractiveObjectType attractiveObjectToDestroy)
        {
            this.puzzleAIBehavior.OnAttractiveObjectDestroyed(attractiveObjectToDestroy);
        }
        public void OnDestinationReached()
        {
            puzzleAIBehavior.OnDestinationReached();
            // empty tick to immediately choose the next position
            this.ComputeAINewDestination(0, 0);
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
        public IPuzzleAIBehavior<AbstractAIComponents> GetAIBehavior()
        {
            return this.puzzleAIBehavior;
        }
        public NavMeshAgent GetAgent()
        {
            return this.agent;
        }
        public Collider GetCollider()
        {
            return this.objectCollider;
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

    [System.Serializable]
    public class ContextMarkVisualFeedbackManagerComponent
    {
        public float YOffsetWhenInteractionRingIsDisplayed;
    }

    class ContextMarkVisualFeedbackManager
    {

        private ContextMarkVisualFeedbackManagerComponent ContextMarkVisualFeedbackManagerComponent;

        #region External Dependencies
        private NPCAIManager NPCAIManagerRef;
        private Camera camera;
        private Canvas mainCanvas;
        private NpcInteractionRingManager NpcInteractionRingManager;
        #endregion

        public ContextMarkVisualFeedbackManager(ContextMarkVisualFeedbackManagerComponent ContextMarkVisualFeedbackManagerComponent,
            NPCAIManager NPCAIManagerRef, Camera camera, Canvas mainCanvas, NpcInteractionRingManager npcFOVRingManager)
        {
            this.ContextMarkVisualFeedbackManagerComponent = ContextMarkVisualFeedbackManagerComponent;
            this.NPCAIManagerRef = NPCAIManagerRef;
            this.camera = camera;
            this.isVisualMarkDisplayed = false;
            this.visualFeedbackMark = null;
            this.mainCanvas = mainCanvas;
            this.NpcInteractionRingManager = npcFOVRingManager;
        }

        private bool isVisualMarkDisplayed;
        private AIFeedbackMarkType visualFeedbackMark;

        public void Tick(float d)
        {
            if (this.isVisualMarkDisplayed)
            {
                var visualMarkPosition = this.NPCAIManagerRef.transform.position + this.NPCAIManagerRef.GetInteractionRingOffset();
                if (this.NpcInteractionRingManager.IsRingEnabled())
                {
                    visualMarkPosition.y += this.ContextMarkVisualFeedbackManagerComponent.YOffsetWhenInteractionRingIsDisplayed;
                }
                this.visualFeedbackMark.transform.position = visualMarkPosition;
                this.visualFeedbackMark.Tick(d);
            }
        }

        private void ReInitBeforeSpawningMark()
        {
            this.isVisualMarkDisplayed = false;
            if (this.visualFeedbackMark != null)
            {
                MonoBehaviour.Destroy(this.visualFeedbackMark.gameObject);
            }
        }

        internal void OnHittedByProjectileFirstTime()
        {
            ReInitBeforeSpawningMark();
            this.isVisualMarkDisplayed = true;
            this.visualFeedbackMark = AIFeedbackMarkType.Instanciate(PrefabContainer.Instance.ExclamationMarkSimple, this.NPCAIManagerRef.transform);
        }

        internal void OnHittedByProjectile2InARow()
        {
            ReInitBeforeSpawningMark();
            this.isVisualMarkDisplayed = true;
            this.visualFeedbackMark = AIFeedbackMarkType.Instanciate(PrefabContainer.Instance.ExclamationMarkDouble, this.NPCAIManagerRef.transform);
        }

        internal void OnAiAffectedByProjectileEnd()
        {
            ReInitBeforeSpawningMark();
        }

        internal void OnAIAttractedStart(AttractiveObjectType attractiveObject)
        {
            ReInitBeforeSpawningMark();
            this.isVisualMarkDisplayed = true;
            this.visualFeedbackMark = AIFeedbackMarkType.Instanciate(attractiveObject.AttractiveObjectInherentConfigurationData.AttractiveObjectAIMarkPrefab, this.NPCAIManagerRef.transform);
        }

        internal void OnAIAttractedEnd()
        {
            this.ReInitBeforeSpawningMark();
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
