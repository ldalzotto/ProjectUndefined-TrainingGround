﻿using System;
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
        #endregion

        #region AI Managers
        private AIFOVManager AIFOVManager;
        #endregion

        public MouseAIBehavior(NavMeshAgent selfAgent, AIComponents aIComponents, Action<FOV> OnFOVChange, PuzzleEventsManager PuzzleEventsManager, AiID aiID) : base(selfAgent, OnFOVChange)
        {
            AIFOVManager = new AIFOVManager(selfAgent, OnFOVChange);
            AITargetZoneComponentManager = new AITargetZoneComponentManager(selfAgent, aIComponents.AITargetZoneComponent);
            AIRandomPatrolComponentManager = new AIRandomPatrolComponentMananger(selfAgent, aIComponents.AIRandomPatrolComponent);
            AIProjectileEscapeManager = new AIProjectileEscapeManager(selfAgent, aIComponents.AIProjectileEscapeComponent, aIComponents.AITargetZoneComponent, AIFOVManager, PuzzleEventsManager, aiID);
            AIFearStunComponentManager = new AIFearStunComponentManager(selfAgent, aIComponents.AIFearStunComponent, PuzzleEventsManager, aiID);
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

        #region Logical Conditions
        private bool IsReactingToProjectile()
        {
            return !this.AIFearStunComponentManager.IsFeared;
        }
        #endregion

        public override Nullable<Vector3> TickAI(float d, float timeAttenuationFactor)
        {
            AITargetZoneComponentManager.TickComponent();

            Vector3? newDirection = null;

            if (!AIFearStunComponentManager.IsFeared)
            {
                if (AIProjectileEscapeManager.IsEscaping())
                {
                    var escapeDestination = AIProjectileEscapeManager.GetCurrentEscapeDirection();
                    OnProjectileEscapeManagerUpdated(escapeDestination);
                    newDirection = escapeDestination;
                }
                else
                {
                    if (AITargetZoneComponentManager.IsInTargetZone())
                    {
                        var escapeDestination = AIProjectileEscapeManager.ForceComputeEscapePoint();
                        OnProjectileEscapeManagerUpdated(escapeDestination);
                        newDirection = escapeDestination;
                    }
                    else
                    {
                        AIProjectileEscapeManager.ClearEscapeDestination();
                        newDirection = AIRandomPatrolComponentManager.TickComponent(AIFOVManager);
                    }
                }

                var fearStunPosition = AIFearStunComponentManager.TickComponent(AIFOVManager);
                if (fearStunPosition.HasValue) //if is feared
                {
                    newDirection = fearStunPosition.Value; //position is the agent itself
                }
            } else
            {
                AIFearStunComponentManager.TickWhileFeared(d, timeAttenuationFactor);
            }
            
            return newDirection;
        }

        public override void TickGizmo()
        {
            // Gizmos.DrawWireSphere(NewDestination, 2f);
            AIRandomPatrolComponentManager.GizmoTick();
            AIProjectileEscapeManager.GizmoTick();
            AIFOVManager.GizmoTick();
        }

        public override void OnTriggerEnter(Collider collider)
        {
            var collisionType = collider.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                if (this.IsReactingToProjectile())
                {
                    AIProjectileEscapeManager.OnTriggerEnter(collider, collisionType);
                }
            }
        }

        public override void OnTriggerStay(Collider collider)
        {
        }

        public override void OnTriggerExit(Collider collider)
        {
            var collisionType = collider.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                AIProjectileEscapeManager.OnTriggerExit(collider, collisionType);
            }
        }

        public override void OnDestinationReached()
        {
            AIRandomPatrolComponentManager.OnDestinationReached();
            AIProjectileEscapeManager.OnDestinationReached();
        }

        public void DebugGUITick()
        {
            GUILayout.Label("IsPatrolling : " + AIRandomPatrolComponentManager.IsPatrolling());
            GUILayout.Label("IsEscaping : " + AIProjectileEscapeManager.IsEscaping());
            GUILayout.Label("IsInTargetZone : " + AITargetZoneComponentManager.IsInTargetZone());
            GUILayout.Label("IsStunFeared : " + AIFearStunComponentManager.IsFeared);
        }
    }

}
