using UnityEngine;
using UnityEngine.AI;

public class MouseAIBehavior : RTPuzzleAIBehavior
{

    #region AI Components
    private AIRandomPatrolComponentMananger AIRandomPatrolComponentManager;
    private AIProjectileEscapeManager AIProjectileEscapeManager;
    #endregion

    public MouseAIBehavior(NavMeshAgent selfAgent, AIRandomPatrolComponent AIRandomPatrolComponent) : base(selfAgent)
    {
        AIRandomPatrolComponentManager = new AIRandomPatrolComponentMananger(selfAgent, AIRandomPatrolComponent, SetDestination);
        AIProjectileEscapeManager = new AIProjectileEscapeManager(selfAgent);
    }

    protected override void TickAI()
    {
        AIRandomPatrolComponentManager.TickComponent();
    }

    public override void TickGizmo()
    {
        Gizmos.DrawWireSphere(NewDestination, 2f);
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
        var collisionType = collider.GetComponent<CollisionType>();
        if (collisionType != null)
        {
            AIProjectileEscapeManager.OnTriggerStay(collider, collisionType);
        }
    }

    public override void OnDestinationReached()
    {
        AIRandomPatrolComponentManager.OnDestinationReached();
    }
}
