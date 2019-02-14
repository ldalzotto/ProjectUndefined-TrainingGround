using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class FOVLocalToWorldTransformations
    {


        public static float AngleFromDirectionInFOVWorld(Vector3 localDirection, NavMeshAgent agent)
        {
            var localDirectionProjected = Vector3.ProjectOnPlane(localDirection, agent.transform.up);
            return Vector3.SignedAngle(localDirectionProjected, Vector3.forward, agent.transform.up);
        }

    }

}
