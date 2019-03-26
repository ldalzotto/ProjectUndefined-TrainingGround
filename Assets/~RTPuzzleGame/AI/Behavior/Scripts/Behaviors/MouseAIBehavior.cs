using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{

    public class MouseAIBehavior : PuzzleAIBehavior
    {

        #region AI Components
        private AIRandomPatrolComponentMananger AIRandomPatrolComponentManager;
        private AIProjectileEscapeManager AIProjectileEscapeManager;
        private AITargetZoneComponentManager AITargetZoneComponentManager;
        private AIFearStunComponentManager AIFearStunComponentManager;
        private AIAttractiveObjectManager AIAttractiveObjectComponent;
        #endregion

        #region AI Managers
        private AIFOVManager AIFOVManager;
        #endregion

        public MouseAIBehavior(NavMeshAgent selfAgent, AIComponents aIComponents, Action<FOV> OnFOVChange, PuzzleEventsManager PuzzleEventsManager, AiID aiID) : base(selfAgent, OnFOVChange)
        {
            AIFOVManager = new AIFOVManager(selfAgent, OnFOVChange);
            AITargetZoneComponentManager = new AITargetZoneComponentManager(selfAgent, aIComponents.AITargetZoneComponent);
            AIRandomPatrolComponentManager = new AIRandomPatrolComponentMananger(selfAgent, aIComponents.AIRandomPatrolComponent);
            AIProjectileEscapeManager = new AIProjectileEscapeManager(selfAgent, aIComponents.AIProjectileEscapeComponent, aIComponents.AITargetZoneComponent, AIFOVManager, PuzzleEventsManager, aiID,
                    AITargetZoneComponentManager.AITargetZoneComponentManagerDataRetrieval);
            AIFearStunComponentManager = new AIFearStunComponentManager(selfAgent, aIComponents.AIFearStunComponent, PuzzleEventsManager, aiID);
            AIAttractiveObjectComponent = new AIAttractiveObjectManager(selfAgent, aiID, PuzzleEventsManager);
        }

        #region Internal Events
        private void OnProjectileEscapeManagerUpdated(Vector3? escapeDestination)
        {
            AIRandomPatrolComponentManager.OnDestinationReached();
            if (escapeDestination.HasValue)
            {
                AIProjectileEscapeManager.ClearEscapeDestination();
            }
        }
        #endregion

        #region External Events
        public override void OnAttractiveObjectDestroyed(AttractiveObjectType attractiveObjectToDestroy)
        {
            if (AIAttractiveObjectComponent.IsDestructedAttractiveObjectEqualsToCurrent(attractiveObjectToDestroy))
            {
                AIAttractiveObjectComponent.ClearAttractedObject();
                this.OnDestinationReached();
            }
        }
        #endregion

        #region Logical Conditions
        private bool IsReactingToProjectile()
        {
            return !this.IsFeared();
        }
        #endregion

        #region Data Retrieval
        public FOV GetFOV()
        {
            return this.AIFOVManager.GetFOV();
        }
        #endregion

        public override Nullable<Vector3> TickAI(in float d, in float timeAttenuationFactor)
        {
            AITargetZoneComponentManager.TickComponent();

            Vector3? newDirection = null;

            if (this.IsFeared())
            {
                AIFearStunComponentManager.TickWhileFeared(d, timeAttenuationFactor);
            }
            else
            {
                var fearStunPosition = AIFearStunComponentManager.TickComponent(AIFOVManager);
                if (this.IsFeared())
                {
                    newDirection = fearStunPosition.Value; //position is the agent itself
                }
                else
                {
                    if (this.IsEscaping())
                    {
                        var escapeDestination = AIProjectileEscapeManager.GetCurrentEscapeDirection();
                        OnProjectileEscapeManagerUpdated(escapeDestination);
                        newDirection = escapeDestination;
                    }
                    else
                    {
                        if (this.IsInTargetZone())
                        {
                            var escapeDestination = AIProjectileEscapeManager.ForceComputeEscapePoint();
                            OnProjectileEscapeManagerUpdated(escapeDestination);
                            newDirection = escapeDestination;
                        }
                        else
                        {
                            AIProjectileEscapeManager.ClearEscapeDestination();
                            if (this.IsInfluencedByAttractiveObject())
                            {
                                newDirection = AIAttractiveObjectComponent.TickComponent();
                            }
                            else
                            {
                                newDirection = AIRandomPatrolComponentManager.TickComponent(AIFOVManager);
                            }
                        }
                    }
                }
            }

            return newDirection;
        }

        public override void TickGizmo()
        {
            AIRandomPatrolComponentManager.GizmoTick();
            AIProjectileEscapeManager.GizmoTick();
            AIFOVManager.GizmoTick();
        }

        public override void OnProjectileTriggerEnter(Collider collider)
        {
            var collisionType = collider.GetComponent<CollisionType>();
            this.OnDestinationReached(true, false, true); //no reset of projectile fov FOV intersection
            AIProjectileEscapeManager.OnTriggerEnter(collider, collisionType);
        }

        public override void OnTriggerEnter(Collider collider)
        {
            var collisionType = collider.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                if (collisionType.IsRTAttractiveObject)
                {
                    if (!AIProjectileEscapeManager.IsEscaping() && !this.IsFeared())
                    {
                        this.OnDestinationReached();
                        AIAttractiveObjectComponent.OnTriggerEnter(collider, collisionType);
                    }
                }
            }
        }

        public override void OnTriggerStay(Collider collider)
        {
            var collisionType = collider.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                if (!this.IsEscaping() && !this.IsFeared())
                {
                    AIAttractiveObjectComponent.OnTriggerStay(collider, collisionType);
                }
            }
        }

        public override void OnTriggerExit(Collider collider)
        {
            var collisionType = collider.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                /*
                if (collisionType.IsRTPProjectile)
                {
                    AIProjectileEscapeManager.OnTriggerExit(collider, collisionType);
                }
                */
                if (collisionType.IsRTAttractiveObject)
                {
                    if (!AIProjectileEscapeManager.IsEscaping() && !this.IsFeared())
                    {
                        AIAttractiveObjectComponent.OnTriggerExit(collider, collisionType);
                    }
                }
            }
        }

        private void OnDestinationReached(bool randomPatrolReset, bool projectileEscapeReset, bool attractiveObjectReset)
        {
            if (randomPatrolReset)
            {
                AIRandomPatrolComponentManager.OnDestinationReached();
            }

            if (projectileEscapeReset)
            {
                AIProjectileEscapeManager.OnDestinationReached();
            }

            if (attractiveObjectReset)
            {
                AIAttractiveObjectComponent.OnDestinationReached();
            }
        }

        public override void OnDestinationReached()
        {
            bool resetAttractiveObjectComponent = !this.AIAttractiveObjectComponent.HasSensedThePresenceOfAnAttractiveObject(); //reset only if attractive object
            this.OnDestinationReached(true, true, resetAttractiveObjectComponent);
        }

        #region State Retrieval
        public bool IsPatrolling() { return AIRandomPatrolComponentManager.IsPatrolling(); }
        public bool IsFeared() { return AIFearStunComponentManager.IsFeared; }
        public bool IsEscaping() { return AIProjectileEscapeManager.IsEscaping(); }
        public bool IsInTargetZone() { return AITargetZoneComponentManager.IsInTargetZone(); }
        public bool IsInfluencedByAttractiveObject() { return AIAttractiveObjectComponent.IsInfluencedByAttractiveObject(); }
        #endregion

        public void DebugGUITick()
        {
            GUILayout.Label("IsPatrolling : " + AIRandomPatrolComponentManager.IsPatrolling());
            GUILayout.Label("IsEscaping : " + AIProjectileEscapeManager.IsEscaping());
            GUILayout.Label("IsInTargetZone : " + AITargetZoneComponentManager.IsInTargetZone());
            GUILayout.Label("IsStunFeared : " + AIFearStunComponentManager.IsFeared);
            GUILayout.Label("IsAttracted : " + AIAttractiveObjectComponent.IsInfluencedByAttractiveObject());
        }
    }

}
