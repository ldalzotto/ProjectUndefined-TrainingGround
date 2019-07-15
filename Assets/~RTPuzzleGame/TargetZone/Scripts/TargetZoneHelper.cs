﻿using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public static class TargetZoneHelper
    {
        
        public static Collider[] GetTargetZonesTriggerColliders(InteractiveObjectContainer interactiveObjectContainer)
        {
            Collider[] foundColliders = new Collider[interactiveObjectContainer.TargetZones.Count];
            var targetZoneValues = interactiveObjectContainer.TargetZones.Values;
            var i = 0;
            foreach (var targetZone in interactiveObjectContainer.TargetZones.Values)
            {
                foundColliders[i] = targetZone.LevelCompletionTriggerModule.GetTargetZoneTriggerCollider();
                i++;
            }
            return foundColliders;
        }

        public static List<TargetZoneObjectModule> GetAllTargetZonesWhereDistanceCheckOverlaps(Bounds testingBound, InteractiveObjectContainer interactiveObjectContainer)
        {
            List<TargetZoneObjectModule> results = null;
            foreach (var targetZone in interactiveObjectContainer.TargetZones.Values)
            {
                if (targetZone.ZoneDistanceDetectionCollider.bounds.Intersects(testingBound))
                {
                    if (results == null)
                    {
                        results = new List<TargetZoneObjectModule>();
                    }
                    results.Add(targetZone);
                }
            }
            return results;
        }

    }

}