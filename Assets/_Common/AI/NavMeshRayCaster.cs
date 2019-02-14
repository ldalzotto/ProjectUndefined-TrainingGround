using UnityEngine;
using UnityEngine.AI;

public class NavMeshRayCaster : MonoBehaviour
{
    public static void CastNavMeshRayFOVAgent(NavMeshAgent agent, float localAngle, float rayDistance, out NavMeshHit hit)
    {
        var castDirection = Quaternion.AngleAxis(-localAngle, agent.transform.up) * agent.transform.forward;
        Debug.Log("Casting to " + castDirection);
        var rayTargetPosition = agent.transform.position + (castDirection * rayDistance);
        NavMesh.Raycast(agent.transform.position, rayTargetPosition, out hit, NavMesh.AllAreas);
    }
}
