using System;
using UnityEngine;
using UnityEngine.AI;

public abstract class RTPuzzleAIBehavior
{

    #region External Dependencies
    protected NavMeshAgent selfAgent;
    #endregion

    public RTPuzzleAIBehavior(NavMeshAgent selfAgent)
    {
        this.selfAgent = selfAgent;
    }

    public abstract Nullable<Vector3> TickAI();
    public abstract void TickGizmo();
    public abstract void OnTriggerEnter(Collider collider);
    public abstract void OnTriggerStay(Collider collider);
    public abstract void OnTriggerExit(Collider collider);

    public abstract void OnDestinationReached();

}
