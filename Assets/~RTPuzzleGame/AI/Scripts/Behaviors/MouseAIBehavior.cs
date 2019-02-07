using UnityEngine;
using UnityEngine.AI;

public class MouseAIBehavior : RTPuzzleAIBehavior
{

    #region AI Components
    private AIRandomPatrolComponentMananger AIRandomPatrolComponentManager;
    #endregion

    public MouseAIBehavior(NavMeshAgent selfAgent, AIRandomPatrolComponent AIRandomPatrolComponent) : base(selfAgent)
    {
        AIRandomPatrolComponentManager = new AIRandomPatrolComponentMananger(selfAgent, AIRandomPatrolComponent);
    }

    protected override void TickAI()
    {
        AIRandomPatrolComponentManager.TickComponent();

        if (AIRandomPatrolComponentManager.DestinationSet)
        {
            SetDestination(AIRandomPatrolComponentManager.Destination);
        }
    }

    public override void TickGizmo()
    {
        Gizmos.DrawWireSphere(AIRandomPatrolComponentManager.Destination, 2f);
    }
}
