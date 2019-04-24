﻿using UnityEngine;

namespace AdventureGame
{
    public class WayPoint : MonoBehaviour
    {
       private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
    }
}
