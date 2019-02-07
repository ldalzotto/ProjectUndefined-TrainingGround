using System;
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
    private bool isMovingTowardsDestination;
    private Action<Vector3> WhenMovingTowardsDestination;

    public AIRandomPatrolComponentMananger(NavMeshAgent patrollingAgent, AIRandomPatrolComponent AIRandomPatrolComponent, Action<Vector3> WhenMovingTowardsDestination)
    {
        this.patrollingAgent = patrollingAgent;
        this.AIRandomPatrolComponent = AIRandomPatrolComponent;
        destination = Vector3.zero;
        this.WhenMovingTowardsDestination = WhenMovingTowardsDestination;
    }

    public void TickComponent()
    {
        if (isMovingTowardsDestination)
        {
            WhenMovingTowardsDestination(destination);
        }
        else
        {
            SetRandomDestination();
        }
    }

    private void SetRandomDestination()
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * AIRandomPatrolComponent.MaxDistance;
        randomDirection += AIRandomPatrolComponent.transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, AIRandomPatrolComponent.MaxDistance, NavMesh.AllAreas))
        {
            Debug.Log(hit.position);
            isMovingTowardsDestination = true;
            destination = hit.position;
        }
    }

    #region External Events
    public void OnDestinationReached()
    {
        isMovingTowardsDestination = false;
    }
    #endregion
}