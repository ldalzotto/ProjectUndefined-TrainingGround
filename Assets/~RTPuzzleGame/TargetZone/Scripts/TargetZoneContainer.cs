﻿using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class TargetZoneContainer : MonoBehaviour
    {
        private Dictionary<TargetZoneID, TargetZone> targetZones = new Dictionary<TargetZoneID, TargetZone>();

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
                foundColliders[i] = targetZone.TargetZoneTriggerType.GetTargetZoneTriggerCollider();
                i++;
            }
            return foundColliders;
        }

        public TargetZone GetTargetZone(TargetZoneID targetZoneID)
        {
            return this.targetZones[targetZoneID];
        }


        public List<TargetZone> GetAllTargetZonesWhereDistanceCheckOverlaps(Bounds testingBound)
        {
            List<TargetZone> results = null;
            foreach (var targetZone in this.targetZones.Values)
            {
                if (targetZone.ZoneDistanceDetectionCollider.bounds.Intersects(testingBound))
                {
                    if (results == null)
                    {
                        results = new List<TargetZone>();
                    }
                    results.Add(targetZone);
                }
            }
            return results;
        }
        #endregion

    }

}
