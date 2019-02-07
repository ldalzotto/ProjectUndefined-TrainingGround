using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class AIRandomPatrolComponent : MonoBehaviour
{
    public float MaxDistance;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, MaxDistance);
    }
}

public class AIRandomPatrolComponentMananger
{
    #region External Dependencies
    private NavMeshAgent patrollingAgent;
    #endregion

    private AIRandomPatrolComponent AIRandomPatrolComponent;

    private Vector3 destination;
    private bool destinationSet;

    public bool DestinationSet { get => destinationSet; }
    public Vector3 Destination { get => destination; }

    public AIRandomPatrolComponentMananger(NavMeshAgent patrollingAgent, AIRandomPatrolComponent AIRandomPatrolComponent)
    {
        this.patrollingAgent = patrollingAgent;
        this.AIRandomPatrolComponent = AIRandomPatrolComponent;
        destination = Vector3.zero;
    }

    public void TickComponent()
    {
        if (!destinationSet)
        {
            SetRandomDestination();
        }
    }

    private void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * AIRandomPatrolComponent.MaxDistance;
        randomDirection += AIRandomPatrolComponent.transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, AIRandomPatrolComponent.MaxDistance, NavMesh.AllAreas))
        {
            Debug.Log(hit.position);
            destinationSet = true;
            destination = hit.position;
            Coroutiner.Instance.StartCoroutine(OnDestinationReached());
        }
    }

    private IEnumerator OnDestinationReached()
    {
        Debug.Log(Time.frameCount + " Destination reached start.");
        yield return new WaitForEndOfFrame();
        yield return new WaitForNavAgentDestinationReached(patrollingAgent);
        Debug.Log(Time.frameCount + " Destination reached end.");
        destinationSet = false;
    }
}