using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class FOVLocalToWorldTransformations
    {

        /// <summary>
        /// <para>
        /// The FOV space is world space vector <see cref="Vector3.forward"/>.
        /// </para>
        /// <para>
        /// The <paramref name="localEscapeDirection"/> is projected to FOV space.
        /// Then, the angle between <see cref="Vector3.forward"/> and the projected escape direction is computed.
        /// </para>
        /// 
        /// </summary>
        /// <param name="localEscapeDirection"></param>
        /// <param name="agent"></param>
        /// <returns></returns>
        public static float AngleFromDirectionInFOVSpace(Vector3 localEscapeDirection, NavMeshAgent agent)
        {
            //TODO -> is reacting correctly with slopes ?
            var localDirectionProjected = Vector3.ProjectOnPlane(localEscapeDirection, Vector3.up);// agent.transform.up);
            return Vector3.SignedAngle(localDirectionProjected, Vector3.forward, Vector3.up);// agent.transform.up);
        }

    }

}
