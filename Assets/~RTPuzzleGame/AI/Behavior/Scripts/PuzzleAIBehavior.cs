using GameConfigurationID;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static RTPuzzle.AIBehaviorManagerContainer;

namespace RTPuzzle
{
    public interface IPuzzleAIBehavior<out C> where C : AbstractAIComponents
    {
        Nullable<Vector3> TickAI(in float d, in float timeAttenuationFactor);
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
    public abstract class PuzzleAIBehavior<C> : IPuzzleAIBehavior<C> where C : AbstractAIComponents
    {
        protected C aIComponents;

        #region External Dependencies
        protected NavMeshAgent selfAgent;
        protected Action<FOV> OnFOVChange;
        #endregion

        #region Internal Dependencies
        private Action forceUpdateAIBehavior;
        protected PuzzleAIBehaviorExternalEventManager puzzleAIBehaviorExternalEventManager;
        protected Dictionary<Type, List<Func<bool>>> aiBehaviorExternalEventInterruptionMatrix;
        protected AIBehaviorManagerContainer aIBehaviorManagerContainer;
        #endregion

        #region Data retrieval

#if UNITY_EDITOR
        public AbstractAIComponents AIComponents { get => aIComponents; set => aIComponents = (C)value; }
        public PuzzleAIBehaviorExternalEventManager PuzzleAIBehaviorExternalEventManager { get => puzzleAIBehaviorExternalEventManager; }
#endif
        public Action ForceUpdateAIBehavior { get => forceUpdateAIBehavior; }
        public AIFOVManager AIFOVManager { get => aIFOVManager; }
        #endregion

        #region AI Manager Availability
        public bool EvaluateAIManagerAvailabilityToTheFirst(in InterfaceAIManager aiManager, EvaluationType evaluationType = EvaluationType.INCLUDED)
        {
            return this.aIBehaviorManagerContainer.EvaluateAIManagerAvailabilityToTheFirst(aiManager, evaluationType);
        }
        #endregion

        #region External Events
        public void ReceiveEvent(PuzzleAIBehaviorExternalEvent externalEvent)
        {
            this.puzzleAIBehaviorExternalEventManager.ReceiveEvent(externalEvent);
        }
        #endregion

        protected AIFOVManager aIFOVManager;

        public PuzzleAIBehavior(NavMeshAgent selfAgent, C AIComponents,
            PuzzleAIBehaviorExternalEventManager puzzleAIBehaviorExternalEventManager, Action<FOV> OnFOVChange, Action ForceUpdateAIBehavior)
        {
            this.selfAgent = selfAgent;
            this.aIComponents = AIComponents;
            this.puzzleAIBehaviorExternalEventManager = puzzleAIBehaviorExternalEventManager;
            this.puzzleAIBehaviorExternalEventManager.Init(this);
            this.OnFOVChange = OnFOVChange;
            this.forceUpdateAIBehavior = ForceUpdateAIBehavior;
            this.aIFOVManager = new AIFOVManager(selfAgent, OnFOVChange);
        }

        public Nullable<Vector3> TickAI(in float d, in float timeAttenuationFactor)
        {
            // (1) - Call the BeforeManagersUpdate callbacks.
            foreach (var aiManager in this.aIBehaviorManagerContainer.GetAllAIManagers())
            {
                aiManager.BeforeManagersUpdate(d, timeAttenuationFactor);
            }

            // (2) - Computing the first enabled AI Manager next position.
            var orderedAIManagers = this.aIBehaviorManagerContainer.AIManagersByExecutionOrder.Values;
            foreach (var aiManager in orderedAIManagers)
            {
                if (aiManager.IsManagerEnabled())
                {
                    return aiManager.OnManagerTick(d, timeAttenuationFactor);
                }
            }

            // (3) - If nothing has been detected active. Fallback to the last ai manager.
            return orderedAIManagers[orderedAIManagers.Count - 1].OnManagerTick(d, timeAttenuationFactor);
        }

        /// <summary>
        /// Reseting all manager state. This is called before enabling a manager.
        /// </summary>
        public void ManagersStateReset(List<InterfaceAIManager> exceptions = null)
        {
            foreach (var aiManager in this.aIBehaviorManagerContainer.GetAllAIManagers())
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

        public virtual void OnDestinationReached()
        {
            foreach (var aiManager in this.aIBehaviorManagerContainer.GetAllAIManagers())
            {
                aiManager.OnDestinationReached();
            }
        }
        public virtual void OnAttractiveObjectDestroyed(AttractiveObjectTypeModule attractiveObjectToDestroy) { }

        public static IPuzzleAIBehavior<AbstractAIComponents> BuildAIBehaviorFromType(Type behaviorType, AIBheaviorBuildInputData aIBheaviorBuildInputData)
        {
            if (behaviorType == typeof(GenericPuzzleAIBehavior))
            {
                return new GenericPuzzleAIBehavior(aIBheaviorBuildInputData.selfAgent, (GenericPuzzleAIComponents)aIBheaviorBuildInputData.aIComponents, aIBheaviorBuildInputData.OnFOVChange, aIBheaviorBuildInputData.ForceUpdateAIBehavior,
                    aIBheaviorBuildInputData.PuzzleEventsManager, aIBheaviorBuildInputData.InteractiveObjectContainer, aIBheaviorBuildInputData.aiID, aIBheaviorBuildInputData.aiCollider, aIBheaviorBuildInputData.PlayerManagerDataRetriever, aIBheaviorBuildInputData.AIDestimationMoveManagerComponent);
            }
            return null;
        }

        #region Interuption matrix
        public bool DoesEventInteruptManager(Type externalEventType)
        {
            if (this.aiBehaviorExternalEventInterruptionMatrix.ContainsKey(externalEventType))
            {
                foreach (var interuptionCondition in this.aiBehaviorExternalEventInterruptionMatrix[externalEventType])
                {
                    if (interuptionCondition.Invoke())
                    {
                        //at least one of interruption trigger is true -> we interrupt
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion
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
        public AIDestimationMoveManagerComponent AIDestimationMoveManagerComponent;

        public AIBheaviorBuildInputData(NavMeshAgent selfAgent, AbstractAIComponents aIComponents,
            Action<FOV> onFOVChange, PuzzleEventsManager puzzleEventsManager, PlayerManagerDataRetriever PlayerManagerDataRetriever,
            InteractiveObjectContainer InteractiveObjectContainer, AiID aiID, Collider aiCollider, Action ForceUpdateAIBehavior, AIDestimationMoveManagerComponent AIDestimationMoveManagerComponent)
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
