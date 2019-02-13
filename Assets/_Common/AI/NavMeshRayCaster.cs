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
}
