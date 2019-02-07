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

    private Vector3 newDestination;

    #region State
    private bool newDestinationDefined;
    #endregion

    public bool NewDestinationDefined { get => newDestinationDefined; }
    public Vector3 NewDestination { get => newDestination; }

    public void Tick()
    {
        newDestinationDefined = false;
        TickAI();
    }
    protected abstract void TickAI();
    public abstract void TickGizmo();

    protected void SetDestination(Vector3 destination)
    {
        newDestinationDefined = true;
        newDestination = destination;
    }
}
