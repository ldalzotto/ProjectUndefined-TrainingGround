using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    public class PhysicsHelper
    {
        public static bool PhysicsRayInContactWithColliders(Ray ray, Vector3 targetPoint, Collider[] colliders)
        {
            var raycastHits = Physics.RaycastAll(ray, Vector3.Distance(ray.origin, targetPoint));
            for (var i = 0; i < raycastHits.Length; i++)
            {
                foreach (var collider in colliders)
                {
                    if (raycastHits[i].collider.GetInstanceID() == collider.GetInstanceID())
                    {
                        return true;
                    }
                }

            }
            return false;
        }
    }

}
