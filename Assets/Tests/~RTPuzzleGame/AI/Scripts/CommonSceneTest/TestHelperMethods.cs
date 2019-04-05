using UnityEngine;
using UnityEngine.AI;

namespace Tests
{
    public class TestHelperMethods
    {
        public static void SetAgentDestinationPositionReached(NavMeshAgent agent)
        {
            agent.velocity = Vector3.zero;
            agent.nextPosition = agent.destination;
            agent.transform.position = agent.destination;
        }

        public static void SetAgentDestinationPositionReached(NavMeshAgent agent, Vector3 worldPosition)
        {
            agent.Warp(worldPosition);
            agent.SetDestination(worldPosition);
            SetAgentDestinationPositionReached(agent);
        }

    }

}
