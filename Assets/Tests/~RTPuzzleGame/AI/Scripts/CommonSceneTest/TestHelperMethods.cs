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

    }

}
