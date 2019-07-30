using CoreGame;
using GameConfigurationID;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public interface IPuzzleAIBehavior<out C> where C : AbstractAIComponents
    {
        Nullable<Vector3> TickAI(float d, float timeAttenuationFactor);
        void EndOfFixedTick();
        void TickGizmo();
        void ReceiveEvent(PuzzleAIBehaviorExternalEvent externalEvent);
        void OnTriggerEnter(Collider collider);
        void OnTriggerStay(Collider collider);
        void OnTriggerExit(Collider collider);

        void OnDestinationReached();
        void OnAttractiveObjectDestroyed(AttractiveObjectTypeModule attractiveObjectToDestroy);
#if UNITY_EDITOR
        AbstractAIComponents AIComponents { get; set; }
#endif
    }

    /// <summary>
    /// The <see cref="PuzzleAIBehavior{C}"/> is the entry point of AI.
    /// It inializes the <see cref="InterfaceAIManager"/>s based on <see cref="AbstractAIComponents"/> to a container <see cref="AIBehaviorManagerContainer"/>.
    /// The container <see cref="AIBehaviorManagerContainer"/> is responsible of the order of execution of <see cref="InterfaceAIManager"/>s.
    /// All change of AI behaviors are handles by the <see cref="PuzzleAIBehaviorExternalEventManager"/> where all events are processed.
    /// </summary>
    /// <typeparam name="C"></typeparam>
    public abstract class PuzzleAIBehavior<C> : MonoBehaviour, IPuzzleAIBehavior<C> where C : AbstractAIComponents
    {
        protected C aIComponents;

        #region External Dependencies
        protected NavMeshAgent selfAgent;
        protected Action<FOV> OnFOVChange;
        #endregion

        #region Internal Dependencies
        private Action forceUpdateAIBehavior;
        protected PuzzleAIBehaviorExternalEventManager puzzleAIBehaviorExternalEventManager;
        protected AIBehaviorManagerContainer aIBehaviorManagerContainer;
        protected InterfaceAIManager currentManagerState;
        #endregion

        #region Data retrieval

#if UNITY_EDITOR
        public AbstractAIComponents AIComponents { get => aIComponents; set => aIComponents = (C)value; }
        public PuzzleAIBehaviorExternalEventManager PuzzleAIBehaviorExternalEventManager { get => puzzleAIBehaviorExternalEventManager; }
#endif
        public Action ForceUpdateAIBehavior { get => forceUpdateAIBehavior; }
        public AIFOVManager AIFOVManager { get => aIFOVManager; }

        protected List<InterfaceAIManager> GetAllManagers()
        {
            return this.aIBehaviorManagerContainer.AIManagersByExecutionOrder.Values.ToList();
        }
        #endregion

        #region AI Manager Availability
        public bool IsManagerAllowedToBeActive(in InterfaceAIManager aiManager)
        {
            if (this.currentManagerState == null)
            {
                return true;
            }

            return this.aIBehaviorManagerContainer.GetAIManagerIndex(aiManager) <= this.aIBehaviorManagerContainer.GetAIManagerIndex(this.currentManagerState);
        }
        public bool IsCurrentManagerEquals(in InterfaceAIManager aiManager)
        {
            if (this.currentManagerState == null)
            {
                return true;
            }

            return this.aIBehaviorManagerContainer.GetAIManagerIndex(aiManager) == this.aIBehaviorManagerContainer.GetAIManagerIndex(this.currentManagerState);
        }
        #endregion

        #region External Events
        public void ReceiveEvent(PuzzleAIBehaviorExternalEvent externalEvent)
        {
            this.puzzleAIBehaviorExternalEventManager.ReceiveEvent(externalEvent);
        }
        #endregion

        protected AIFOVManager aIFOVManager;
        protected AISightVision aiSightVision;

        protected void BaseInit(NavMeshAgent selfAgent, C AIComponents,
            PuzzleAIBehaviorExternalEventManager puzzleAIBehaviorExternalEventManager, Action<FOV> OnFOVChange, Action ForceUpdateAIBehavior)
        {
            this.selfAgent = selfAgent;
            this.aIComponents = AIComponents;
            this.puzzleAIBehaviorExternalEventManager = puzzleAIBehaviorExternalEventManager;
            this.puzzleAIBehaviorExternalEventManager.Init(this);
            this.OnFOVChange = OnFOVChange;
            this.forceUpdateAIBehavior = ForceUpdateAIBehavior;
            this.aIFOVManager = new AIFOVManager(selfAgent, OnFOVChange);

            this.GetComponentInChildren<AISightVision>().IfNotNull(AISightVision =>
            {
                this.aiSightVision = AISightVision;
                this.aiSightVision.Init(puzzleAIBehaviorExternalEventManager);
            });
        }

        protected void AfterChildInit()
        {
            this.currentManagerState = this.aIBehaviorManagerContainer.AIManagersByExecutionOrder.Values.Last();
        }

        public Nullable<Vector3> TickAI(float d, float timeAttenuationFactor)
        {
            this.aiSightVision.IfNotNull(aiSightVision => aiSightVision.Tick(d));

            // (1) - Call the BeforeManagersUpdate callbacks.
            this.BeforeManagersUpdate(d, timeAttenuationFactor);

            // (2) - If nothing has been detected active. Fallback to the last ai manager. 
            if (this.currentManagerState == null || !this.currentManagerState.IsManagerEnabled())
            {
                this.currentManagerState = this.aIBehaviorManagerContainer.AIManagersByExecutionOrder.Values.Last();
            }

            // (3) - Computing the first enabled AI Manager next position.
            return this.currentManagerState.OnManagerTick(d, timeAttenuationFactor);
        }

        protected abstract void BeforeManagersUpdate(float d, float timeAttenuationFactor);

        public void SetManagerState(InterfaceAIManager newManagetState)
        {
            if (newManagetState == null)
            {
                this.ManagersStateReset();
                this.currentManagerState = newManagetState;
            }
            else if (newManagetState != this.currentManagerState && newManagetState.IsManagerEnabled())
            {
                this.ManagersStateReset(new List<InterfaceAIManager>() { newManagetState });
                this.currentManagerState = newManagetState;
            }
        }

        /// <summary>
        /// Reseting all manager state. This is called before enabling a manager.
        /// </summary>
        private void ManagersStateReset(List<InterfaceAIManager> exceptions = null)
        {
            foreach (var aiManager in this.GetAllManagers())
            {
                if ((exceptions == null) || (exceptions != null && !exceptions.Contains(aiManager)))
                {
                    aiManager.OnStateReset();
                }
            }
        }

        public virtual void EndOfFixedTick()
        {
            this.puzzleAIBehaviorExternalEventManager.ConsumeEventsQueued();
        }
        public abstract void TickGizmo();
        public abstract void OnTriggerEnter(Collider collider);
        public abstract void OnTriggerStay(Collider collider);
        public abstract void OnTriggerExit(Collider collider);

        public abstract void OnDestinationReached();

        public virtual void OnAttractiveObjectDestroyed(AttractiveObjectTypeModule attractiveObjectToDestroy) { }

    }

    public struct AIBheaviorBuildInputData
    {
        public NavMeshAgent selfAgent;
        public AbstractAIComponents aIComponents;
        public Action<FOV> OnFOVChange;
        public PuzzleEventsManager PuzzleEventsManager;
        public PlayerManagerDataRetriever PlayerManagerDataRetriever;
        public InteractiveObjectContainer InteractiveObjectContainer;
        public AiID aiID;
        public Collider aiCollider;
        public Action ForceUpdateAIBehavior;
        public TransformMoveManagerComponentV2 AIDestimationMoveManagerComponent;

        public AIBheaviorBuildInputData(NavMeshAgent selfAgent, AbstractAIComponents aIComponents,
            Action<FOV> onFOVChange, PuzzleEventsManager puzzleEventsManager, PlayerManagerDataRetriever PlayerManagerDataRetriever,
            InteractiveObjectContainer InteractiveObjectContainer, AiID aiID, Collider aiCollider, Action ForceUpdateAIBehavior, TransformMoveManagerComponentV2 AIDestimationMoveManagerComponent)
        {
            this.selfAgent = selfAgent;
            this.aIComponents = aIComponents;
            OnFOVChange = onFOVChange;
            PuzzleEventsManager = puzzleEventsManager;
            this.PlayerManagerDataRetriever = PlayerManagerDataRetriever;
            this.InteractiveObjectContainer = InteractiveObjectContainer;
            this.aiID = aiID;
            this.aiCollider = aiCollider;
            this.ForceUpdateAIBehavior = ForceUpdateAIBehavior;
            this.AIDestimationMoveManagerComponent = AIDestimationMoveManagerComponent;
        }
    }

}
