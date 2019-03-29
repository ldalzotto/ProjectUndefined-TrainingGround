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
    }

    public abstract class PuzzleAIBehavior<C> : IPuzzleAIBehavior<C> where C : AbstractAIComponents
    {
        protected C AIComponents;

        #region External Dependencies
        protected NavMeshAgent selfAgent;
        protected Action<FOV> OnFOVChange;
        #endregion

        protected AIFOVManager AIFOVManager;

        public PuzzleAIBehavior(NavMeshAgent selfAgent, C AIComponents, Action<FOV> OnFOVChange)
        {
            this.selfAgent = selfAgent;
            this.AIComponents = AIComponents;
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
                return new GenericPuzzleAIBehavior(aIBheaviorBuildInputData.selfAgent, (GenericPuzzleAIComponents)aIBheaviorBuildInputData.aIComponents, aIBheaviorBuildInputData.OnFOVChange, aIBheaviorBuildInputData.PuzzleEventsManager, aIBheaviorBuildInputData.aiID);
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
        public AiID aiID;

        public AIBheaviorBuildInputData(NavMeshAgent selfAgent, AbstractAIComponents aIComponents, Action<FOV> onFOVChange, PuzzleEventsManager puzzleEventsManager, AiID aiID)
        {
            this.selfAgent = selfAgent;
            this.aIComponents = aIComponents;
            OnFOVChange = onFOVChange;
            PuzzleEventsManager = puzzleEventsManager;
            this.aiID = aiID;
        }
    }
    
}
