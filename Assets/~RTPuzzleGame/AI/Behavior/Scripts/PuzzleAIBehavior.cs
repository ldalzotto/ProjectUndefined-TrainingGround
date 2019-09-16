using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public interface IPuzzleAIBehavior
    {
        void TickAI(float d, float timeAttenuationFactor, ref NPCAIDestinationContext NPCAIDestinationContext);
        void EndOfFixedTick();
        void TickGizmo();
        void ReceiveEvent(PuzzleAIBehaviorExternalEvent externalEvent);
        void OnTriggerEnter(Collider collider);
        void OnTriggerStay(Collider collider);
        void OnTriggerExit(Collider collider);

        void OnDestinationReached();
        void OnAttractiveObjectDestroyed(AttractiveObjectModule attractiveObjectToDestroy);
    }

    /// <summary>
    /// The <see cref="PuzzleAIBehavior{C}"/> is the entry point of AI.
    /// It inializes the <see cref="InterfaceAIManager"/>s based on <see cref="AbstractAIComponents"/> to a container <see cref="AIBehaviorManagerContainer"/>.
    /// The container <see cref="AIBehaviorManagerContainer"/> is responsible of the order of execution of <see cref="InterfaceAIManager"/>s.
    /// All change of AI behaviors are handles by the <see cref="PuzzleAIBehaviorExternalEventManager"/> where all events are processed.
    /// </summary>
    public abstract class PuzzleAIBehavior : IPuzzleAIBehavior
    {
        #region External Dependencies
        protected NavMeshAgent selfAgent;
        #endregion

        #region Internal Dependencies
        private AIObjectTypeInternalEventsListener AIObjectTypeInternalEventsListener;
        protected PuzzleAIBehaviorExternalEventManager puzzleAIBehaviorExternalEventManager;
        protected AIBehaviorManagerContainer aIBehaviorManagerContainer;
        protected InterfaceAIManager currentManagerState;
        #endregion

        #region Data retrieval

#if UNITY_EDITOR
        public PuzzleAIBehaviorExternalEventManager PuzzleAIBehaviorExternalEventManager { get => puzzleAIBehaviorExternalEventManager; }
#endif
        public void ForceUpdateAIBehavior() { this.AIObjectTypeInternalEventsListener.ForceTickAI(); }
        public AIFOVManager AIFOVManager { get => aIFOVManager; }

        protected List<InterfaceAIManager> GetAllManagers()
        {
            return this.aIBehaviorManagerContainer.AIManagersByExecutionOrder.Values.ToList();
        }
        public T GetAIManager<T>() where T : InterfaceAIManager
        {
            return (T)this.aIBehaviorManagerContainer.GetAIManager<T>();
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

        #region Logic Conditions
        public bool IsManagerInstanciated<T>() where T : InterfaceAIManager
        {
            return this.GetAIManager<T>() != null;
        }
        public bool IsManagerEnabled<T>() where T : InterfaceAIManager
        {
            if (IsManagerInstanciated<T>())
            {
                return this.GetAIManager<T>().IsManagerEnabled();
            }
            return false;
        }
        #endregion

        protected AIFOVManager aIFOVManager;

        protected void BaseInit(List<InterfaceAIManager> aiManagers, AIBheaviorBuildInputData AIBheaviorBuildInputData)
        {
            this.selfAgent = AIBheaviorBuildInputData.selfAgent;
            this.AIObjectTypeInternalEventsListener = AIBheaviorBuildInputData.AIObjectTypeInternalEventsListener;
            this.aIFOVManager = new AIFOVManager(selfAgent, this.AIObjectTypeInternalEventsListener.OnFOVChange);
            AIBheaviorBuildInputData.AIFOVManager = this.AIFOVManager;
            foreach (var aiManager in aiManagers)
            {
                aiManager.Init(AIBheaviorBuildInputData);
            }
            this.puzzleAIBehaviorExternalEventManager = AIBheaviorBuildInputData.GenericPuzzleAIBehaviorExternalEventManager;
            this.puzzleAIBehaviorExternalEventManager.Init(this);
        }

        protected void AfterChildInit()
        {
            this.currentManagerState = this.aIBehaviorManagerContainer.AIManagersByExecutionOrder.Values.Last();
        }

        public void TickAI(float d, float timeAttenuationFactor, ref NPCAIDestinationContext NPCAIDestinationContext)
        {
            NPCAIDestinationContext.Clear();

            // (1) - Call the BeforeManagersUpdate callbacks.
            this.BeforeManagersUpdate(d, timeAttenuationFactor);

            // (2) - If nothing has been detected active. Fallback to the last ai manager. 
            if (this.currentManagerState == null || !this.currentManagerState.IsManagerEnabled())
            {
                this.currentManagerState = this.aIBehaviorManagerContainer.AIManagersByExecutionOrder.Values.Last();
            }

            // (3) - Computing the first enabled AI Manager next position.
            this.currentManagerState.OnManagerTick(d, timeAttenuationFactor, ref NPCAIDestinationContext);
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

        public void ResetState(InterfaceAIManager stateToReset)
        {
            if (stateToReset != null)
            {
                stateToReset.OnStateReset();
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

        public virtual void OnAttractiveObjectDestroyed(AttractiveObjectModule attractiveObjectToDestroy) { }

    }

    public struct AIBheaviorBuildInputData
    {
        public NavMeshAgent selfAgent;
        public PuzzleEventsManager PuzzleEventsManager;
        public PlayerManagerDataRetriever PlayerManagerDataRetriever;
        public InteractiveObjectContainer InteractiveObjectContainer;
        public AIObjectID aiID;
        public Collider aiCollider;
        public AIPositionsManager AIPositionsManager;
        public TransformMoveManagerComponentV3 TransformMoveManagerComponent;
        public PuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager;
        public AIObjectTypeInternalEventsListener AIObjectTypeInternalEventsListener;
        public InteractiveObjectType AssociatedInteractiveObject;
        public AIObjectTypeSpeedSetter AIObjectTypeSpeedSetter;
        public AIFOVManager AIFOVManager;

        public AIBheaviorBuildInputData(NavMeshAgent selfAgent, PuzzleEventsManager puzzleEventsManager, PlayerManagerDataRetriever PlayerManagerDataRetriever,
            InteractiveObjectContainer InteractiveObjectContainer, AIObjectID aiID, Collider aiCollider, AIPositionsManager AIPositionsManager, TransformMoveManagerComponentV3 TransformMoveManagerComponent,
            AIObjectTypeInternalEventsListener AIObjectTypeInternalEventsListener, InteractiveObjectType AssociatedInteractiveObject, AIObjectTypeSpeedSetter AIObjectTypeSpeedSetter)
        {
            this.selfAgent = selfAgent;
            PuzzleEventsManager = puzzleEventsManager;
            this.PlayerManagerDataRetriever = PlayerManagerDataRetriever;
            this.InteractiveObjectContainer = InteractiveObjectContainer;
            this.aiID = aiID;
            this.aiCollider = aiCollider;
            this.AIPositionsManager = AIPositionsManager;
            this.TransformMoveManagerComponent = TransformMoveManagerComponent;
            this.GenericPuzzleAIBehaviorExternalEventManager = new GenericPuzzleAIBehaviorExternalEventManager();
            this.AIObjectTypeInternalEventsListener = AIObjectTypeInternalEventsListener;
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
            this.AIObjectTypeSpeedSetter = AIObjectTypeSpeedSetter;
            this.AIFOVManager = null;
        }
    }

}
