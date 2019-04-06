using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public interface IPuzzleAIBehavior<out C> where C : AbstractAIComponents
    {
        Nullable<Vector3> TickAI(in float d, in float timeAttenuationFactor);
        void TickGizmo();
        void OnProjectileTriggerEnter(Collider collider);
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
        private C aIComponents;

        #region External Dependencies
        protected NavMeshAgent selfAgent;
        protected Action<FOV> OnFOVChange;
        #endregion

        #region Data retrieval

#if UNITY_EDITOR
        public AbstractAIComponents AIComponents { get => aIComponents; set => aIComponents = (C)value; }
#endif
        #endregion

        protected AIFOVManager AIFOVManager;

        public PuzzleAIBehavior(NavMeshAgent selfAgent, C AIComponents, Action<FOV> OnFOVChange)
        {
            this.selfAgent = selfAgent;
            this.aIComponents = AIComponents;
            this.OnFOVChange = OnFOVChange;
            this.AIFOVManager = new AIFOVManager(selfAgent, OnFOVChange);
        }

        public abstract Nullable<Vector3> TickAI(in float d, in float timeAttenuationFactor);
        public abstract void TickGizmo();
        public abstract void OnProjectileTriggerEnter(Collider collider);
        public abstract void OnTriggerEnter(Collider collider);
        public abstract void OnTriggerStay(Collider collider);
        public abstract void OnTriggerExit(Collider collider);

        public abstract void OnDestinationReached();
        public virtual void OnAttractiveObjectDestroyed(AttractiveObjectType attractiveObjectToDestroy) { }

        public static IPuzzleAIBehavior<AbstractAIComponents> BuildAIBehaviorFromType(Type behaviorType, AIBheaviorBuildInputData aIBheaviorBuildInputData)
        {
            if (behaviorType == typeof(GenericPuzzleAIBehavior))
            {
                return new GenericPuzzleAIBehavior(aIBheaviorBuildInputData.selfAgent, (GenericPuzzleAIComponents)aIBheaviorBuildInputData.aIComponents, aIBheaviorBuildInputData.OnFOVChange, 
                    aIBheaviorBuildInputData.PuzzleEventsManager, aIBheaviorBuildInputData.TargetZoneContainer, aIBheaviorBuildInputData.aiID);
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

        public AIBheaviorBuildInputData(NavMeshAgent selfAgent, AbstractAIComponents aIComponents, Action<FOV> onFOVChange, PuzzleEventsManager puzzleEventsManager, TargetZoneContainer TargetZoneContainer, AiID aiID)
        {
            this.selfAgent = selfAgent;
            this.aIComponents = aIComponents;
            OnFOVChange = onFOVChange;
            PuzzleEventsManager = puzzleEventsManager;
            this.TargetZoneContainer = TargetZoneContainer;
            this.aiID = aiID;
        }
    }

}
