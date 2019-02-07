using UnityEngine;
using UnityEngine.AI;

class AIDestinationMoveManager
{
    private AIDestimationMoveManagerComponent AIDestimationMoveManagerComponent;
    private NavMeshAgent objectAgent;
    private Transform objectTransform;

    public AIDestinationMoveManager(AIDestimationMoveManagerComponent aIDestimationMoveManagerComponent, NavMeshAgent objectAgent, Transform objectTransform)
    {
        AIDestimationMoveManagerComponent = aIDestimationMoveManagerComponent;
        this.objectAgent = objectAgent;
        this.objectTransform = objectTransform;
    }

    public void Tick(float d)
    {
        if (objectAgent.velocity.normalized != Vector3.zero)
        {
            objectTransform.rotation = Quaternion.Slerp(objectTransform.rotation, Quaternion.LookRotation(objectAgent.velocity.normalized), d * AIDestimationMoveManagerComponent.AIRotationSpeed);
        }

        var playerMovementOrientation = (objectAgent.nextPosition - objectTransform.position).normalized;
        objectAgent.speed = AIDestimationMoveManagerComponent.SpeedMultiplicationFactor;
        objectTransform.position = objectAgent.nextPosition;
    }

    #region External Events
    public void SetDestination(Vector3 destination)
    {
        objectAgent.SetDestination(destination);
    }
    public void EnableAgent()
    {
        objectAgent.isStopped = false;
    }
    public void DisableAgent()
    {
        objectAgent.isStopped = true;
        objectAgent.nextPosition = objectAgent.transform.position;
    }
    #endregion
}

[System.Serializable]
public class AIDestimationMoveManagerComponent
{
    public float AIRotationSpeed;
    public float SpeedMultiplicationFactor;
}