using GameConfigurationID;
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
                foundColliders[i] = targetZone.ILevelCompletionTriggerModuleDataRetriever.GetTargetZoneTriggerCollider();
                i++;
            }
            return foundColliders;
        }

        public static List<ITargetZoneModuleDataRetriever> GetAllTargetZonesWhereDistanceCheckOverlaps(Bounds testingBound, InteractiveObjectContainer interactiveObjectContainer)
        {
            List<ITargetZoneModuleDataRetriever> results = null;
            foreach (var targetZone in interactiveObjectContainer.TargetZones.Values)
            {
                if (targetZone.ZoneDistanceDetectionCollider.bounds.Intersects(testingBound))
                {
                    if (results == null)
                    {
                        results = new List<ITargetZoneModuleDataRetriever>();
                    }
                    results.Add(targetZone);
                }
            }
            return results;
        }

    }

}
