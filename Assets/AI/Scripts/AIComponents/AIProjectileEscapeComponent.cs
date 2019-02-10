using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    [System.Serializable]
    public class AIProjectileEscapeComponent : MonoBehaviour, AIComponentInitializerMessageReceiver
    {
        public float SamplePositionPrecision;
        public float EscapeDistance;

        public void InitializeContainer(AIComponents aIComponents)
        {
            aIComponents.AIProjectileEscapeComponent = this;
        }
    }

    public class AIProjectileEscapeManager
    {
        #region External Dependencies
        private NavMeshAgent escapingAgent;
        #endregion

        #region Internal Dependencies
        private AIProjectileEscapeComponent AIProjectileEscapeComponent;
        #endregion

        #region State
        private bool isEscaping;
        private Nullable<Vector3> escapeDestination;
        #endregion

        private Action<Vector3> SetAgentPosition;

        public bool IsEscaping { get => isEscaping; }
        public Vector3? EscapeDestination { get => escapeDestination; }

        public AIProjectileEscapeManager(NavMeshAgent escapingAgent, AIProjectileEscapeComponent AIProjectileEscapeComponent)
        {
            this.AIProjectileEscapeComponent = AIProjectileEscapeComponent;
            this.escapingAgent = escapingAgent;
        }

        public void ClearEscapeDestination()
        {
            escapeDestination = null;
        }

        public void GizmoTick()
        {
            if (EscapeDestination.HasValue)
            {
                Gizmos.DrawWireSphere(EscapeDestination.Value, 2f);
            }
        }

        public void OnTriggerEnter(Collider collider, CollisionType collisionType)
        {
            if (collisionType.IsRTPProjectile)
            {
                isEscaping = true;
                escapeDestination = ComputeEscapePoint(collider);
            }
        }

        private Nullable<Vector3> ComputeEscapePoint(Collider collider)
        {
            var escapeDirection = (escapingAgent.transform.position - collider.bounds.center).normalized;
            var escapeDirectionProjected = Vector3.ProjectOnPlane(escapeDirection, escapingAgent.transform.up);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(escapingAgent.transform.position + (escapeDirectionProjected * AIProjectileEscapeComponent.EscapeDistance), out hit, AIProjectileEscapeComponent.SamplePositionPrecision, NavMesh.AllAreas))
            {
                return hit.position;
            }
            return null;

        }

        internal void OnDestinationReached()
        {
            isEscaping = false;
        }

        public void OnLaunchProjectileDestroyed(LaunchProjectile launchProjectile)
        {
        }

        public void OnTriggerExit(Collider collider, CollisionType collisionType)
        {
            if (collisionType.IsRTPProjectile)
            {
                isEscaping = false;
            }
        }



    }

}
