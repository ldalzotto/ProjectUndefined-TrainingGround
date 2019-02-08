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

    private bool isMovingTowardsDestination;

    public AIRandomPatrolComponentMananger(NavMeshAgent patrollingAgent, AIRandomPatrolComponent AIRandomPatrolComponent)
    {
        this.patrollingAgent = patrollingAgent;
        this.AIRandomPatrolComponent = AIRandomPatrolComponent;
    }

    public Nullable<Vector3> TickComponent()
    {
        if (!isMovingTowardsDestination)
        {
            return SetRandomDestination();
        }
        return null;
    }

    private Nullable<Vector3> SetRandomDestination()
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * AIRandomPatrolComponent.MaxDistance;
        randomDirection += AIRandomPatrolComponent.transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, AIRandomPatrolComponent.MaxDistance, NavMesh.AllAreas))
        {
            isMovingTowardsDestination = true;
            return hit.position;
        }
        return null;
    }

    #region External Events
    public void OnDestinationReached()
    {
        isMovingTowardsDestination = false;
    }
    public void SetPosition(Vector3 worldPosition)
    {
        AIRandomPatrolComponent.transform.position = worldPosition;
    }
    #endregion
}