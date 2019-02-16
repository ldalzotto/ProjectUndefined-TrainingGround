using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class TargetZoneContainer : MonoBehaviour
    {
        private Dictionary<TargetZoneID, TargetZone> targetZones = new Dictionary<TargetZoneID, TargetZone>();

        public Dictionary<TargetZoneID, TargetZone> TargetZones { get => targetZones; }

        public void Add(TargetZone targetZone)
        {
            targetZones[targetZone.TargetZoneID] = targetZone;
        }

    }

}
