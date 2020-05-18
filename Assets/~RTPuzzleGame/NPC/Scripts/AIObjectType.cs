﻿using UnityEngine;
using UnityEngine.AI;
using CoreGame;
using GameConfigurationID;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTPuzzle
{

    public class AIObjectType : MonoBehaviour, IRenderBoundRetrievable, SightTrackingListener
    {

#if UNITY_EDITOR
        [Header("Debug")]
        public bool DebugEabled;
#endif

        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        #region Internal Dependencies
        private NavMeshAgent agent;
        private Collider objectCollider;
        #endregion

        #region AI Behavior Components
        [CustomEnum(ConfigurationType = typeof(AIObjectTypeDefinitionConfiguration), OpenToConfiguration = true)]
        public AIObjectTypeDefinitionID AIObjectTypeDefinitionID;

        public AiID AiID;
        
        #endregion

        #region State
        private NPCAIDestinationContext NPCAIDestinationContext;
        #endregion

        #region Data Retrieval
        public static AIObjectType FromCollisionType(CollisionType collisionType)
        {
            if (collisionType == null) { return null; }
            return collisionType.GetComponent<AIObjectType>();
        }

#if UNITY_EDITOR
        public AIDestinationMoveManager GetNPCAIDestinationMoveManager() { return this.AIDestinationMoveManager; }
#endif
        #endregion

        private InteractiveObjectSharedDataType interactiveObjectSharedData;
        private AIDestinationMoveManager AIDestinationMoveManager;
        private NPCSpeedAdjusterManager NPCSpeedAdjusterManager;

        private IPuzzleAIBehavior<AbstractAIComponents> puzzleAIBehavior;
        private NpcInteractionRingManager NpcFOVRingManager;
        private ContextMarkVisualFeedbackManager ContextMarkVisualFeedbackManager;
        private AnimationVisualFeedbackManager AnimationVisualFeedbackManager;
        private LineVisualFeedbackManager LineVisualFeedbackManager;
        private AIAnimationManager NPCAIAnimationManager;

        public void Init()
        {
            this.PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            var puzzleCOnfigurationmanager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var NPCAIManagerContainer = GameObject.FindObjectOfType<AIManagerContainer>();
            NPCAIManagerContainer.OnNPCAiManagerCreated(this);
            var interactiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();
            var playerManagerDataRetriever = GameObject.FindObjectOfType<PlayerManagerDataRetriever>();
            var coreConfigurationManager = GameObject.FindObjectOfType<CoreConfigurationManager>();
            var aiPositionsManager = GameObject.FindObjectOfType<AIPositionsManager>();
            var animationConfiguration = coreConfigurationManager.AnimationConfiguration();

            var rigidBody = GetComponent<Rigidbody>();
            var animator = GetComponentInChildren<Animator>();
            this.objectCollider = GetComponent<Collider>();
            this.interactiveObjectSharedData = GetComponent<InteractiveObjectSharedDataType>();

            this.NPCAIDestinationContext = new NPCAIDestinationContext();

            agent = GetComponent<NavMeshAgent>();
            agent.updatePosition = false;
            agent.updateRotation = false;

            var aiBehaviorInherentData = puzzleCOnfigurationmanager.AIComponentsConfiguration()[AiID];
            var PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();

            NpcFOVRingManager = new NpcInteractionRingManager(this);

            AIDestinationMoveManager = new AIDestinationMoveManager(interactiveObjectSharedData.InteractiveObjectSharedDataTypeInherentData.TransformMoveManagerComponent, agent, this.SendOnDestinationReachedEvent);
            NPCSpeedAdjusterManager = new NPCSpeedAdjusterManager(agent);

            this.puzzleAIBehavior = this.GetComponent<GenericPuzzleAIBehavior>();
            var aIBheaviorBuildInputData = new AIBheaviorBuildInputData(agent, aiBehaviorInherentData.AIComponents, OnFOVChange, PuzzleEventsManager, playerManagerDataRetriever, interactiveObjectContainer, this.AiID, this.objectCollider, this.ForceTickAI, aiPositionsManager);
            ((GenericPuzzleAIBehavior)this.puzzleAIBehavior).Init(agent, (GenericPuzzleAIComponents)aIBheaviorBuildInputData.aIComponents, aIBheaviorBuildInputData.OnFOVChange, aIBheaviorBuildInputData.ForceUpdateAIBehavior,
                    aIBheaviorBuildInputData.PuzzleEventsManager, aIBheaviorBuildInputData.InteractiveObjectContainer, aIBheaviorBuildInputData.aiID, aIBheaviorBuildInputData.aiCollider, aIBheaviorBuildInputData.PlayerManagerDataRetriever, interactiveObjectSharedData.InteractiveObjectSharedDataTypeInherentData.TransformMoveManagerComponent, aIBheaviorBuildInputData.AIPositionsManager);
            ContextMarkVisualFeedbackManager = new ContextMarkVisualFeedbackManager(this, NpcFOVRingManager, puzzleCOnfigurationmanager);
            AnimationVisualFeedbackManager = new AnimationVisualFeedbackManager(animator, animationConfiguration);
            LineVisualFeedbackManager = new LineVisualFeedbackManager(this);
            NPCAIAnimationManager = new AIAnimationManager(animator, animationConfiguration);

            this.GetComponent<InRangeColliderTracker>().IfNotNull((InRangeColliderTracker) => InRangeColliderTracker.Init());

            //Sight listeners
            this.GetComponent<InteractiveObjectType>().IfNotNull((InteractiveObjectType) => InteractiveObjectType.GetEnabledOrDisabledModule<ObjectSightModule>().IfNotNull((ObjectSightModule) => ObjectSightModule.RegisterSightTrackingListener(this)));

            //Intiialize with 0 time
            this.TickWhenTimeFlows(0, 0);
        }

        public void TickWhenTimeFlows(in float d, in float timeAttenuationFactor)
        {
            this.ComputeAINewDestination(d, timeAttenuationFactor);
            AIDestinationMoveManager.Tick(d, timeAttenuationFactor, ref this.NPCAIDestinationContext);
            NPCSpeedAdjusterManager.Tick(d, timeAttenuationFactor);
        }

        internal void TickAlways(float d, float timeAttenuationFactor)
        {
            NPCAIAnimationManager.TickAlways(d, timeAttenuationFactor);
            NpcFOVRingManager.Tick(d);
            ContextMarkVisualFeedbackManager.Tick(d);
            LineVisualFeedbackManager.Tick(d, this.transform.position);
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
            puzzleAIBehavior.TickAI(d, timeAttenuationFactor, ref this.NPCAIDestinationContext);
            AIDestinationMoveManager.SetDestination(ref this.NPCAIDestinationContext);
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

            var oldGizmoColor = Gizmos.color;
            Gizmos.color = Color.yellow;
            if (this.agent != null && this.agent.hasPath)
            {
                Vector3 startPoint = this.agent.path.corners[0];
                for (var i = 1; i < this.agent.path.corners.Length; i++)
                {
                    Gizmos.DrawLine(startPoint, this.agent.path.corners[i]);
                    startPoint = this.agent.path.corners[i];
                }
            }
            Gizmos.color = oldGizmoColor;
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
        public void OnProjectileTriggerEnter(LaunchProjectileModule launchProjectile)
        {
            this.puzzleAIBehavior.ReceiveEvent(new ProjectileTriggerEnterAIBehaviorEvent(launchProjectile));
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

        internal void OnAIAttractedStart(AttractiveObjectModule attractiveObject)
        {
            this.LineVisualFeedbackManager.OnAttractiveObjectStart(attractiveObject.AttractiveObjectId);
            this.ContextMarkVisualFeedbackManager.ReceiveEvent(ContextMarkVisualFeedbackEvent.ATTRACTED_START, this.AiID);
        }
        internal void OnAIAttractedEnd()
        {
            this.LineVisualFeedbackManager.OnAttractiveObjectEnd();
            this.ContextMarkVisualFeedbackManager.ReceiveEvent(ContextMarkVisualFeedbackEvent.DELETE, this.AiID);
        }
        public void OnAttractiveObjectDestroyed(AttractiveObjectModule attractiveObjectToDestroy)
        {
            this.puzzleAIBehavior.OnAttractiveObjectDestroyed(attractiveObjectToDestroy);
        }
        public void OnDestinationReached()
        {
            puzzleAIBehavior.OnDestinationReached();
            // empty tick to immediately choose the next position
            this.ForceTickAI();
        }

        public void OnDisarmObjectTriggerEnter(DisarmObjectModule disarmObjectModule)
        {
            this.puzzleAIBehavior.ReceiveEvent(new DisarmingObjectEnterAIbehaviorEvent(disarmObjectModule));
        }
        public void OnDisarmObjectTriggerExit(DisarmObjectModule disarmObjectModule)
        {
            this.puzzleAIBehavior.ReceiveEvent(new DisarmingObjectExitAIbehaviorEvent(disarmObjectModule));
        }

        public void OnDisarmObjectStart(DisarmObjectModule disarmObjectModule)
        {
            this.NPCAIAnimationManager.OnDisarmObjectStart(disarmObjectModule);
        }
        public void OnDisarmObjectEnd()
        {
            this.NPCAIAnimationManager.OnDisarmObjectEnd();
        }
        public void SightInRangeEnter(ColliderWithCollisionType trackedCollider)
        {
            this.puzzleAIBehavior.ReceiveEvent(new SightInRangeEnterAIBehaviorEvent(trackedCollider));
        }

        public void SightInRangeExit(ColliderWithCollisionType trackedCollider)
        {
            this.puzzleAIBehavior.ReceiveEvent(new SightInRangeExitAIBehaviorEvent(trackedCollider));
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
        public ExtendedBounds GetAverageModelBoundLocalSpace()
        {
            return BoundsHelper.GetAverageRendererBounds(this.GetRenderers());
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

    class AnimationVisualFeedbackManager
    {
        private Animator Animator;
        private AnimationConfiguration AnimationConfiguration;

        public AnimationVisualFeedbackManager(Animator animator, AnimationConfiguration AnimationConfiguration)
        {
            Animator = animator;
            this.AnimationConfiguration = AnimationConfiguration;
        }

        internal void OnHittedByProjectileFirstTime()
        {
            AnimationPlayerHelper.Play(this.Animator, this.AnimationConfiguration.ConfigurationInherentData[AnimationID.HITTED_BY_PROJECTILE_1ST], 0f);
        }

        internal void OnEscapeWithoutTargetStart()
        {
            AnimationPlayerHelper.Play(this.Animator, this.AnimationConfiguration.ConfigurationInherentData[AnimationID.HITTED_BY_PROJECTILE_2ND], 0f);
        }

        internal void OnAIFearedStunnedEnded()
        {
            AnimationPlayerHelper.Play(this.Animator, this.AnimationConfiguration.ConfigurationInherentData[AnimationID.POSE_OVERRIVE_LISTENING], 0f);
        }

        internal void OnAIFearedStunned()
        {
            AnimationPlayerHelper.Play(this.Animator, this.AnimationConfiguration.ConfigurationInherentData[AnimationID.FEAR], 0f);
        }
    }
}
