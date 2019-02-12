using UnityEngine;
using UnityEngine.AI;

public class NavMeshRayCaster : MonoBehaviour
{
    public static void CastNavMeshRay(Vector3 startPosition, Vector3 escapeDirectionProjected, Quaternion rotation, float rayDistance, out NavMeshHit hit)
    {
        var rayTargetPosition = startPosition + (rotation * (escapeDirectionProjected * rayDistance));
        NavMesh.Raycast(startPosition, rayTargetPosition, out hit, NavMesh.AllAreas);
    }

    public static void CastNavMeshRay(Transform sourceTransform, Quaternion rotation, float rayDistance, out NavMeshHit hit)
    {
        var rayTargetPosition = sourceTransform.position + (rotation * (sourceTransform.forward * rayDistance));
        NavMesh.Raycast(sourceTransform.position, rayTargetPosition, out hit, NavMesh.AllAreas);
    }
}
