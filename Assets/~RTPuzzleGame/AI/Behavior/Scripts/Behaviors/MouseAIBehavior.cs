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
        #endregion

        #region AI Managers
        private AIFOVManager AIFOVManager;
        #endregion

        public MouseAIBehavior(NavMeshAgent selfAgent, AIRandomPatrolComponent AIRandomPatrolComponent,
                AIProjectileEscapeComponent AIProjectileEscapeComponent, AITargetZoneComponent AIWarningZoneComponent
                    , Action<FOV> OnFOVChange) : base(selfAgent, OnFOVChange)
        {
            AIFOVManager = new AIFOVManager(selfAgent, OnFOVChange);
            AITargetZoneComponentManager = new AITargetZoneComponentManager(selfAgent, AIWarningZoneComponent);
            AIRandomPatrolComponentManager = new AIRandomPatrolComponentMananger(selfAgent, AIRandomPatrolComponent);
            AIProjectileEscapeManager = new AIProjectileEscapeManager(selfAgent, AIProjectileEscapeComponent, AIWarningZoneComponent, AIFOVManager);
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

        public override Nullable<Vector3> TickAI()
        {
            AITargetZoneComponentManager.TickComponent();
            if (AIProjectileEscapeManager.IsEscaping())
            {
                var escapeDestination = AIProjectileEscapeManager.GetCurrentEscapeDirection();
                OnProjectileEscapeManagerUpdated(escapeDestination);
                return escapeDestination;
            }
            else
            {
                if (AITargetZoneComponentManager.IsInTargetZone())
                {
                    var escapeDestination = AIProjectileEscapeManager.ForceComputeEscapePoint();
                    OnProjectileEscapeManagerUpdated(escapeDestination);
                    return escapeDestination;
                }
                else
                {
                    AIProjectileEscapeManager.ClearEscapeDestination();
                    return AIRandomPatrolComponentManager.TickComponent(AIFOVManager);
                }
            }

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
                AIProjectileEscapeManager.OnTriggerEnter(collider, collisionType);
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
        }
    }

}
