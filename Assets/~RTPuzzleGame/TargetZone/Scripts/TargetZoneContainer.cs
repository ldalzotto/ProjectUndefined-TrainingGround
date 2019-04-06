using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class TargetZoneContainer : MonoBehaviour
    {
        private Dictionary<TargetZoneID, TargetZone> targetZones = new Dictionary<TargetZoneID, TargetZone>();

        public Dictionary<TargetZoneID, TargetZone> TargetZones { get => targetZones; }

        public void Init()
        {
            var targetZones = GameObject.FindObjectsOfType<TargetZone>();
            if (targetZones != null)
            {
                foreach (var targetZone in targetZones)
                {
                    targetZone.Init();
                }
            }
        }

        public void Add(TargetZone targetZone)
        {
            targetZones[targetZone.TargetZoneID] = targetZone;
        }

        #region Data Retrieval
        public Collider[] GetTargetZonesTriggerColliders()
        {
            Collider[] foundColliders = new Collider[targetZones.Count];
            var targetZoneValues = this.targetZones.Values;
            var i = 0;
            foreach (var targetZone in targetZones.Values)
            {
                foundColliders[i] = targetZone.TargetZoneTriggerType.TargetZoneTriggerCollider;
                i++;
            }
            return foundColliders;
        }
        #endregion

    }

}
