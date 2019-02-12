using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class AIRandomPatrolComponent : MonoBehaviour, AIComponentInitializerMessageReceiver
{
    public float MaxDistance;

    #region Internal Reference
    private AIComponentsContainer AIComponentsContainer;
    #endregion

    public void Start()
    {
        AIComponentsContainer = GetComponentInParent<AIComponentsContainer>();
    }

    private void OnDrawGizmos()
    {
        var col = Color.yellow;
        Gizmos.color = col;
        Gizmos.DrawWireSphere(transform.position, MaxDistance);
        if (AIComponentsContainer != null)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = col;
            Handles.Label(transform.position + (transform.up * MaxDistance), new GUIContent(AIComponentsContainer.AiID.ToString() + " random patrol."), style);
        }
    }

    public void InitializeContainer(AIComponents aIComponents)
    {
        aIComponents.AIRandomPatrolComponent = this;
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


    #region Logical Conditions
    public bool IsPatrolling()
    {
        return isMovingTowardsDestination;
    }
    #endregion
}