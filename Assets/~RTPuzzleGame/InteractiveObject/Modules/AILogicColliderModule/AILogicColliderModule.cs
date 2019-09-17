//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{

    public class AILogicColliderModule : RTPuzzle.InteractiveObjectModule
    {
        #region Internal Dependencies
        private AIObjectDataRetriever AIObjectDataRetirever;
        #endregion

        #region State
        private List<IAILogicColliderModuleListener> PhysicsEventListeners;
        private BoxCollider associatedCollider;
        #endregion

        #region Data Retrieval
        public BoxCollider GetCollider()
        {
            return this.associatedCollider;
        }
        #endregion

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval, IInteractiveObjectTypeEvents IInteractiveObjectTypeEvents)
        {
            this.AIObjectDataRetirever = interactiveObjectInitializationObject.ParentAIObjectTypeReference;
            this.associatedCollider = GetComponent<BoxCollider>();
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
        }

        #region Physics Events
        private void OnTriggerEnter(Collider other)
        {
            this.AIObjectDataRetirever.GetAIBehavior().OnTriggerEnter(other);
            if (this.PhysicsEventListeners != null)
            {
                foreach (var physicsEventListener in this.PhysicsEventListeners)
                {
                    physicsEventListener.OnTriggerEnter(other);
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            this.AIObjectDataRetirever.GetAIBehavior().OnTriggerStay(other);
            if (this.PhysicsEventListeners != null)
            {
                foreach (var physicsEventListener in this.PhysicsEventListeners)
                {
                    physicsEventListener.OnTriggerStay(other);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            this.AIObjectDataRetirever.GetAIBehavior().OnTriggerExit(other);
            if (this.PhysicsEventListeners != null)
            {
                foreach (var physicsEventListener in this.PhysicsEventListeners)
                {
                    physicsEventListener.OnTriggerExit(other);
                }
            }
        }
        #endregion

        #region External Events
        public void AddListener(IAILogicColliderModuleListener IAILogicColliderModuleListener)
        {
            if (this.PhysicsEventListeners == null) { this.PhysicsEventListeners = new List<IAILogicColliderModuleListener>(); }
            this.PhysicsEventListeners.Add(IAILogicColliderModuleListener);
        }
        #endregion

        public static class AILogicColliderModuleInstancer
        {
            public static void PopulateFromDefinition(AILogicColliderModule aILogicColliderModule, AILogicColliderModuleDefinition aILogicColliderModuleDefinition)
            {
                var boxCollider = aILogicColliderModule.GetComponent<BoxCollider>();
                boxCollider.center = aILogicColliderModuleDefinition.Center;
                boxCollider.size = aILogicColliderModuleDefinition.Size;
            }
        }

        public static AIObjectType FromCollisionType(CollisionType collisionType)
        {
            if (collisionType == null) { return null; }
            var AILogicColliderModule = collisionType.GetComponent<AILogicColliderModule>();
            if (AILogicColliderModule != null) { return AILogicColliderModule.GetComponentInParent<AIObjectType>(); }
            return null;
        }
    }

    public interface IAILogicColliderModuleListener
    {
        void OnTriggerEnter(Collider other);

        void OnTriggerStay(Collider other);

        void OnTriggerExit(Collider other);
    }
}
