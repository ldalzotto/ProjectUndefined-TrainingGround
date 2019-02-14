using UnityEngine;
using UnityEngine.AI;

public class NavMeshRayCaster : MonoBehaviour
{
    public static void CastNavMeshRay(Vector3 startPosition, Vector3 escapeDirectionProjected, Quaternion rotation, float rayDistance, out NavMeshHit hit)
    {
        var rayTargetPosition = startPosition + (rotation * (escapeDirectionProjected * rayDistance));
        NavMesh.Raycast(startPosition, rayTargetPosition, out hit, NavMesh.AllAreas);
    }

    public static void CastNavMeshRay(Transform startPosition, Quaternion rotation, float rayDistance, out NavMeshHit hit)
    {
        var rayTargetPosition = startPosition.position + (rotation * (startPosition.forward * rayDistance));
        NavMesh.Raycast(startPosition.position, rayTargetPosition, out hit, NavMesh.AllAreas);
    }

    public static void CastNavMeshRayWorldSpace(Transform startPosition, Quaternion worldRotation, float rayDistance, out NavMeshHit hit)
    {
        var rayTargetPosition = startPosition.position + (worldRotation * (Vector3.forward * rayDistance));
        NavMesh.Raycast(startPosition.position, rayTargetPosition, out hit, NavMesh.AllAreas);
    }

    public static void CastNavMeshRayFOVAgent(NavMeshAgent agent, float localAngle, float rayDistance, out NavMeshHit hit)
    {
        var castDirection = Quaternion.AngleAxis(-localAngle, agent.transform.up) * agent.transform.forward;
        Debug.Log("Casting to " + castDirection);
        var rayTargetPosition = agent.transform.position + (castDirection * rayDistance);
        NavMesh.Raycast(agent.transform.position, rayTargetPosition, out hit, NavMesh.AllAreas);
    }
}
