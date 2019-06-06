using UnityEngine;
using UnityEngine.AI;
using CoreGame;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

        #region Data Retrieval
        public static NPCAIManager FromCollisionType(CollisionType collisionType)
        {
            if (collisionType == null) { return null; }
            return collisionType.GetComponent<NPCAIManager>();
        }
        #endregion

        public AIDestimationMoveManagerComponent AIDestimationMoveManagerComponent;

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
            var playerManagerDataRetriever = GameObject.FindObjectOfType<PlayerManagerDataRetriever>();

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
            puzzleAIBehavior = PuzzleAIBehavior<AbstractAIComponents>.BuildAIBehaviorFromType(aiBehaviorInherentData.BehaviorType,
                new AIBheaviorBuildInputData(agent, aiBehaviorInherentData.AIComponents, OnFOVChange, PuzzleEventsManager, playerManagerDataRetriever, targetZoneContainer, this.AiID, this.objectCollider, this.ForceTickAI, this.AIDestimationMoveManagerComponent));
            NPCAnimationDataManager = new NPCAnimationDataManager(animator);
            ContextMarkVisualFeedbackManager = new ContextMarkVisualFeedbackManager(this, NpcFOVRingManager, puzzleCOnfigurationmanager);
            AnimationVisualFeedbackManager = new AnimationVisualFeedbackManager(animator);

            //Intiialize with 0 time
            this.TickWhenTimeFlows(0, 0);
        }

        public void TickWhenTimeFlows(in float d, in float timeAttenuationFactor)
        {
            this.ComputeAINewDestination(d, timeAttenuationFactor);
            AIDestinationMoveManager.Tick(d, timeAttenuationFactor);
            NPCSpeedAdjusterManager.Tick(d, timeAttenuationFactor);
        }

        internal void TickAlways(in float d, in float timeAttenuationFactor)
        {
            NPCAnimationDataManager.Tick(timeAttenuationFactor);
            NpcFOVRingManager.Tick(d);
            ContextMarkVisualFeedbackManager.Tick(d);
        }

        internal void EndOfFixedTick()
        {
            this.puzzleAIBehavior.EndOfFixedTick();
        }

        private void ForceTickAI()
        {
            this.ComputeAINewDestination(0, 0);
        }

        private void ComputeAINewDestination(in float d, in float timeAttenuationFactor)
        {
            var newDestination = puzzleAIBehavior.TickAI(d, timeAttenuationFactor);
            if (newDestination.HasValue)
            {
                SetDestinationWithCoroutineReached(newDestination.Value);
            }
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

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            var labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.normal.textColor = Color.magenta;
            Handles.Label(transform.position + new Vector3(0, 3f, 0), AiID.ToString(), labelStyle);
            Gizmos.DrawIcon(transform.position + new Vector3(0, 5.5f, 0), "Gizmo_AI", true);
#endif
        }

        public void GUITick()
        {
#if UNITY_EDITOR
            if (DebugEabled)
            {
                var mouseAIBehavior = puzzleAIBehavior as GenericPuzzleAIBehavior;
                var screenPos = Camera.main.WorldToScreenPoint(transform.position);
                screenPos.y = Camera.main.pixelHeight - screenPos.y;
                GUILayout.BeginArea(new Rect(screenPos, new Vector2(200, 300)));
                GUILayout.BeginVertical("box");
                GUILayout.Label("Position : " + transform.position.ToString());
                GUILayout.Label("Agent speed : " + this.agent.velocity.ToString("F8"));
                mouseAIBehavior.DebugGUITick();
                GUILayout.EndVertical();
                GUILayout.EndArea();
            }
#endif
        }

        #region External Events
        public void OnProjectileTriggerEnter(LaunchProjectile launchProjectile)
        {
            this.puzzleAIBehavior.ReceiveEvent(new ProjectileTriggerEnterAIBehaviorEvent(launchProjectile));
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
            this.ContextMarkVisualFeedbackManager.ReceiveEvent(ContextMarkVisualFeedbackEvent.PROJECTILE_HITTED_FIRST_TIME, this.AiID);
            this.AnimationVisualFeedbackManager.OnHittedByProjectileFirstTime();
        }

        public void OnEscapeWithoutTargetStart()
        {
            this.ContextMarkVisualFeedbackManager.ReceiveEvent(ContextMarkVisualFeedbackEvent.ESCAPE_WITHOUT_TARGET, this.AiID);
            this.AnimationVisualFeedbackManager.OnEscapeWithoutTargetStart();
        }
        public void OnEscapeWithoutTargetEnd()
        {
            this.ContextMarkVisualFeedbackManager.ReceiveEvent(ContextMarkVisualFeedbackEvent.DELETE, this.AiID);
        }
        public void OnAiAffectedByProjectileEnd()
        {
            this.ContextMarkVisualFeedbackManager.ReceiveEvent(ContextMarkVisualFeedbackEvent.DELETE, this.AiID);
        }

        internal void OnGameOver()
        {
            this.ContextMarkVisualFeedbackManager.ReceiveEvent(ContextMarkVisualFeedbackEvent.DELETE, this.AiID);
        }

        internal void OnAIFearedStunnedEnded()
        {
            this.puzzleAIBehavior.ReceiveEvent(new FearedEndAIBehaviorEvent(
                eventProcessedCallback: () =>
                {
                    this.AnimationVisualFeedbackManager.OnAIFearedStunnedEnded();
                }
           ));
        }

        internal void OnAIFearedForced(float fearTime)
        {
            this.puzzleAIBehavior.ReceiveEvent(new FearedForcedAIBehaviorEvent(fearTime));
        }

        internal void OnAIFearedStunned()
        {
            this.puzzleAIBehavior.ReceiveEvent(new FearedStartAIBehaviorEvent(
                eventProcessedCallback: () =>
                {
                    this.AnimationVisualFeedbackManager.OnAIFearedStunned();
                }
            ));
        }

        internal void OnAIAttractedStart(AttractiveObjectType attractiveObject)
        {
            this.ContextMarkVisualFeedbackManager.ReceiveEvent(ContextMarkVisualFeedbackEvent.ATTRACTED_START, this.AiID);
        }
        internal void OnAIAttractedEnd()
        {
            this.ContextMarkVisualFeedbackManager.ReceiveEvent(ContextMarkVisualFeedbackEvent.DELETE, this.AiID);
        }
        public void OnAttractiveObjectDestroyed(AttractiveObjectType attractiveObjectToDestroy)
        {
            this.puzzleAIBehavior.OnAttractiveObjectDestroyed(attractiveObjectToDestroy);
        }
        public void OnDestinationReached()
        {
            puzzleAIBehavior.OnDestinationReached();
            // empty tick to immediately choose the next position
            this.ForceTickAI();
        }
        #endregion

        #region Internal Events
        private void OnFOVChange(FOV currentFOV)
        {
            NpcFOVRingManager.OnFOVChanged(currentFOV);
        }
        private void SendOnDestinationReachedEvent()
        {
            this.PuzzleEventsManager.PZ_EVT_AI_DestinationReached(this.AiID);
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

        internal void OnEscapeWithoutTargetStart()
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
