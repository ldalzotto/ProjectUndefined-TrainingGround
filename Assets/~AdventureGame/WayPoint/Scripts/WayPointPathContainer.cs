using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{
    public class WayPointPathContainer : MonoBehaviour
    {
        private bool hasInit;
        private Dictionary<WaypointPathId, WayPointPath> WayPointPaths = new Dictionary<WaypointPathId, WayPointPath>();

        public void Init()
        {
            if (!hasInit)
            {
                var wayPointPaths = GetComponentsInChildren<WayPointPath>();
                for (var i = 0; i < wayPointPaths.Length; i++)
                {
                    WayPointPaths.Add(wayPointPaths[i].Id, wayPointPaths[i]);
                }
                hasInit = true;
            }
        }

        public WayPointPath GetWayPointPath(WaypointPathId waypointPathId)
        {
            if (!hasInit)
            {
                Init();
            }
            return WayPointPaths[waypointPathId];
        }
    }

}
