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
    public abstract void OnTriggerEnter(Collider collider);
    public abstract void OnTriggerStay(Collider collider);

    protected void SetDestination(Vector3 destination)
    {
        Debug.Log(Time.frameCount + " Set destination : " + destination);
        newDestinationDefined = true;
        newDestination = destination;
    }

    public abstract void OnDestinationReached();

}
