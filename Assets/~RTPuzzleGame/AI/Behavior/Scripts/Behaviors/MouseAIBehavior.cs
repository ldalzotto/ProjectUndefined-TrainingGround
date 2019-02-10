using System;
using UnityEngine;
using UnityEngine.AI;

public class MouseAIBehavior : RTPuzzleAIBehavior
{

    #region AI Components
    private AIRandomPatrolComponentMananger AIRandomPatrolComponentManager;
    private AIProjectileEscapeManager AIProjectileEscapeManager;
    #endregion

    public MouseAIBehavior(NavMeshAgent selfAgent, AIRandomPatrolComponent AIRandomPatrolComponent, AIProjectileEscapeComponent AIProjectileEscapeComponent) : base(selfAgent)
    {
        AIRandomPatrolComponentManager = new AIRandomPatrolComponentMananger(selfAgent, AIRandomPatrolComponent);
        AIProjectileEscapeManager = new AIProjectileEscapeManager(selfAgent, AIProjectileEscapeComponent);
    }

    public override Nullable<Vector3> TickAI()
    {
        if (AIProjectileEscapeManager.IsEscaping)
        {
            AIRandomPatrolComponentManager.OnDestinationReached();
            Nullable<Vector3> escapeDestination = null;
            if (AIProjectileEscapeManager.EscapeDestination.HasValue)
            {
                AIRandomPatrolComponentManager.SetPosition(AIProjectileEscapeManager.EscapeDestination.Value);
                escapeDestination = AIProjectileEscapeManager.EscapeDestination.Value;
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

    public override void OnExternalEvent(AIBehaviorAbstractExternalEvent aIBehaviorAbstractExternalEvent)
    {
        if (aIBehaviorAbstractExternalEvent.GetType() == typeof(OnLaunchProjectileDestroyed))
        {
            var eventData = aIBehaviorAbstractExternalEvent.GetEventData() as LaunchProjectile;
            AIProjectileEscapeManager.OnLaunchProjectileDestroyed(eventData);
        }
    }

    public void DebugGUITick()
    {
        GUILayout.Label("IsEscaping : " + AIProjectileEscapeManager.IsEscaping);
    }
}
