using System;
using UnityEngine;
using UnityEngine.AI;

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
        void OnAttractiveObjectDestroyed(AttractiveObjectType attractiveObjectToDestroy);
#if UNITY_EDITOR
        AbstractAIComponents AIComponents { get; set; }
#endif
    }

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
        #endregion

        #region Data retrieval

#if UNITY_EDITOR
        public AbstractAIComponents AIComponents { get => aIComponents; set => aIComponents = (C)value; }
#endif
        public Action ForceUpdateAIBehavior { get => forceUpdateAIBehavior; }
        public AIFOVManager AIFOVManager { get => aIFOVManager; }
        #endregion

        #region External Events
        public void ReceiveEvent(PuzzleAIBehaviorExternalEvent externalEvent)
        {
            this.puzzleAIBehaviorExternalEventManager.ReceiveEvent(externalEvent);
        }
        #endregion

        protected AIFOVManager aIFOVManager;

        public PuzzleAIBehavior(NavMeshAgent selfAgent, C AIComponents, PuzzleAIBehaviorExternalEventManager puzzleAIBehaviorExternalEventManager, Action<FOV> OnFOVChange, Action ForceUpdateAIBehavior)
        {
            this.selfAgent = selfAgent;
            this.aIComponents = AIComponents;
            this.puzzleAIBehaviorExternalEventManager = puzzleAIBehaviorExternalEventManager;
            this.puzzleAIBehaviorExternalEventManager.Init(this);
            this.OnFOVChange = OnFOVChange;
            this.forceUpdateAIBehavior = ForceUpdateAIBehavior;
            this.aIFOVManager = new AIFOVManager(selfAgent, OnFOVChange);
        }

        public abstract Nullable<Vector3> TickAI(in float d, in float timeAttenuationFactor);
        public virtual void EndOfFixedTick()
        {
            this.puzzleAIBehaviorExternalEventManager.ConsumeEventsQueued();
        }
        public abstract void TickGizmo();
        public abstract void OnTriggerEnter(Collider collider);
        public abstract void OnTriggerStay(Collider collider);
        public abstract void OnTriggerExit(Collider collider);

        public abstract void OnDestinationReached();
        public virtual void OnAttractiveObjectDestroyed(AttractiveObjectType attractiveObjectToDestroy) { }

        public static IPuzzleAIBehavior<AbstractAIComponents> BuildAIBehaviorFromType(Type behaviorType, AIBheaviorBuildInputData aIBheaviorBuildInputData)
        {
            if (behaviorType == typeof(GenericPuzzleAIBehavior))
            {
                return new GenericPuzzleAIBehavior(aIBheaviorBuildInputData.selfAgent, (GenericPuzzleAIComponents)aIBheaviorBuildInputData.aIComponents, aIBheaviorBuildInputData.OnFOVChange, aIBheaviorBuildInputData.ForceUpdateAIBehavior,
                    aIBheaviorBuildInputData.PuzzleEventsManager, aIBheaviorBuildInputData.TargetZoneContainer, aIBheaviorBuildInputData.aiID, aIBheaviorBuildInputData.aiCollider);
            }
            return null;
        }
    }

    public struct AIBheaviorBuildInputData
    {
        public NavMeshAgent selfAgent;
        public AbstractAIComponents aIComponents;
        public Action<FOV> OnFOVChange;
        public PuzzleEventsManager PuzzleEventsManager;
        public TargetZoneContainer TargetZoneContainer;
        public AiID aiID;
        public Collider aiCollider;
        public Action ForceUpdateAIBehavior;

        public AIBheaviorBuildInputData(NavMeshAgent selfAgent, AbstractAIComponents aIComponents,
            Action<FOV> onFOVChange, PuzzleEventsManager puzzleEventsManager,
            TargetZoneContainer TargetZoneContainer, AiID aiID, Collider aiCollider, Action ForceUpdateAIBehavior)
        {
            this.selfAgent = selfAgent;
            this.aIComponents = aIComponents;
            OnFOVChange = onFOVChange;
            PuzzleEventsManager = puzzleEventsManager;
            this.TargetZoneContainer = TargetZoneContainer;
            this.aiID = aiID;
            this.aiCollider = aiCollider;
            this.ForceUpdateAIBehavior = ForceUpdateAIBehavior;
        }
    }

}
