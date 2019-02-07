using UnityEngine;
using UnityEngine.AI;

public class AIProjectileEscapeComponent : MonoBehaviour
{
}

public class AIProjectileEscapeManager
{
    #region External Dependencies
    private NavMeshAgent escapingAgent;
    #endregion

    #region State
    private bool isEscaping;
    #endregion

    public AIProjectileEscapeManager(NavMeshAgent escapingAgent)
    {
        this.escapingAgent = escapingAgent;
    }

    public void OnTriggerEnter(Collider collider, CollisionType collisionType)
    {
        if (collisionType.IsRTPProjectile)
        {
            isEscaping = true;
            Debug.Log("FOUND PROJECTILE");
        }
    }

    public void OnTriggerStay(Collider collider, CollisionType collisionType)
    {
        if (collisionType.IsRTPProjectile)
        {
            var b = collider.ClosestPointOnBounds(escapingAgent.transform.position);
        }
    }

}
