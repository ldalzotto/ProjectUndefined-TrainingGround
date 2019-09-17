using UnityEngine;
using UnityEngine.AI;
using CoreGame;
using GameConfigurationID;
using static AIMovementDefinitions;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTPuzzle
{
    public interface AIObjectTypeInternalEventsListener
    {
        void ForceTickAI();
    }

    public interface AIObjectTypeSpeedSetter
    {
        void SetSpeedAttenuationFactor(AIMovementSpeedDefinition AIMovementSpeedDefinition);
    }

    public interface AIObjectDataRetriever
    {
        IPuzzleAIBehavior GetAIBehavior();
        IInteractiveObjectTypeDataRetrieval GetInteractiveObjectTypeDataRetrieval();
    }

    public class AIObjectType : MonoBehaviour, IRenderBoundRetrievable, SightTrackingListener, AIObjectTypeInternalEventsListener, AIObjectTypeSpeedSetter, AIObjectDataRetriever
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
        #endregion

        #region AI Behavior Components
        [CustomEnum(ConfigurationType = typeof(AIObjectTypeDefinitionConfiguration), OpenToConfiguration = true)]
        public AIObjectTypeDefinitionID AIObjectTypeDefinitionID;

        [HideInInspector]
        public AIObjectID AiID;

        #endregion

        #region State
        private NPCAIDestinationContext NPCAIDestinationContext;
        private List<InterfaceAIManager> aiManagers;
        public List<InterfaceAIManager> AiManagers { get => aiManagers; set => aiManagers = value; }
        #endregion

        private InteractiveObjectSharedDataType interactiveObjectSharedData;
        private InteractiveObjectType associatedInteractivObject;

        private AIDestinationMoveManager AIDestinationMoveManager;
        private NPCSpeedAdjusterManager NPCSpeedAdjusterManager;

        private IPuzzleAIBehavior puzzleAIBehavior;
        private IContextMarkVisualFeedbackEvent IContextMarkVisualFeedbackEvent;
        private AnimationVisualFeedbackManager AnimationVisualFeedbackManager;

        public void Init()
        {
            Debug.Log(MyLog.Format("AIObjectType Init : " + this.AIObjectTypeDefinitionID.ToString()));

            var puzzleCOnfigurationmanager = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager;
            var puzzleStaticConfiguration = PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration;

            if (this.AIObjectTypeDefinitionID != AIObjectTypeDefinitionID.NONE)
            {
                puzzleCOnfigurationmanager.AIObjectTypeDefinitionConfiguration()[this.AIObjectTypeDefinitionID]
                    .DefineAIObject(this, puzzleStaticConfiguration.PuzzlePrefabConfiguration, puzzleCOnfigurationmanager.PuzzleGameConfiguration);
                this.GetComponent<InteractiveObjectType>().IfNotNull(InteractiveObjectType => InteractiveObjectType.Init(new InteractiveObjectInitializationObject() { ParentAIObjectTypeReference = this }));
            }

            this.PuzzleEventsManager = PuzzleGameSingletonInstances.PuzzleEventsManager;
            var NPCAIManagerContainer = PuzzleGameSingletonInstances.AIManagerContainer;
            NPCAIManagerContainer.OnNPCAiManagerCreated(this);
            var interactiveObjectContainer = PuzzleGameSingletonInstances.InteractiveObjectContainer;
            var playerManagerDataRetriever = PuzzleGameSingletonInstances.PlayerManagerDataRetriever;
            var coreConfigurationManager = CoreGameSingletonInstances.CoreConfigurationManager;
            var coreStaticConfiguration = CoreGameSingletonInstances.CoreStaticConfigurationContainer.CoreStaticConfiguration;
            var aiPositionsManager = PuzzleGameSingletonInstances.AIPositionsManager;
            var animationConfiguration = coreConfigurationManager.AnimationConfiguration();

            var animator = GetComponentInChildren<Animator>();

            this.interactiveObjectSharedData = GetComponent<InteractiveObjectSharedDataType>();
            this.associatedInteractivObject = GetComponent<InteractiveObjectType>();

            this.NPCAIDestinationContext = new NPCAIDestinationContext();

            agent = GetComponent<NavMeshAgent>();
            agent.updatePosition = false;
            agent.updateRotation = false;

            var PuzzleEventsManager = PuzzleGameSingletonInstances.PuzzleEventsManager;

            //FOV
            var FovModule = this.GetComponent<InteractiveObjectType>().GetModule<FovModule>();
            this.IContextMarkVisualFeedbackEvent = this.GetComponent<InteractiveObjectType>().GetModule<ContextMarkVisualFeedbackModule>();

            AIDestinationMoveManager = new AIDestinationMoveManager(interactiveObjectSharedData.InteractiveObjectSharedDataTypeInherentData.TransformMoveManagerComponent, agent, this.SendOnDestinationReachedEvent);
            NPCSpeedAdjusterManager = new NPCSpeedAdjusterManager(agent);

            this.puzzleAIBehavior = new GenericPuzzleAIBehavior();

            var aIBheaviorBuildInputData = new AIBheaviorBuildInputData(agent, PuzzleEventsManager, playerManagerDataRetriever,
                     interactiveObjectContainer, this.AiID, this.GetLogicCollider(), aiPositionsManager, interactiveObjectSharedData.InteractiveObjectSharedDataTypeInherentData.TransformMoveManagerComponent, this, this.associatedInteractivObject, this, FovModule);

            ((GenericPuzzleAIBehavior)this.puzzleAIBehavior).Init(this.aiManagers, aIBheaviorBuildInputData);

            if (animator != null)
            {
                AnimationVisualFeedbackManager = new AnimationVisualFeedbackManager(animator, animationConfiguration);
            }

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

        internal void EndOfFixedTick()
        {
            this.puzzleAIBehavior.EndOfFixedTick();
        }

        public void ForceTickAI()
        {
            this.ComputeAINewDestination(0, 0);
        }

        private void ComputeAINewDestination(in float d, in float timeAttenuationFactor)
        {
            puzzleAIBehavior.TickAI(d, timeAttenuationFactor, ref this.NPCAIDestinationContext);
            AIDestinationMoveManager.SetDestination(ref this.NPCAIDestinationContext);
        }

        public void GizmoTick()
        {
            puzzleAIBehavior.TickGizmo();
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
            if (this.IContextMarkVisualFeedbackEvent != null)
            {
                this.IContextMarkVisualFeedbackEvent.CreateExclamationMark();
            }

            this.AnimationVisualFeedbackManager.IfNotNull(AnimationVisualFeedbackManager => AnimationVisualFeedbackManager.OnHittedByProjectileFirstTime());
        }

        public void OnEscapeWithoutTargetStart()
        {
            if (this.IContextMarkVisualFeedbackEvent != null)
            {
                this.IContextMarkVisualFeedbackEvent.CreateDoubleExclamationMark();
            }
            this.AnimationVisualFeedbackManager.IfNotNull(AnimationVisualFeedbackManager => AnimationVisualFeedbackManager.OnEscapeWithoutTargetStart());
        }

        public void OnEscapeWithoutTargetEnd()
        {
            if (this.IContextMarkVisualFeedbackEvent != null)
            {
                this.IContextMarkVisualFeedbackEvent.Delete();
            }
        }

        public void OnAiAffectedByProjectileEnd()
        {
            if (this.IContextMarkVisualFeedbackEvent != null)
            {
                this.IContextMarkVisualFeedbackEvent.Delete();
            }
        }

        internal void OnGameOver()
        {
            if (this.IContextMarkVisualFeedbackEvent != null)
            {
                this.IContextMarkVisualFeedbackEvent.Delete();
            }
        }

        internal void OnAIFearedStunnedEnded()
        {
            this.puzzleAIBehavior.ReceiveEvent(new FearedEndAIBehaviorEvent(
                eventProcessedCallback: () =>
                {
                    this.AnimationVisualFeedbackManager.IfNotNull(AnimationVisualFeedbackManager => AnimationVisualFeedbackManager.OnAIFearedStunnedEnded());
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
                    this.AnimationVisualFeedbackManager.IfNotNull(AnimationVisualFeedbackManager => AnimationVisualFeedbackManager.OnAIFearedStunned());
                }
            ));
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

        public void SightInRangeEnter(ColliderWithCollisionType trackedCollider)
        {
            this.puzzleAIBehavior.ReceiveEvent(new SightInRangeEnterAIBehaviorEvent(trackedCollider));
        }

        public void SightInRangeExit(ColliderWithCollisionType trackedCollider)
        {
            this.puzzleAIBehavior.ReceiveEvent(new SightInRangeExitAIBehaviorEvent(trackedCollider));
        }

        public void SetSpeedAttenuationFactor(AIMovementSpeedDefinition AIMovementSpeedDefinition)
        {
            this.AIDestinationMoveManager.SetSpeedAttenuationFactor(AIMovementSpeedDefinition);
        }
        #endregion

        #region Internal Events
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
        public IPuzzleAIBehavior GetAIBehavior()
        {
            return this.puzzleAIBehavior;
        }

        public IInteractiveObjectTypeDataRetrieval GetInteractiveObjectTypeDataRetrieval()
        {
            return this.associatedInteractivObject;
        }

        public NavMeshAgent GetAgent()
        {
            if (this.agent == null) { this.agent = GetComponent<NavMeshAgent>(); }
            return this.agent;
        }
        public Collider GetLogicCollider()
        {
            return this.associatedInteractivObject.GetModule<AILogicColliderModule>().GetCollider();
        }
        public ExtendedBounds GetAverageModelBoundLocalSpace()
        {
            return BoundsHelper.GetAverageRendererBounds(this.GetRenderers());
        }
        #endregion


        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            var labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.normal.textColor = Color.white;
            Handles.Label(transform.position + new Vector3(0, -2f, 0), this.AIObjectTypeDefinitionID.ToString(), labelStyle);

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

    public class AnimationVisualFeedbackManager
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
