using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{
    public class WayPointPath : MonoBehaviour
    {

        public List<WayPoint> WayPointsToFollow;
        public bool Loop;
        public WaypointPathId Id;


        private void OnDrawGizmos()
        {
            if (WayPointsToFollow != null && WayPointsToFollow.Count >= 2)
            {
                for (var i = 0; i < WayPointsToFollow.Count - 1; i++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(WayPointsToFollow[i].transform.position, WayPointsToFollow[i + 1].transform.position);
                }

                if (Loop)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(WayPointsToFollow[WayPointsToFollow.Count - 1].transform.position, WayPointsToFollow[0].transform.position);
                }
            }

        }

    }

    public enum WaypointPathId
    {
        ROAD_PATH_IN = 0,
        ROAD_PATH_OUT = 1
    }

}
