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
        private AIWarningZoneComponentManager AIWarningZoneComponentManager;
        #endregion

        public MouseAIBehavior(NavMeshAgent selfAgent, AIRandomPatrolComponent AIRandomPatrolComponent, AIProjectileEscapeComponent AIProjectileEscapeComponent, AIWarningZoneComponent AIWarningZoneComponent) : base(selfAgent)
        {
            AIWarningZoneComponentManager = new AIWarningZoneComponentManager(selfAgent, AIWarningZoneComponent);
            AIRandomPatrolComponentManager = new AIRandomPatrolComponentMananger(selfAgent, AIRandomPatrolComponent);
            AIProjectileEscapeManager = new AIProjectileEscapeManager(selfAgent, AIProjectileEscapeComponent);
        }

        public override Nullable<Vector3> TickAI()
        {
            AIWarningZoneComponentManager.TickComponent();
            if (AIProjectileEscapeManager.IsEscaping)
            {
                AIRandomPatrolComponentManager.OnDestinationReached();
                var escapeDestination = AIProjectileEscapeManager.TickComponent();
                if (escapeDestination.HasValue)
                {
                    escapeDestination = escapeDestination.Value;
                    AIProjectileEscapeManager.ClearEscapeDestination();
                }
                return escapeDestination;
            }
            else
            {
                if (AIWarningZoneComponentManager.IsInWarningZone())
                {
                    Debug.Log("force escape");
                    AIRandomPatrolComponentManager.OnDestinationReached();
                    var escapeDestination = AIProjectileEscapeManager.TickComponent(true);
                    if (escapeDestination.HasValue)
                    {
                        escapeDestination = escapeDestination.Value;
                        AIProjectileEscapeManager.ClearEscapeDestination();
                    }
                    return escapeDestination;
                }
                else
                {
                    AIProjectileEscapeManager.ClearEscapeDestination();
                    return AIRandomPatrolComponentManager.TickComponent();
                }
            }

        }

        public override void TickGizmo()
        {
            // Gizmos.DrawWireSphere(NewDestination, 2f);
            AIProjectileEscapeManager.GizmoTick();
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
            GUILayout.Label("IsEscaping : " + AIProjectileEscapeManager.IsEscaping);
            GUILayout.Label("IsInWarningZone : " + AIWarningZoneComponentManager.IsInWarningZone());
        }
    }

}
